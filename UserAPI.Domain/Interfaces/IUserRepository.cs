using UserAPI.Domain.Entities;

namespace UserAPI.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Create(User usuario);
        Task<User> GetById(Guid id);
        Task<User> GetByEmail(string email);
        Task<IList<User>> GetAll();
        Task<User> GetByEmailPass(string email, string pass);
        Task<User> Update(User entidade);
    }
}
