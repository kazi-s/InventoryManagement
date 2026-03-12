using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models.ViewModels
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InventoryCount { get; set; }
        public int ItemCount { get; set; }
    }

    public class UserListViewModel
    {
        public List<UserManagementViewModel> Users { get; set; } = new List<UserManagementViewModel>();
        public string? SearchTerm { get; set; }
        public string SortBy { get; set; } = "name";
        public bool SortAscending { get; set; } = true;
    }
}