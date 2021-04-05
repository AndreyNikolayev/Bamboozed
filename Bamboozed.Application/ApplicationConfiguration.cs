using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Commands.Services;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.Application.Events.Interfaces;
using Bamboozed.Application.Interfaces;
using Bamboozed.Application.Services;
using Bamboozed.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace Bamboozed.Application
{
    public static class ApplicationConfiguration
    {
        public static void Setup(IServiceCollection services)
        {
            services.UseDal();

            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<BambooService>();
            services.AddScoped<NotificationService>();
            services.AddSingleton<RequestParser>();
            services.AddScoped<TimeOffService>();
            services.AddScoped<CryptoService>();
            services.AddScoped<MailSenderService>();

            services.AddScoped<CommandParser>();
            services.AddScoped<CommandBus>();

            services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            services.AddScoped<ICommandHandler<InputCodeCommand>, InputCodeCommandHandler>();
            services.AddScoped<ICommandHandler<InputPasswordCommand>, InputPasswordCommandHandler>();

            services.AddScoped<DomainEventBus>();

            services.AddScoped<IEventHandler<PasswordSubmittedEvent>, PasswordSubmittedEventHandler>();
            services.AddScoped<IEventHandler<RegistrationCodeEnteredEvent>, RegistrationCodeEnteredEventHandler>();
            services.AddScoped<IEventHandler<RegistrationStepFailedEvent>, RegistrationStepFailedEventHandler>();
            services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        }
    }
}
