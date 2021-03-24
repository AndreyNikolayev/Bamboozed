using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Handlers;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Commands.Services;
using Bamboozed.Application.Context.Contexts;
using Bamboozed.Application.Context.Interfaces;
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
            services.AddScoped<IBambooService, BambooService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRequestParser, RequestParser>();
            services.AddScoped<ITimeOffService, TimeOffService>();
            services.AddScoped<ICryptoService, CryptoService>();
            services.AddScoped<IMailSenderService, MailSenderService>();

            services.AddScoped<IConversationReferenceContext, ConversationReferenceContext>();
            services.AddScoped<IReadonlyConversationReferenceContext, ReadonlyConversationReferenceContext>();

            services.AddScoped<ICommandParser, CommandParser>();
            services.AddScoped<ICommandBus, CommandBus>();

            services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            services.AddScoped<ICommandHandler<InputCodeCommand>, InputCodeCommandHandler>();
            services.AddScoped<ICommandHandler<InputPasswordCommand>, InputPasswordCommandHandler>();
        }
    }
}
