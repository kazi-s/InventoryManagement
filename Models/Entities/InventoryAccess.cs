namespace InventoryManagement.Models.Entities
{
    public class InventoryAccess
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Inventory? Inventory { get; set; }
        public virtual User? User { get; set; }
    }
}