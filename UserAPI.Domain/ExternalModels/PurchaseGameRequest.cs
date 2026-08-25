namespace UserAPI.Domain.ExternalModels
{
    public class PurchaseGameRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public decimal ValuePay { get; set; }
        public string PaymentMethod { get; set; } // "CREDIT_CARD", "PIX", "BOLETO"
        public string? CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? ExpirationDate { get; set; }
    }
}