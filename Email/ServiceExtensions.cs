using Email.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Email
{
    public static class ServiceExtensions
    {
        public static void AddDataManagerSqlServer(this IServiceCollection services,
            string defaultFrom,
            string defaultPassword,
            string host,
            int port
        )
        {
            services.AddScoped<IEmailSender, EmailSender>(x => new EmailSender(defaultFrom, defaultPassword, host, port));
        }
    }
}
