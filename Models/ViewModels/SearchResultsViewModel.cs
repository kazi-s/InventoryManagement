namespace InventoryManagement.Models.ViewModels
{
    public class SearchResultsViewModel
    {
        public string? Query { get; set; }
        public string? Tag { get; set; }
        public List<InventorySearchResultViewModel> Inventories { get; set; } = new List<InventorySearchResultViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> PopularTags { get; set; } = new Dictionary<string, int>();
    }

    public class InventorySearchResultViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string?> Tags { get; set; } = new List<string?>();
    }
}