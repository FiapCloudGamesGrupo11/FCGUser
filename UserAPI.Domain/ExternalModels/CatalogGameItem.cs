namespace UserAPI.Domain.ExternalModels
{
    public class CatalogGameItem
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
