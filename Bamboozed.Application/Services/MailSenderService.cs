using System;
using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace Bamboozed.Application.Services
{
    public class MailSenderService : IMailSenderService
    {
        private const string LoginSettingsKey = "EmailLogin";
        private const string PasswordSettingsKey = "EmailPassword";
        private const string SmtpHost = "smtp.gmail.com";
        private const int Port = 465;

        private readonly ISettingsService _settingsService;

        public MailSenderService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task SendRegistrationCode(string email, string code)
        {
            using var client = new SmtpClient();

            client.Connect(SmtpHost, Port, SecureSocketOptions.SslOnConnect);

            client.Authenticate(_settingsService.Get(LoginSettingsKey), _settingsService.Get(PasswordSettingsKey));

            var message = new MimeMessage
            {
                From = { MailboxAddress.Parse(_settingsService.Get(LoginSettingsKey)) },
                To = { MailboxAddress.Parse(email) },
                Subject = "Registration code for Bamboozed",
                Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = $"Your registration code: {code}" }
            };

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
