using Bamboozed.DAL.Enums;
using Microsoft.WindowsAzure.Storage.Table;

namespace Bamboozed.DAL.Entities
{
    public class User: TableEntity
    {
        public string Email => RowKey;
        public string ConversationId { get; set; }
        public string ConversationReferenceJson { get; set; }
        public int UserStatusId { get; set; }
        public string RegistrationCode { get; set; }
        public UserStatus UserStatus
        {
            get => (UserStatus)UserStatusId;
            set => UserStatusId = (int)value;
        }

        public User() { }

        public User(string email, string conversationId, string conversationReferenceJson, string registrationCode)
            : base("Bamboozed", email)
        {
            ConversationId = conversationId;
            ConversationReferenceJson = conversationReferenceJson;
            UserStatus = UserStatus.RegistrationCodeSent;
            RegistrationCode = registrationCode;
        }
    }
}
