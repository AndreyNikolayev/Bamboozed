using System;
using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.Application.Services;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("register", HelpText = "register email")]
    public class RegisterCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Your Bamboo Email")]
        public string Email { get; set; }
        public string ConversationId { get; set; }
    }

    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly MailSenderService _mailSenderService;
        private readonly DomainEventBus _domainEventBus;

        public RegisterCommandHandler(
            IRepository<User> userRepository,
            MailSenderService mailSenderService,
            DomainEventBus domainEventBus)
        {
            _userRepository = userRepository;
            _mailSenderService = mailSenderService;
            _domainEventBus = domainEventBus;
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
                    if (user.ConversationId != command.ConversationId)
                    {
                        return Result.Failure(
                            "User is already associated with another chat. Contact admin in order to change chat.");
                    }

                    return user.UserStatus switch
                    {
                        UserStatus.RegistrationCodeSent => Result.Failure(
                            "Registration code is already sent to your mailbox. Please submit it with 'code' command."),
                        UserStatus.RegistrationCodeSubmitted => Result.Failure(
                            "Registration code is already submitted. Please submit BambooHR password it with 'password' command."),
                        UserStatus.Active => Result.Failure("User is already registered."),
                        _ => throw new ArgumentException("Invalid user status")
                    };
                })
                .Ensure(users => users
                    .All(p => p.ConversationId != command.ConversationId || p.Email == command.Email),
                    "Chat is already used for another email. Please contact admin."
                )
                .Map(_ => new Random().Next(0, 1000000).ToString("D6"))
                .Tap(registrationCode => _mailSenderService.SendRegistrationCode(command.Email, registrationCode))
                .Tap(registrationCode => _userRepository.Add(new User(command.Email, command.ConversationId, registrationCode)))
                .Tap(user => _domainEventBus.Dispatch(new UserCreatedEvent(command.Email)))
                .OnFailure(error => _domainEventBus.Dispatch(new RegistrationStepFailedEvent(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }
    }
}
