using System.Linq;
using Bamboozed.Application.Commands.Entities;
using Bamboozed.Application.Commands.Interfaces;
using System.Threading.Tasks;
using Bamboozed.Application.Context;
using Bamboozed.DAL.Repository;
using Bamboozed.Domain.User;

namespace Bamboozed.Application.Commands.Handlers
{
    public class InputCodeCommandHandler: ICommandHandler<InputCodeCommand>
    {
        private readonly ReadonlyConversationReferenceContext _conversationReferenceContext;
        private readonly IRepository<User> _userRepository;

        public InputCodeCommandHandler(
            ReadonlyConversationReferenceContext conversationReferenceContext,
            IRepository<User> userRepository)
        {
            _conversationReferenceContext = conversationReferenceContext;
            _userRepository = userRepository;
        }

        public async Task<CommandResult> Handle(InputCodeCommand command)
        {
            var userConversationId = _conversationReferenceContext.Context.User.Id;

            var users = await _userRepository.Get();

            var user = users.FirstOrDefault(p => p.ConversationId == userConversationId);

            if (user == null)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    Message = "Chat is not recognized. Please use 'register' command first."
                };
            }

            if (user.UserStatus == UserStatus.Active || user.UserStatus == UserStatus.RegistrationCodeSubmitted)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    Message = "Code is already submitted."
                };
            }

            if (user.RegistrationCode != command.Code)
            {
                return new CommandResult
                {
                    IsSuccess = false,
                    Message = "Wrong code."
                };
            }

            user.ChangeUserStatus(UserStatus.RegistrationCodeSubmitted);

            await _userRepository.Edit(user);

            return new CommandResult
            {
                IsSuccess = true,
                Message = "Code is verified. Please now submit your Bamboo password with 'password' command."
            };
        }
    }
}
