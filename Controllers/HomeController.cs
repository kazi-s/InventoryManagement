using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var latestInventories = await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .OrderByDescending(i => i.CreatedAt)
                .Take(10)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    CreatorName = i.Creator != null ? i.Creator.UserName : "Unknown",
                    ItemCount = i.Items.Count
                })
                .ToListAsync();

            var popularInventories = await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .OrderByDescending(i => i.Items.Count)
                .Take(5)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Description,
                    i.ImageUrl,
                    CreatorName = i.Creator != null ? i.Creator.UserName : "Unknown",
                    ItemCount = i.Items.Count
                })
                .ToListAsync();

            var tags = await _context.Tags
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    Count = t.InventoryTags.Count
                })
                .ToListAsync();

            ViewBag.LatestInventories = latestInventories;
            ViewBag.PopularInventories = popularInventories;
            ViewBag.Tags = tags;
            ViewBag.Theme = GetTheme();

            return View();
        }
    }
}