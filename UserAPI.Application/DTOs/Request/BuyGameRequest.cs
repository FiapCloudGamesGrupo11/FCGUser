namespace UserAPI.Application.DTOs.Request
{
    public class BuyGameRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
        public string PaymentMethod { get; set; } // "CREDIT_CARD", "PIX", "BOLETO"
        public string? CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? ExpirationDate { get; set; }
    }
}
