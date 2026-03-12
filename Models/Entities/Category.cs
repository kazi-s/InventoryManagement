namespace InventoryManagement.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Predefined categories: Equipment, Furniture, Book, Other
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}