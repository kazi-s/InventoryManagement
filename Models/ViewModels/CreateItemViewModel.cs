using System.ComponentModel.DataAnnotations;
using InventoryManagement.Models.Entities;

namespace InventoryManagement.Models.ViewModels
{
    public class CreateItemViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;
        
        [Display(Name = "Custom ID")]
        public string CustomId { get; set; } = string.Empty;
        
        [Display(Name = "String Field 1")]
        public string? StringValue1 { get; set; }
        
        [Display(Name = "String Field 2")]
        public string? StringValue2 { get; set; }
        
        [Display(Name = "String Field 3")]
        public string? StringValue3 { get; set; }
        
        // Text values (multi-line)
        [Display(Name = "Text Field 1")]
        public string? TextValue1 { get; set; }
        
        [Display(Name = "Text Field 2")]
        public string? TextValue2 { get; set; }
        
        [Display(Name = "Text Field 3")]
        public string? TextValue3 { get; set; }
        
        [Display(Name = "Number Field 1")]
        public decimal? NumberValue1 { get; set; }
        
        [Display(Name = "Number Field 2")]
        public decimal? NumberValue2 { get; set; }
        
        [Display(Name = "Number Field 3")]
        public decimal? NumberValue3 { get; set; }
        
        [Display(Name = "Boolean Field 1")]
        public bool BoolValue1 { get; set; }
        
        [Display(Name = "Boolean Field 2")]
        public bool BoolValue2 { get; set; }
        
        [Display(Name = "Boolean Field 3")]
        public bool BoolValue3 { get; set; }
        
        [Display(Name = "Document Link 1")]
        [Url]
        public string? DocumentLink1 { get; set; }
        
        [Display(Name = "Document Link 2")]
        [Url]
        public string? DocumentLink2 { get; set; }
        
        [Display(Name = "Document Link 3")]
        [Url]
        public string? DocumentLink3 { get; set; }
        
        public Inventory? Inventory { get; set; }
    }

    public class EditItemViewModel : CreateItemViewModel
    {
        public int Id { get; set; }
    }
}