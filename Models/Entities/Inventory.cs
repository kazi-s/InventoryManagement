using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public string CustomIdFormat { get; set; } = "[]";
        
        public bool String1Enabled { get; set; }
        public string? String1Name { get; set; }
        public string? String1Description { get; set; }
        public bool String1ShowInTable { get; set; }
        
        public bool String2Enabled { get; set; }
        public string? String2Name { get; set; }
        public string? String2Description { get; set; }
        public bool String2ShowInTable { get; set; }
        
        public bool String3Enabled { get; set; }
        public string? String3Name { get; set; }
        public string? String3Description { get; set; }
        public bool String3ShowInTable { get; set; }
        
        public bool Text1Enabled { get; set; }
        public string? Text1Name { get; set; }
        public string? Text1Description { get; set; }
        public bool Text1ShowInTable { get; set; }
        
        public bool Text2Enabled { get; set; }
        public string? Text2Name { get; set; }
        public string? Text2Description { get; set; }
        public bool Text2ShowInTable { get; set; }
        
        public bool Text3Enabled { get; set; }
        public string? Text3Name { get; set; }
        public string? Text3Description { get; set; }
        public bool Text3ShowInTable { get; set; }
        
        public bool Number1Enabled { get; set; }
        public string? Number1Name { get; set; }
        public string? Number1Description { get; set; }
        public bool Number1ShowInTable { get; set; }
        
        public bool Number2Enabled { get; set; }
        public string? Number2Name { get; set; }
        public string? Number2Description { get; set; }
        public bool Number2ShowInTable { get; set; }
        
        public bool Number3Enabled { get; set; }
        public string? Number3Name { get; set; }
        public string? Number3Description { get; set; }
        public bool Number3ShowInTable { get; set; }
        
        public bool Bool1Enabled { get; set; }
        public string? Bool1Name { get; set; }
        public string? Bool1Description { get; set; }
        public bool Bool1ShowInTable { get; set; }
        
        public bool Bool2Enabled { get; set; }
        public string? Bool2Name { get; set; }
        public string? Bool2Description { get; set; }
        public bool Bool2ShowInTable { get; set; }
        
        public bool Bool3Enabled { get; set; }
        public string? Bool3Name { get; set; }
        public string? Bool3Description { get; set; }
        public bool Bool3ShowInTable { get; set; }
        
        public bool DocumentLink1Enabled { get; set; }
        public string? DocumentLink1Name { get; set; }
        public string? DocumentLink1Description { get; set; }
        public bool DocumentLink1ShowInTable { get; set; }
        
        public bool DocumentLink2Enabled { get; set; }
        public string? DocumentLink2Name { get; set; }
        public string? DocumentLink2Description { get; set; }
        public bool DocumentLink2ShowInTable { get; set; }
        
        public bool DocumentLink3Enabled { get; set; }
        public string? DocumentLink3Name { get; set; }
        public string? DocumentLink3Description { get; set; }
        public bool DocumentLink3ShowInTable { get; set; }
        
        public virtual Category? Category { get; set; }
        public virtual User? Creator { get; set; }
        public virtual ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
        public virtual ICollection<InventoryAccess> AccessList { get; set; } = new List<InventoryAccess>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}