using System.Threading.Tasks;

namespace Bamboozed.Application.Interfaces
{
    public interface INotificationService
    {
        Task Notify(string message);
    }
}
