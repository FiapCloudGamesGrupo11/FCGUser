using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using UserAPI.Domain.ExternalModels;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.ExternalServices
{
    public class GameCatalogClient : IGameCatalogClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GameCatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IList<GameLibraryItem>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default)
        {
            var response = await _httpClient.GetAsync($"/api/Library?userId={userId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new List<GameLibraryItem>();

            response.EnsureSuccessStatusCode();

            var items = await response.Content.ReadFromJsonAsync<IList<GameLibraryItem>>(JsonOptions, ct);
            return items ?? new List<GameLibraryItem>();
        }
    }
}
