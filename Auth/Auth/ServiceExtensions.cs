using Microsoft.Extensions.DependencyInjection;
using Auth.Abstractions;
using Auth.Services;
using Auth.DTOs;
using Microsoft.Extensions.Configuration;

namespace Auth
{
    public static class ServiceExtensions
    {
        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenConfig>(configuration.GetSection("JWT"));

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}
