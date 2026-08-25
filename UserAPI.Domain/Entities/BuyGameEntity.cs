namespace UserAPI.Domain.Entities
{
    public class BuyGameEntity
    {
        public BuyGameEntity(Guid userId, Guid gameId, decimal price, string paymentMethod, string? cardNumber, string? cvv, string? expirationDate)
        {
            UserId = userId;
            GameId = gameId;
            Price = price;
            PaymentMethod = paymentMethod;
            CardNumber = cardNumber;
            Cvv = cvv;
            ExpirationDate = expirationDate;
        }

        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal Price { get; set; }
        public string PaymentMethod { get; set; } // "CREDIT_CARD", "PIX", "BOLETO"
        public string? CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? ExpirationDate { get; set; }
        
    }
}