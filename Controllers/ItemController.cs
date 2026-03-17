using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Models.Entities;
using InventoryManagement.Models.ViewModels;
using System.Text.Json;
using System.Text;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ItemController> _logger;

        public ItemController(ApplicationDbContext context, ILogger<ItemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int inventoryId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Creator)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return NotFound();
            }

            var userId = UserManager.GetUserId(User);
            var hasAccess = User.IsInRole("Admin") || 
                           inventory.CreatorId == userId ||
                           inventory.IsPublic ||
                           await _context.InventoryAccesses.AnyAsync(a => a.InventoryId == inventoryId && a.UserId == userId);

            if (!hasAccess)
            {
                return Forbid();
            }

            var viewModel = new CreateItemViewModel
            {
                InventoryId = inventoryId,
                InventoryTitle = inventory.Title,
                Inventory = inventory,
                CustomId = GenerateCustomId(inventory) // Auto-generate ID
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            var inventory = await _context.Inventories.FindAsync(model.InventoryId);
            if (inventory == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = UserManager.GetUserId(User);
                    
                    var item = new Item
                    {
                        InventoryId = model.InventoryId,
                        CustomId = model.CustomId,
                        CreatedById = userId,
                        
                        StringValue1 = model.StringValue1,
                        StringValue2 = model.StringValue2,
                        StringValue3 = model.StringValue3,
                        
                        TextValue1 = model.TextValue1,
                        TextValue2 = model.TextValue2,
                        TextValue3 = model.TextValue3,
                        
                        NumberValue1 = model.NumberValue1,
                        NumberValue2 = model.NumberValue2,
                        NumberValue3 = model.NumberValue3,
                        
                        BoolValue1 = model.BoolValue1,
                        BoolValue2 = model.BoolValue2,
                        BoolValue3 = model.BoolValue3,
                        
                        DocumentLink1 = model.DocumentLink1,
                        DocumentLink2 = model.DocumentLink2,
                        DocumentLink3 = model.DocumentLink3
                    };

                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Inventory", new { id = model.InventoryId });
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique constraint") == true)
                {
                    ModelState.AddModelError("CustomId", "This Custom ID already exists in this inventory. Please use a different ID.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating item");
                    ModelState.AddModelError("", "An error occurred while saving the item.");
                }
            }

            model.Inventory = inventory;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items
                .Include(i => i.Inventory)
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var userId = UserManager.GetUserId(User);
            var hasAccess = User.IsInRole("Admin") || 
                           item.Inventory?.CreatorId == userId ||
                           item.CreatedById == userId;

            if (!hasAccess)
            {
                return Forbid();
            }

            var viewModel = new EditItemViewModel
            {
                Id = item.Id,
                InventoryId = item.InventoryId,
                InventoryTitle = item.Inventory?.Title ?? "",
                Inventory = item.Inventory,
                CustomId = item.CustomId,
                
                StringValue1 = item.StringValue1,
                StringValue2 = item.StringValue2,
                StringValue3 = item.StringValue3,
                
                TextValue1 = item.TextValue1,
                TextValue2 = item.TextValue2,
                TextValue3 = item.TextValue3,
                
                NumberValue1 = item.NumberValue1,
                NumberValue2 = item.NumberValue2,
                NumberValue3 = item.NumberValue3,
                
                BoolValue1 = item.BoolValue1 ?? false,
                BoolValue2 = item.BoolValue2 ?? false,
                BoolValue3 = item.BoolValue3 ?? false,
                
                DocumentLink1 = item.DocumentLink1,
                DocumentLink2 = item.DocumentLink2,
                DocumentLink3 = item.DocumentLink3
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditItemViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                var item = await _context.Items
                    .Include(i => i.Inventory)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (item == null)
                {
                    return NotFound();
                }

                item.CustomId = model.CustomId;
                item.StringValue1 = model.StringValue1;
                item.StringValue2 = model.StringValue2;
                item.StringValue3 = model.StringValue3;
                item.TextValue1 = model.TextValue1;
                item.TextValue2 = model.TextValue2;
                item.TextValue3 = model.TextValue3;
                item.NumberValue1 = model.NumberValue1;
                item.NumberValue2 = model.NumberValue2;
                item.NumberValue3 = model.NumberValue3;
                item.BoolValue1 = model.BoolValue1;
                item.BoolValue2 = model.BoolValue2;
                item.BoolValue3 = model.BoolValue3;
                item.DocumentLink1 = model.DocumentLink1;
                item.DocumentLink2 = model.DocumentLink2;
                item.DocumentLink3 = model.DocumentLink3;
                item.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Inventory", new { id = item.InventoryId });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique constraint") == true)
            {
                ModelState.AddModelError("CustomId", "This Custom ID already exists in this inventory.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Like([FromBody] LikeRequest request)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized();
            }

            var userId = UserManager.GetUserId(User);
            
            var existingLike = await _context.ItemLikes
                .FirstOrDefaultAsync(l => l.ItemId == request.ItemId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.ItemLikes.Remove(existingLike);
            }
            else
            {
                _context.ItemLikes.Add(new ItemLike
                {
                    ItemId = request.ItemId,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            var likeCount = await _context.ItemLikes.CountAsync(l => l.ItemId == request.ItemId);

            return Json(new { 
                success = true, 
                liked = existingLike == null,
                likes = likeCount 
            });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items
                .Include(i => i.Inventory)
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var inventoryId = item.InventoryId;
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Inventory", new { id = inventoryId });
        }

        private string GenerateCustomId(Inventory inventory)
        {
            var format = JsonSerializer.Deserialize<List<IdPart>>(inventory.CustomIdFormat) 
                ?? new List<IdPart>();

            if (!format.Any())
            {
                return Random.Shared.Next(100000, 999999).ToString();
            }

            var parts = new List<string>();
            
            foreach (var part in format)
            {
                switch (part.Type)
                {
                    case "fixed":
                        parts.Add(part.Value ?? "");
                        break;
                    case "random20":
                        parts.Add(Random.Shared.Next(0, 1 << 20).ToString("X5"));
                        break;
                    case "random32":
                        parts.Add(Random.Shared.Next(0, int.MaxValue).ToString("X8"));
                        break;
                    case "random6digit":
                        parts.Add(Random.Shared.Next(100000, 999999).ToString());
                        break;
                    case "random9digit":
                        parts.Add(Random.Shared.Next(100000000, 999999999).ToString());
                        break;
                    case "guid":
                        parts.Add(Guid.NewGuid().ToString().Substring(0, 8));
                        break;
                    case "date":
                        parts.Add(DateTime.UtcNow.ToString("yyyyMMdd"));
                        break;
                    case "sequence":
                        var maxSequence = _context.Items
                            .Where(i => i.InventoryId == inventory.Id)
                            .Select(i => i.CustomId)
                            .AsEnumerable()
                            .Select(id =>
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(id, @"\d+$");
                                return match.Success ? int.Parse(match.Value) : 0;
                            })
                            .DefaultIfEmpty(0)
                            .Max();
                        parts.Add((maxSequence + 1).ToString(part.Format ?? "D6"));
                        break;
                }
            }

            return string.Join("", parts);
        }
    }

    public class LikeRequest
    {
        public int ItemId { get; set; }
    }
}