using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.Application.Interfaces;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using Bamboozed.Domain.User;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities
{
    [Verb("password", HelpText = "Add or update bamboo password")]
    public class InputPasswordCommand : ICommand
    {
        [Value(0, Required = true, HelpText = "Your Bamboo password")]
        public string Password { get; set; }
        public string ConversationId { get; set; }
    }

    public class InputPasswordCommandHandler : ICommandHandler<InputPasswordCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly DomainEventBus _domainEventBus;

        public InputPasswordCommandHandler(
            IRepository<User> userRepository,
            IPasswordService passwordService,
            DomainEventBus domainEventBus)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _domainEventBus = domainEventBus;
        }

        public async Task<Result> Handle(InputPasswordCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users
                    .FirstOrDefault(p => p.ConversationId == command.ConversationId)
                )
                .Ensure(user => user != null, "Chat is not recognized. Please use 'register' command first.")
                .CheckIf(user => user.UserStatus != UserStatus.Active, user => user.Activate())
                .Tap(user => _passwordService.Set(user.Email, command.Password))
                .Tap(user => _userRepository.Edit(user))
                .Tap(user => _domainEventBus.Dispatch(new PasswordSubmittedEvent(user.Email)))
                .OnFailure(error => _domainEventBus.Dispatch(new RegistrationStepFailedEvent(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }
    }
}
