using Email.Abstractions;
using Email.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Email
{
    public static class ServiceExtensions
    {
        public static void AddEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailConfig>(configuration.GetSection("Email"));

            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}
