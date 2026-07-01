using business_logic.Interfaces;
using business_logic.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace business_logic
{
    public static class ServiceExtensions
    {
        public static void AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(ApplicationProfile).Assembly);
            });

            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            services.AddCustomServices();
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IStepService, StepService>();
        }
    }
}
