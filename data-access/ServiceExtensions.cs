using Core.Entities;
using Core.Interfaces;
using data_access.data;
using data_access.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace data_access
{
    public static class ServiceExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext(connectionString);
            services.AddRepositories();
        }
        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MyTaskDbContext>(opts => opts.UseSqlServer(connectionString));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<MyTaskDbContext>();
        }
    }
}
