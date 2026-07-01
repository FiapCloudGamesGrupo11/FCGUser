using UserAPI.Domain.ExternalModels;

namespace UserAPI.Domain.Interfaces
{
    public interface IGameCatalogClient
    {
        Task<IList<GameLibraryItem>> GetUserLibraryAsync(Guid userId, CancellationToken ct = default);
    }
}
