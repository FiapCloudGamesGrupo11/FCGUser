using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using UserAPI.Domain.Interfaces;
using UserAPI.Infrastructure.Authorization;
using UserAPI.Infrastructure.ExternalServices;
using UserAPI.Infrastructure.Messaging;
using UserAPI.Infrastructure.Persistence;
using UserAPI.Infrastructure.Repository;

namespace UserAPI.Infrastructure
{
    public static class InfrastructureConfigModule
    {
        public static IServiceCollection AddConfigInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionString");
            services.AddDbContext<UserDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUserDbContext, UserDbContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthHelpers, AuthHelpers>();

            services.AddHttpClient<IGameCatalogClient, GameCatalogClient>(c =>
                c.BaseAddress = new Uri(configuration["CatalogApi:BaseUrl"]!));

            var rabbit  = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = rabbit["Host"]!,
                Port     = int.Parse(rabbit["Port"]!),
                UserName = rabbit["Username"]!,
                Password = rabbit["Password"]!
            };
            var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            services.AddSingleton(connection);
            services.AddSingleton<IEventPublisher, RabbitMqPublisher>();

            return services;
        }
    }
}
