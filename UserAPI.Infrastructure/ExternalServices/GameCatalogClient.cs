using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UserAPI.Domain.Entities;
using UserAPI.Domain.ExternalModels;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.ExternalServices
{
    public class GameCatalogClient : IGameCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GameCatalogClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GameCatalogClient(HttpClient httpClient, ILogger<GameCatalogClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IList<GameLibraryItem>> GetUserGames(Guid userId, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"/api/UsersGames/GetGamesByUserId?userId={userId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<GameLibraryItem>();

            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<IList<GameLibraryItem>>(JsonOptions, ct);
            return items ?? new List<GameLibraryItem>();
        }

        public async Task<IList<CatalogGameItem>> GetAllGames(CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync("/api/Game/GetAll", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<CatalogGameItem>();

            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<IList<CatalogGameItem>>(JsonOptions, ct);
            return items ?? new List<CatalogGameItem>();
        }

        public async Task BuyGame(BuyGameEntity requestEntity, CancellationToken ct = default)
        {
            var request = new PurchaseGameRequest
            {
                UserId = requestEntity.UserId,
                GameId = requestEntity.GameId,
                ValuePay = requestEntity.Price,
                PaymentMethod = requestEntity.PaymentMethod,
                CardNumber = requestEntity.CardNumber,
                Cvv = requestEntity.Cvv,
                ExpirationDate = requestEntity.ExpirationDate
            };

            _logger.LogInformation(
                "Iniciando compra de jogo. UserId: {UserId}, GameId: {GameId}, Price: {Price}",
                requestEntity.UserId, requestEntity.GameId, requestEntity.Price);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/UsersGames/Post", request, cancellationToken: ct);

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    _logger.LogWarning("Jogo não encontrado. GameId: {GameId}", requestEntity.GameId);
                    throw new InvalidOperationException($"Game with ID {requestEntity.GameId} not found in catalog");
                }

                _logger.LogInformation("Compra realizada com sucesso. Aguarde a confirmação. UserId: {UserId}, GameId: {GameId}", requestEntity.UserId, requestEntity.GameId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao realizar compra. UserId: {UserId}, GameId: {GameId}", requestEntity.UserId, requestEntity.GameId);
                throw;
            }
        }
    }
}
