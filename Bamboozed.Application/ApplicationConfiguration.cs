using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Commands.Services;
using Bamboozed.Application.Context;
using Bamboozed.Application.Events;
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

            services.AddScoped<ConversationReferenceContext>();
            services.AddScoped<ReadonlyConversationReferenceContext>();

            services.AddScoped<ICommandParser, CommandParser>();
            services.AddScoped<CommandBus>();
            services.AddScoped<DomainEventBus>();

            services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            services.AddScoped<ICommandHandler<InputCodeCommand>, InputCodeCommandHandler>();
            services.AddScoped<ICommandHandler<InputPasswordCommand>, InputPasswordCommandHandler>();
        }
    }
}
