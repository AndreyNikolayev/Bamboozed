using System;
using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Context;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("register", HelpText = "register email")]
    public class RegisterCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Your Bamboo Email")]
        public string Email { get; set; }
    }

    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly ReadonlyConversationReferenceContext _conversationReferenceContext;
        private readonly IRepository<User> _userRepository;
        private readonly MailSenderService _mailSenderService;

        public RegisterCommandHandler(ReadonlyConversationReferenceContext conversationReferenceContext,
            IRepository<User> userRepository,
            MailSenderService mailSenderService)
        {
            _conversationReferenceContext = conversationReferenceContext;
            _userRepository = userRepository;
            _mailSenderService = mailSenderService;
        }

        public async Task<Result> Handle(RegisterCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Check(users =>
                {
                    var user = users.FirstOrDefault(p => p.Email == command.Email);
                    if (user == null)
                    {
                        return Result.Success();
                    }
                    if (user.ConversationId != _conversationReferenceContext.Context.User.Id)
                    {
                        return Result.Failure(
                            "User is already associated with another chat. Contact admin in order to change chat.");
                    }

                    switch (user.UserStatus)
                    {
                        case UserStatus.RegistrationCodeSent:
                            return Result.Failure(
                               "Registration code is already sent to your mailbox. Please submit it with 'code' command.");
                        case UserStatus.RegistrationCodeSubmitted:
                            return Result.Failure(
                                "Registration code is already submitted. Please submit BambooHR password it with 'password' command.");
                        case UserStatus.Active:
                            return Result.Failure("User is already registered.");
                        default:
                            throw new ArgumentException("Invalid user status");
                    }

                })
                .Ensure(users => users
                    .All(p => p.ConversationId != _conversationReferenceContext.Context.User.Id || p.Email == command.Email),
                    "Chat is already used for another email. Please contact admin."
                )
                .Map(_ => new Random().Next(0, 1000000).ToString("D6"))
                .Tap(registrationCode => _mailSenderService.SendRegistrationCode(command.Email, registrationCode))
                .Tap(registrationCode => _userRepository.Add(new User(command.Email, _conversationReferenceContext.Context.User.Id,
                    JsonConvert.SerializeObject(_conversationReferenceContext.Context), registrationCode)))
                .Tap(user => DomainEvents.Dispatch(new UserCreatedEvent(command.Email)))
                .OnFailure(error => DomainEvents.Dispatch(new RegistrationStepFailedEvent(_conversationReferenceContext.Context, error)))
                .Bind(_ => Result.Success());
        }
    }
}
