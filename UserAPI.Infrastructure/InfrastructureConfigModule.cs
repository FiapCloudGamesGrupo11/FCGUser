using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddHttpContextAccessor();
            services.AddTransient<AuthorizationDelegatingHandler>();

            services.AddHttpClient<IGameCatalogClient, GameCatalogClient>(c =>
                c.BaseAddress = new Uri(configuration["CatalogApi:BaseUrl"]!)).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

            services.AddSingleton<IAmazonSQS>(_ =>
            {
                var region = configuration["AWS:Region"] ?? "us-east-1";
                var serviceUrl = configuration["AWS:ServiceUrl"];

                if (!string.IsNullOrWhiteSpace(serviceUrl))
                {
                    var localConfig = new AmazonSQSConfig
                    {
                        ServiceURL = serviceUrl,
                        AuthenticationRegion = region
                    };

                    return new AmazonSQSClient(
                        new BasicAWSCredentials("test", "test"),
                        localConfig);
                }

                return new AmazonSQSClient(RegionEndpoint.GetBySystemName(region));
            });

            services.AddSingleton<IEventPublisher, SqsEventPublisher>();

            return services;
        }
    }
}
