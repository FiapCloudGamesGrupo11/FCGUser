using Microsoft.EntityFrameworkCore;
using UserAPI.Infrastructure.Persistence;

namespace UserAPI.API.Extensions
{
    public static class MigrationExtensions
    {
        public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserDbContext>>();

            const int maxAttempts = 10;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    db.Database.Migrate();
                    logger.LogInformation("Migrations aplicadas com sucesso (tentativa {Attempt}).", attempt);
                    return app;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, "Falha ao aplicar migrations (tentativa {Attempt}/{Max}). Aguardando 5s...", attempt, maxAttempts);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }

            return app;
        }
    }
}
