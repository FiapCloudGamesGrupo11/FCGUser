namespace UserAPI.Application.DTOs.Request
{
    public class BuyGameRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
    }
}
