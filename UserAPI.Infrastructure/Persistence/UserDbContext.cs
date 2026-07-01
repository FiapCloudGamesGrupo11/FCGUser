using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserAPI.Domain.Entities;
using UserAPI.Domain.Interfaces;

namespace UserAPI.Infrastructure.Persistence
{
    public class UserDbContext : DbContext, IUserDbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
