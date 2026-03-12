namespace InventoryManagement.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Inventory? Inventory { get; set; }
        public virtual User? User { get; set; }
    }
}