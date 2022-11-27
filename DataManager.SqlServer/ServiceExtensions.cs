using DataManager.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataManager.SqlServer
{
    public static class ServiceExtensions
    {
        public static void AddDataManagerSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("DataManager.Common"))
            );

            services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();
        }
    }
}
