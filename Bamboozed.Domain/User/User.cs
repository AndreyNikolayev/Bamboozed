using System;
using Bamboozed.Domain.Extensions;
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

        public Result ChangeUserStatus(UserStatus status)
        {
            if (UserStatus == status)
            {
                throw new ArgumentException($"User status is already set to {status.GetDescription()}");
            }

            if (status == UserStatus.RegistrationCodeSent)
            {
                return Result.Failure($"Status cannot be changed back to {UserStatus.RegistrationCodeSent.GetDescription()}");
            }

            if (status == UserStatus.Active)
            {
                return Result.Failure($"Status cannot be changed from {UserStatus.Active.GetDescription()}");
            }

            if (UserStatus == UserStatus.RegistrationCodeSent && status == UserStatus.Active)
            {
                return Result.Failure($"Status cannot be changed to {UserStatus.Active.GetDescription()} from {UserStatus.RegistrationCodeSent.GetDescription()}");
            }

            UserStatus = status;
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
