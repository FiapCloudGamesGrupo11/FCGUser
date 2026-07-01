using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserAPI.Application.Behaviors;
using UserAPI.Application.Interfaces;
using UserAPI.Application.Services;
using UserAPI.Application.Validators;

namespace UserAPI.Application
{
    public static class ApplicationConfigModule
    {
        public static IServiceCollection AddConfigServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped(typeof(IValidationBehavior<>), typeof(ValidationBehavior<>));
            services.AddValidatorsFromAssembly(typeof(UserValidator).Assembly);

            return services;
        }
    }
}
