using Bamboozed.Application.Interfaces;
using Bamboozed.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bamboozed.Application
{
    public static class ApplicationConfiguration
    {
        public static void Setup(IServiceCollection services)
        {
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IBambooService, BambooService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRequestParser, RequestParser>();
            services.AddScoped<ITimeOffService, TimeOffService>();
        }
    }
}
