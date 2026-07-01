namespace UserAPI.Domain.ExternalModels
{
    public class GameLibraryItem
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
