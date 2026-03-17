using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string CustomId { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public string? StringValue1 { get; set; }
        public string? StringValue2 { get; set; }
        public string? StringValue3 { get; set; }
        
        public string? TextValue1 { get; set; }
        public string? TextValue2 { get; set; }
        public string? TextValue3 { get; set; }
        
        public decimal? NumberValue1 { get; set; }
        public decimal? NumberValue2 { get; set; }
        public decimal? NumberValue3 { get; set; }
        
        public bool? BoolValue1 { get; set; }
        public bool? BoolValue2 { get; set; }
        public bool? BoolValue3 { get; set; }
        
        public string? DocumentLink1 { get; set; }
        public string? DocumentLink2 { get; set; }
        public string? DocumentLink3 { get; set; }
        
        public virtual Inventory? Inventory { get; set; }
        public virtual User? CreatedBy { get; set; }
        public virtual ICollection<ItemLike> Likes { get; set; } = new List<ItemLike>();
    }
}