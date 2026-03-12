using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class IdPart
    {
        public string Type { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Format { get; set; }
        public int Order { get; set; }
    }
}

namespace InventoryManagement.Models.ViewModels
{
    public class IdFormatViewModel
    {
        public int InventoryId { get; set; }
        public string InventoryTitle { get; set; } = string.Empty;
        public List<IdPart> Parts { get; set; } = new List<IdPart>();
        public string Preview { get; set; } = string.Empty;
    }
}