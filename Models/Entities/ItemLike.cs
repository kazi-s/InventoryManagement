namespace InventoryManagement.Models.Entities
{
    public class ItemLike
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Item? Item { get; set; }
        public virtual User? User { get; set; }
    }
}