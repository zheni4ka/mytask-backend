using business_logic;
using data_access;
using Hangfire;
using Microsoft.OpenApi;
using mytask_backend.Helpers;
using Scalar.AspNetCore;
using mytask_backend.Middlewares;


namespace mytask_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string? connectionString = builder.Configuration.GetConnectionString("connectionString");

            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddBusinessLogicServices();
            builder.Services.AddDataAccessServices(connectionString);
            builder.Services.AddOpenApi();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCustomServices();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200") 
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials(); 
                    });
            });

            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();


            app.UseCors("AllowAngularDev");
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
