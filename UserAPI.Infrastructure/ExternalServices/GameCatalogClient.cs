using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
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

        public async Task BuyGame(Guid userId, Guid gameId, decimal price, CancellationToken ct = default)
        {
            var request = new PurchaseGameRequest
            {
                UserId = userId,
                GameId = gameId,
                ValuePay = price
            };

            _logger.LogInformation(
                "Iniciando compra de jogo. UserId: {UserId}, GameId: {GameId}, Price: {Price}",
                userId, gameId, price);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/UsersGames/Post", request, cancellationToken: ct);

                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    _logger.LogWarning("Jogo não encontrado. GameId: {GameId}", gameId);
                    throw new InvalidOperationException($"Game with ID {gameId} not found in catalog");
                }

                _logger.LogInformation("Compra realizada com sucesso. Aguarde a confirmação. UserId: {UserId}, GameId: {GameId}", userId, gameId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro ao realizar compra. UserId: {UserId}, GameId: {GameId}", userId, gameId);
                throw;
            }
        }
    }
}
