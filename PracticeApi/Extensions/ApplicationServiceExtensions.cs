using Microsoft.AspNetCore.Http.Features;
using PracticeModel.Interface;
using PracticeModel.Interface.Repositories;
using PracticeModel.Interface.Services;
using PracticeRepository.Data;
using PracticeRepository.Repository;
using PracticeService;

namespace PracticeApi.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IUnitOfWork,UnitOfWork>()
                .AddScoped<IWeatherRepository,WeatherRepository>()
                .AddScoped<IErrorRepository,ErrorRepository>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services) {
            return services.AddScoped<IErrorService,ErrorService>()
                .AddScoped<IEmailService,EmailService>()
                .AddScoped<IUserService,UserService>()
                .AddScoped<IWeatherService,WeatherService>();
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services) { 
        
        List<string> cors = new List<string>();
            cors.Add("");
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(cors.ToArray()).AllowAnyHeader().AllowAnyMethod();
                });
            });
            return services;
             

        }
        public static IServiceCollection ConfigureForms(this IServiceCollection services) {
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            return services;
        }
            
    }
}
