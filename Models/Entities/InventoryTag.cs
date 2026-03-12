namespace InventoryManagement.Models.Entities
{
    public class InventoryTag
    {
        public int InventoryId { get; set; }
        public int TagId { get; set; }
        
        // Navigation properties
        public virtual Inventory? Inventory { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}