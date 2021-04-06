using System.Linq;
using System.Threading.Tasks;
using Bamboozed.Application.Commands.Interfaces;
using Bamboozed.Application.Events;
using Bamboozed.Application.Events.Events;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.Extensions;
using CommandLine;
using CSharpFunctionalExtensions;

namespace Bamboozed.Application.Commands.Entities.UserCommand
{
    [Verb("code", HelpText = "Insert code from the registration email (ex. code 123456)")]
    public class InputCodeCommand: ICommand
    {
        [Value(0, Required = true, HelpText = "Code from registration email")]
        public string Code { get; set; }
        public string ConversationId { get; set; }
    }

    public class InputCodeCommandHandler : ICommandHandler<InputCodeCommand>
    {
        private readonly IRepository<Domain.User.User> _userRepository;
        private readonly DomainEventBus _domainEventBus;

        public InputCodeCommandHandler(
            IRepository<Domain.User.User> userRepository,
            DomainEventBus domainEventBus)
        {
            _userRepository = userRepository;
            _domainEventBus = domainEventBus;
        }

        public async Task<Result> Handle(InputCodeCommand command)
        {
            return await _userRepository.Get()
                .ToResultTask()
                .Map(users => users
                    .FirstOrDefault(p => p.ConversationId == command.ConversationId)
                )
                .Ensure(user => user != null, "Chat is not recognized. Please use 'register' command first.")
                .Check(user => user.SubmitRegistrationCode(command.Code))
                .Tap(user => _userRepository.Edit(user))
                .Tap(user => _domainEventBus.Dispatch(new RegistrationCodeEnteredEvent(user.Email)))
                .OnFailure(error => _domainEventBus.Dispatch(new RegistrationStepFailedEvent(command.ConversationId, error)))
                .Bind(_ => Result.Success());
        }
    }
}
