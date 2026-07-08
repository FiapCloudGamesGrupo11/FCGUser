using UserAPI.Application.DTOs.Request;
using UserAPI.Application.DTOs.Response;
using UserAPI.Domain.Enums;
using UserAPI.Domain.ExternalModels;

namespace UserAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserCreatedResponseView> CreateUser(UserRequestView userRequest);
        Task<UserCreatedResponseView> GetUserById(Guid id);
        Task<IList<UserCreatedResponseView>> GetAll();
        Task<LoginResponseView> Login(LoginRequestView request);
        Task<UserCreatedResponseView> CreateAdmin(UserRequestView request);
        Task<UserCreatedResponseView> UpdateRole(Guid id, Role requestRole);
        Task<UserCreatedResponseView> UpdateStatus(Guid id, Status requestStatus);
        Task<UserCreatedResponseView> UpdateUser(Guid id, UserRequestUpdateView userRequest);
        Task<IList<GameLibraryItem>> GetUserGames(Guid userId, CancellationToken ct = default);
        Task BuyGame(Guid userId, Guid gameId, decimal price, CancellationToken ct = default);
    }
}
