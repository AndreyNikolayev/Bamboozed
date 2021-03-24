using System.Threading.Tasks;

namespace Bamboozed.Application.Interfaces
{
    public interface IMailSenderService
    {
        Task SendRegistrationCode(string email, string code);
    }
}
