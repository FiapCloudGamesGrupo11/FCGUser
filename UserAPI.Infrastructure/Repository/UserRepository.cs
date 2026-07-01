using Microsoft.EntityFrameworkCore;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDbContext _context;

        public UserRepository(IUserDbContext context)
        {
            _context = context;
        }

        public async Task<User> Create(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetById(Guid id)
            => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetByEmail(string email)
            => await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<IList<User>> GetAll()
            => await _context.Users.ToListAsync();

        public async Task<User> GetByEmailPass(string email, string pass)
            => await _context.Users.SingleOrDefaultAsync(p => p.Email == email && p.Password == pass);

        public async Task<User> Update(User entidade)
        {
            _context.Users.Update(entidade);
            await _context.SaveChangesAsync();
            return entidade;
        }
    }
}
