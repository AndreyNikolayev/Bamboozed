using CSharpFunctionalExtensions;

namespace Bamboozed.Domain.User
{
    public sealed class User: Entity<string>
    {
        public string Email { get; private set; }
        public string ConversationId { get; private set; }
        public string ConversationReferenceJson { get; private set; }
        public UserStatus UserStatus { get; private set; }
        public string RegistrationCode { get; private set; }

        public User() { }

        public User(string email, string conversationId, string conversationReferenceJson, string registrationCode): base(email)
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

        public Result SubmitRegistrationCode(string code)
        {
            if (UserStatus != UserStatus.RegistrationCodeSent)
            {
                return Result.Failure("Code is already submitted.");
            }

            if (code != RegistrationCode)
            {
                return Result.Failure("Wrong code.");
            }

            UserStatus = UserStatus.RegistrationCodeSubmitted;

            return Result.Success();
        }

        public Result Activate()
        {
            if (UserStatus != UserStatus.RegistrationCodeSubmitted)
            {
                return Result.Failure("Registration code is sent to your mailbox. Please submit it with 'code' command before submitting the password.");
            }

            UserStatus = UserStatus.Active;

            return Result.Success();
        }
    }

    public enum UserStatus
    {
        RegistrationCodeSent,
        RegistrationCodeSubmitted,
        Active
    }
}
