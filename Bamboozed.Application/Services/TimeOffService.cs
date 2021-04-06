using System;
using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.NotificationRequest;
using Bamboozed.Domain.User;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;

namespace Bamboozed.Application.Services
{
    public class TimeOffService
    {
        private const string TimeOffLabel = "Bamboo-TimeOff";
        private const string LoginSettingsKey = "EmailLogin";
        private const string PasswordSettingsKey = "EmailPassword";
        private const string ImapHost = "imap.gmail.com";
        private const int Port = 993;

        private readonly RequestParser _requestParser;
        private readonly BambooService _bambooService;
        private readonly NotificationService _notificationService;
        private readonly ISettingsService _settingsService;
        private readonly IRepository<User> _userRepository;

        public TimeOffService(RequestParser requestParser,
            BambooService bambooService,
            NotificationService notificationService,
            ISettingsService settingsService,
            IRepository<User> userRepository)
        {
            _requestParser = requestParser;
            _bambooService = bambooService;
            _notificationService = notificationService;
            _settingsService = settingsService;
            _userRepository = userRepository;
        }

        public async Task Handle()
        {
            await WithImapClient(async client =>
            {
                var folder = await client.GetFolderAsync(TimeOffLabel);
                await folder.OpenAsync(FolderAccess.ReadWrite);
                var messageIds = await folder.SearchAsync(SearchQuery.NotSeen);

                await Task.WhenAll(messageIds.Select(p => HandleSingleRequest(folder, p)));
            });
        }

        private async Task HandleSingleRequest(IMailFolder folder, UniqueId messageId)
        {
            var message = await folder.GetMessageAsync(messageId);

            var request = _requestParser.ParseRequest(message);

            var user = await _userRepository.GetById(request.ApproverEmail);

            await _bambooService.ApproveTimeOff(request);
            await folder.AddFlagsAsync(messageId, MessageFlags.Seen, false);
            await _notificationService.Notify(new NotificationRequest(
                user.Value.ConversationId,
                request.ApprovedMessage));
        }

        private async Task WithImapClient(Func<ImapClient, Task> action)
        {
            var client = new ImapClient();

            var login = _settingsService.Get(LoginSettingsKey);
            var password = _settingsService.Get(PasswordSettingsKey);

            await client.ConnectAsync(ImapHost, Port, true);
            await client.AuthenticateAsync(login, password);

            try
            {
                await action(client);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
