using UserAPI.Domain.ExternalModels;

namespace UserAPI.Domain.Interfaces
{
    public interface IGameCatalogClient
    {
        Task<IList<GameLibraryItem>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
        Task BuyGame(Guid userId, Guid gameId, decimal price, CancellationToken ct = default);
    }
}
