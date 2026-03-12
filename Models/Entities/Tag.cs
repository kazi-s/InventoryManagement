namespace InventoryManagement.Models.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public virtual ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }
}