using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Models.Entities;
using InventoryManagement.Models.ViewModels;

namespace InventoryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Index
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string sortBy = "name", bool ascending = true)
        {
            var query = _userManager.Users.AsQueryable();

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.UserName != null && u.UserName.ToLower().Contains(search) ||
                    u.Email != null && u.Email.ToLower().Contains(search) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(search))
                );
            }

            // Get users with additional data
            var users = new List<UserManagementViewModel>();
            
            foreach (var user in await query.ToListAsync())
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var inventoryCount = await _context.Inventories.CountAsync(i => i.CreatorId == user.Id);
                var itemCount = await _context.Items.CountAsync(i => i.CreatedById == user.Id);

                users.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsBlocked = user.IsBlocked,
                    BlockedUntil = user.BlockedUntil,
                    IsAdmin = isAdmin,
                    CreatedAt = user.CreatedAt,
                    InventoryCount = inventoryCount,
                    ItemCount = itemCount
                });
            }

            // Apply sorting
            users = sortBy.ToLower() switch
            {
                "email" => ascending ? users.OrderBy(u => u.Email).ToList() : users.OrderByDescending(u => u.Email).ToList(),
                "created" => ascending ? users.OrderBy(u => u.CreatedAt).ToList() : users.OrderByDescending(u => u.CreatedAt).ToList(),
                "inventories" => ascending ? users.OrderBy(u => u.InventoryCount).ToList() : users.OrderByDescending(u => u.InventoryCount).ToList(),
                "items" => ascending ? users.OrderBy(u => u.ItemCount).ToList() : users.OrderByDescending(u => u.ItemCount).ToList(),
                "blocked" => ascending ? users.OrderBy(u => u.IsBlocked).ToList() : users.OrderByDescending(u => u.IsBlocked).ToList(),
                _ => ascending ? users.OrderBy(u => u.UserName).ToList() : users.OrderByDescending(u => u.UserName).ToList(),
            };

            var viewModel = new UserListViewModel
            {
                Users = users,
                SearchTerm = search,
                SortBy = sortBy,
                SortAscending = ascending
            };

            ViewBag.Theme = GetTheme();
            return View(viewModel);
        }

        // POST: Admin/ToggleAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            
            // Prevent admin from removing their own admin rights
            if (userId == currentUserId)
            {
                TempData["Warning"] = "You cannot remove admin access from yourself using this action. Use the profile page instead.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            
            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                _logger.LogInformation($"Admin {currentUserId} removed admin role from user {userId}");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                _logger.LogInformation($"Admin {currentUserId} added admin role to user {userId}");
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/ToggleBlock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(string userId, int? days = null)
        {
            var currentUserId = _userManager.GetUserId(User);
            
            // Admin cannot block themselves
            if (userId == currentUserId)
            {
                TempData["Error"] = "You cannot block yourself.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.IsBlocked)
            {
                // Unblock
                user.IsBlocked = false;
                user.BlockedUntil = null;
                _logger.LogInformation($"Admin {currentUserId} unblocked user {userId}");
            }
            else
            {
                // Block
                user.IsBlocked = true;
                user.BlockedUntil = days.HasValue ? DateTime.UtcNow.AddDays(days.Value) : null;
                _logger.LogInformation($"Admin {currentUserId} blocked user {userId} for {(days.HasValue ? days + " days" : "indefinitely")}");
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            
            // Admin cannot delete themselves
            if (userId == currentUserId)
            {
                TempData["Error"] = "You cannot delete yourself.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if user is admin
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Cannot delete admin users. Remove admin role first.";
                return RedirectToAction(nameof(Index));
            }

            // Delete user's data (cascade delete will handle related records)
            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                _logger.LogInformation($"Admin {currentUserId} deleted user {userId}");
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Error deleting user.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/UserDetails/5
        [HttpGet]
        public async Task<IActionResult> UserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var inventories = await _context.Inventories
                .Where(i => i.CreatorId == userId)
                .Include(i => i.Category)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    Category = i.Category != null ? i.Category.Name : "",
                    ItemCount = i.Items.Count,
                    i.CreatedAt
                })
                .ToListAsync();

            ViewBag.User = user;
            ViewBag.IsAdmin = isAdmin;
            ViewBag.Inventories = inventories;
            ViewBag.Theme = GetTheme();

            return View();
        }
    }
}