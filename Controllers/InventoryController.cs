using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Models.Entities;
using InventoryManagement.Models.ViewModels;
using System.Text.Json;
using System.Text;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ApplicationDbContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateInventoryViewModel
            {
                Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = UserManager.GetUserId(User);
                
                var inventory = new Inventory
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    CreatorId = userId,
                    IsPublic = model.IsPublic,
                    ImageUrl = model.ImageUrl,
                    CustomIdFormat = model.CustomIdFormat,
                    
                    String1Enabled = model.String1Enabled,
                    String1Name = model.String1Name,
                    String1Description = model.String1Description,
                    String1ShowInTable = model.String1ShowInTable,
                    
                    String2Enabled = model.String2Enabled,
                    String2Name = model.String2Name,
                    String2Description = model.String2Description,
                    String2ShowInTable = model.String2ShowInTable,
                    
                    String3Enabled = model.String3Enabled,
                    String3Name = model.String3Name,
                    String3Description = model.String3Description,
                    String3ShowInTable = model.String3ShowInTable,
                    
                    Text1Enabled = model.Text1Enabled,
                    Text1Name = model.Text1Name,
                    Text1Description = model.Text1Description,
                    Text1ShowInTable = model.Text1ShowInTable,
                    
                    Text2Enabled = model.Text2Enabled,
                    Text2Name = model.Text2Name,
                    Text2Description = model.Text2Description,
                    Text2ShowInTable = model.Text2ShowInTable,
                    
                    Text3Enabled = model.Text3Enabled,
                    Text3Name = model.Text3Name,
                    Text3Description = model.Text3Description,
                    Text3ShowInTable = model.Text3ShowInTable,
                    
                    Number1Enabled = model.Number1Enabled,
                    Number1Name = model.Number1Name,
                    Number1Description = model.Number1Description,
                    Number1ShowInTable = model.Number1ShowInTable,
                    
                    Number2Enabled = model.Number2Enabled,
                    Number2Name = model.Number2Name,
                    Number2Description = model.Number2Description,
                    Number2ShowInTable = model.Number2ShowInTable,
                    
                    Number3Enabled = model.Number3Enabled,
                    Number3Name = model.Number3Name,
                    Number3Description = model.Number3Description,
                    Number3ShowInTable = model.Number3ShowInTable,
                    
                    Bool1Enabled = model.Bool1Enabled,
                    Bool1Name = model.Bool1Name,
                    Bool1Description = model.Bool1Description,
                    Bool1ShowInTable = model.Bool1ShowInTable,
                    
                    Bool2Enabled = model.Bool2Enabled,
                    Bool2Name = model.Bool2Name,
                    Bool2Description = model.Bool2Description,
                    Bool2ShowInTable = model.Bool2ShowInTable,
                    
                    Bool3Enabled = model.Bool3Enabled,
                    Bool3Name = model.Bool3Name,
                    Bool3Description = model.Bool3Description,
                    Bool3ShowInTable = model.Bool3ShowInTable,
                    
                    DocumentLink1Enabled = model.DocumentLink1Enabled,
                    DocumentLink1Name = model.DocumentLink1Name,
                    DocumentLink1Description = model.DocumentLink1Description,
                    DocumentLink1ShowInTable = model.DocumentLink1ShowInTable,
                    
                    DocumentLink2Enabled = model.DocumentLink2Enabled,
                    DocumentLink2Name = model.DocumentLink2Name,
                    DocumentLink2Description = model.DocumentLink2Description,
                    DocumentLink2ShowInTable = model.DocumentLink2ShowInTable,
                    
                    DocumentLink3Enabled = model.DocumentLink3Enabled,
                    DocumentLink3Name = model.DocumentLink3Name,
                    DocumentLink3Description = model.DocumentLink3Description,
                    DocumentLink3ShowInTable = model.DocumentLink3ShowInTable
                };

                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = inventory.Id });
            }

            model.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Category)
                .Include(i => i.Items)
                    .ThenInclude(item => item.CreatedBy)
                    .ThenInclude(user => user != null ? user.ItemLikes : null)
                .Include(i => i.Comments)
                    .ThenInclude(c => c.User)
                .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
                .Include(i => i.AccessList)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", inventory.CategoryId);
            ViewBag.Theme = GetTheme();
            return View(inventory);
        }

        [HttpGet]
        public async Task<IActionResult> MyInventories()
        {
            var userId = UserManager.GetUserId(User);
            
            var inventories = await _context.Inventories
                .Where(i => i.CreatorId == userId)
                .Include(i => i.Category)
                .Include(i => i.Creator)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new InventoryListViewModel
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    ImageUrl = i.ImageUrl,
                    CategoryName = i.Category != null ? i.Category.Name : "",
                    CreatorName = i.Creator != null ? i.Creator.UserName ?? "" : "",
                    ItemCount = i.Items.Count,
                    IsPublic = i.IsPublic,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            return View(inventories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSettings(int id, string title, string description, int categoryId, string imageUrl, bool isPublic, string[] tags)
        {
            var inventory = await _context.Inventories
                .Include(i => i.InventoryTags)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            var userId = UserManager.GetUserId(User);
            if (!User.IsInRole("Admin") && inventory.CreatorId != userId)
            {
                return Forbid();
            }

            inventory.Title = title;
            inventory.Description = description;
            inventory.CategoryId = categoryId;
            inventory.ImageUrl = imageUrl;
            inventory.IsPublic = isPublic;
            inventory.UpdatedAt = DateTime.UtcNow;

            var currentTags = inventory.InventoryTags?.Select(it => it.Tag?.Name).ToList() ?? new List<string?>();
            var tagsToAdd = tags.Except(currentTags).ToList();
            var tagsToRemove = currentTags.Except(tags).ToList();

            foreach (var tagName in tagsToRemove)
            {
                var tagToRemove = inventory.InventoryTags?.FirstOrDefault(it => it.Tag?.Name == tagName);
                if (tagToRemove != null)
                {
                    _context.InventoryTags.Remove(tagToRemove);
                }
            }

            foreach (var tagName in tagsToAdd)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                _context.InventoryTags.Add(new InventoryTag
                {
                    InventoryId = inventory.Id,
                    TagId = tag.Id
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AutoSaveSettings(int id, string title, string description, int categoryId, string imageUrl, bool isPublic)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Title = title;
            inventory.Description = description;
            inventory.CategoryId = categoryId;
            inventory.ImageUrl = imageUrl;
            inventory.IsPublic = isPublic;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateIdPreview([FromBody] IdPreviewRequest request)
        {
            var inventory = await _context.Inventories.FindAsync(request.InventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            var preview = GenerateIdFromParts(request.Parts, inventory.Id);
            return Json(new { preview });
        }

        [HttpPost]
        public async Task<IActionResult> SaveIdFormat([FromBody] SaveIdFormatRequest request)
        {
            var inventory = await _context.Inventories.FindAsync(request.InventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            var userId = UserManager.GetUserId(User);
            if (!User.IsInRole("Admin") && inventory.CreatorId != userId)
            {
                return Forbid();
            }

            inventory.CustomIdFormat = JsonSerializer.Serialize(request.Parts);
            inventory.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccess([FromBody] UpdateAccessRequest request)
        {
            var inventory = await _context.Inventories.FindAsync(request.InventoryId);
            if (inventory == null) return NotFound();
            
            inventory.IsPublic = request.IsPublic;
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAccess([FromBody] AddUserAccessRequest request)
        {
            var existing = await _context.InventoryAccesses
                .FirstOrDefaultAsync(a => a.InventoryId == request.InventoryId && a.UserId == request.UserId);
            
            if (existing == null)
            {
                _context.InventoryAccesses.Add(new InventoryAccess
                {
                    InventoryId = request.InventoryId,
                    UserId = request.UserId
                });
                await _context.SaveChangesAsync();
            }
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserAccess([FromBody] RemoveUserAccessRequest request)
        {
            var access = await _context.InventoryAccesses
                .FirstOrDefaultAsync(a => a.InventoryId == request.InventoryId && a.UserId == request.UserId);
            
            if (access != null)
            {
                _context.InventoryAccesses.Remove(access);
                await _context.SaveChangesAsync();
            }
            
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFields([FromBody] UpdateFieldsRequest request)
        {
            var inventory = await _context.Inventories.FindAsync(request.InventoryId);
            if (inventory == null) return NotFound();
            
            foreach (var field in request.Fields)
            {
                switch (field.Type)
                {
                    case "string":
                        typeof(Inventory).GetProperty($"String{field.Index}Enabled")?.SetValue(inventory, field.Enabled);
                        typeof(Inventory).GetProperty($"String{field.Index}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"String{field.Index}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"String{field.Index}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        break;
                    case "number":
                        typeof(Inventory).GetProperty($"Number{field.Index}Enabled")?.SetValue(inventory, field.Enabled);
                        typeof(Inventory).GetProperty($"Number{field.Index}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"Number{field.Index}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"Number{field.Index}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        break;
                    case "bool":
                        typeof(Inventory).GetProperty($"Bool{field.Index}Enabled")?.SetValue(inventory, field.Enabled);
                        typeof(Inventory).GetProperty($"Bool{field.Index}Name")?.SetValue(inventory, field.Name);
                        typeof(Inventory).GetProperty($"Bool{field.Index}Description")?.SetValue(inventory, field.Description);
                        typeof(Inventory).GetProperty($"Bool{field.Index}ShowInTable")?.SetValue(inventory, field.ShowInTable);
                        break;
                }
            }
            
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        private string GenerateIdFromParts(List<IdPart> parts, int inventoryId)
        {
            var result = new StringBuilder();
            
            foreach (var part in parts)
            {
                switch (part.Type)
                {
                    case "fixed":
                        result.Append(part.Value ?? "");
                        break;
                        
                    case "random20":
                        var r20 = Random.Shared.Next(0, 1 << 20);
                        result.Append(ApplyFormat(r20.ToString("X5"), part.Format));
                        break;
                        
                    case "random32":
                        var r32 = Random.Shared.Next(0, int.MaxValue);
                        result.Append(ApplyFormat(r32.ToString("X8"), part.Format));
                        break;
                        
                    case "random6digit":
                        var r6 = Random.Shared.Next(100000, 999999);
                        result.Append(ApplyFormat(r6.ToString(), part.Format));
                        break;
                        
                    case "random9digit":
                        var r9 = Random.Shared.Next(100000000, 999999999);
                        result.Append(ApplyFormat(r9.ToString(), part.Format));
                        break;
                        
                    case "guid":
                        result.Append(ApplyFormat(Guid.NewGuid().ToString().Substring(0, 8), part.Format));
                        break;
                        
                    case "date":
                        result.Append(ApplyFormat(DateTime.UtcNow.ToString("yyyyMMdd"), part.Format));
                        break;
                        
                    case "sequence":
                        var maxSeq = _context.Items
                            .Where(i => i.InventoryId == inventoryId)
                            .Select(i => i.CustomId)
                            .AsEnumerable()
                            .Select(id =>
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(id, @"\d+$");
                                return match.Success ? int.Parse(match.Value) : 0;
                            })
                            .DefaultIfEmpty(0)
                            .Max() + 1;
                        result.Append(ApplyFormat(maxSeq.ToString(), part.Format ?? "D6"));
                        break;
                }
            }
            
            return result.ToString();
        }

        private string ApplyFormat(string value, string? format)
        {
            if (string.IsNullOrEmpty(format))
                return value;
                
            try
            {
                if (int.TryParse(value, out int intValue))
                {
                    return intValue.ToString(format);
                }
            }
            catch
            {
                // If formatting fails, return original
            }
            
            return value;
        }
    }

    public class IdPreviewRequest
    {
        public int InventoryId { get; set; }
        public List<IdPart> Parts { get; set; } = new List<IdPart>();
    }

    public class SaveIdFormatRequest
    {
        public int InventoryId { get; set; }
        public List<IdPart> Parts { get; set; } = new List<IdPart>();
    }

    public class IdPart
    {
        public string Type { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Format { get; set; }
        public int Order { get; set; }
    }

    public class UpdateAccessRequest
    {
        public int InventoryId { get; set; }
        public bool IsPublic { get; set; }
    }

    public class AddUserAccessRequest
    {
        public int InventoryId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class RemoveUserAccessRequest
    {
        public int InventoryId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateFieldsRequest
    {
        public int InventoryId { get; set; }
        public List<FieldConfig> Fields { get; set; } = new List<FieldConfig>();
    }

    public class FieldConfig
    {
        public string Type { get; set; } = string.Empty;
        public int Index { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool ShowInTable { get; set; }
    }
}