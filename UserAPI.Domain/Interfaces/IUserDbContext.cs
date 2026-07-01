using Microsoft.EntityFrameworkCore;
using UserAPI.Domain.Entities;

namespace UserAPI.Domain.Interfaces
{
    public interface IUserDbContext
    {
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
