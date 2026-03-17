using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Models.ViewModels;
using System.Linq;

namespace InventoryManagement.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ApplicationDbContext context, ILogger<SearchController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string q, string? tag = null, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q) && string.IsNullOrWhiteSpace(tag))
            {
                return View(new SearchResultsViewModel());
            }

            var pageSize = 10;
            var query = _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
                .AsQueryable();

            // Full-text search
            if (!string.IsNullOrWhiteSpace(q))
            {
                var searchTerms = q.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var term in searchTerms)
                {
                    var termParam = term;
                    query = query.Where(i => 
                        i.Title.ToLower().Contains(termParam) ||
                        (i.Description != null && i.Description.ToLower().Contains(termParam)) ||
                        (i.Creator != null && i.Creator.UserName != null && i.Creator.UserName.ToLower().Contains(termParam))
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(i => i.InventoryTags != null && 
                    i.InventoryTags.Any(it => it.Tag != null && it.Tag.Name == tag));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var inventories = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new InventorySearchResultViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    ImageUrl = i.ImageUrl,
                    CategoryName = i.Category != null ? i.Category.Name : "",
                    CreatorName = i.Creator != null ? i.Creator.UserName ?? "" : "",
                    ItemCount = i.Items.Count,
                    CreatedAt = i.CreatedAt,
                    Tags = i.InventoryTags != null ? i.InventoryTags.Select(it => it.Tag != null ? it.Tag.Name : string.Empty).ToList() : new List<string?>()
                })
                .ToListAsync();

            var popularTags = await _context.Tags
                .OrderByDescending(t => t.InventoryTags != null ? t.InventoryTags.Count : 0)
                .Take(20)
                .Select(t => new { t.Name, Count = t.InventoryTags != null ? t.InventoryTags.Count : 0 })
                .ToListAsync();

            var viewModel = new SearchResultsViewModel
            {
                Query = q,
                Tag = tag,
                Inventories = inventories,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PopularTags = popularTags.ToDictionary(t => t.Name ?? "", t => t.Count)
            };

            ViewBag.Theme = GetTheme();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Tag(string tag)
        {
            return await Index("", tag, 1);
        }

        [HttpGet]
        public async Task<IActionResult> Suggest(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new List<object>());
            }

            var suggestions = await _context.Inventories
                .Where(i => i.Title.ToLower().Contains(term.ToLower()))
                .OrderByDescending(i => i.Items.Count)
                .Take(5)
                .Select(i => new
                {
                    id = i.Id,
                    title = i.Title,
                    url = Url.Action("Details", "Inventory", new { id = i.Id })
                })
                .ToListAsync();

            return Json(suggestions);
        }
    }
}