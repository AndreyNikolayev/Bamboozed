using Bamboozed.Domain.Base;

namespace Bamboozed.Domain.User
{
    public sealed class User: Entity
    {
        public string Email { get; private set; }
        public string ConversationId { get; private set; }
        public string ConversationReferenceJson { get; private set; }
        public UserStatus UserStatus { get; private set; }
        public string RegistrationCode { get; private set; }

        public User() { }

        public User(string email, string conversationId, string conversationReferenceJson, string registrationCode)
        {
            Email = email;
            ConversationId = conversationId;
            ConversationReferenceJson = conversationReferenceJson;
            UserStatus = UserStatus.RegistrationCodeSent;
            RegistrationCode = registrationCode;
        }
        /// <summary>
        /// Email of the user
        /// </summary>
        public override string Id => Email;

        public void ChangeUserStatus(UserStatus status)
        {
            UserStatus = status;
        }
    }

    public enum UserStatus
    {
        None,
        RegistrationCodeSent,
        RegistrationCodeSubmitted,
        Active
    }
}
