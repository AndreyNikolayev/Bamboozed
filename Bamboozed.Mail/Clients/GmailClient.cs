using System;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace Bamboozed.Mail.Clients
{
    public class GmailClient: IDisposable
    {
        private readonly ImapClient _imapClient;

        private const string ImapHost = "imap.gmail.com";
        private const int Port = 993;

        private GmailClient(string login, string password)
        {
            _imapClient = new ImapClient();
            _imapClient.Authenticate(login, password);
        }

        public async Task<MimeMessage> GetUnseenTimeOffRequests(string topicName = "INBOX")
        {
            var folder = await _imapClient.GetFolderAsync(topicName);
            await folder.OpenAsync(FolderAccess.ReadWrite);

            return await folder.SearchAsync(SearchQuery.NotSeen);
        }

        public void Dispose()
        {
            _imapClient?.Dispose();
        }
    }
}
