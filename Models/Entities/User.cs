using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Models.Entities
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Inventory> OwnedInventories { get; set; } = new List<Inventory>();
        public virtual ICollection<InventoryAccess> AccessibleInventories { get; set; } = new List<InventoryAccess>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<ItemLike> ItemLikes { get; set; } = new List<ItemLike>();
    }
}