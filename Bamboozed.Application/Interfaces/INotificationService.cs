using System.Threading.Tasks;
using Bamboozed.Domain;

namespace Bamboozed.Application.Interfaces
{
    public interface INotificationService
    {
        Task Notify(string message);
        Task Notify(NotificationRequest request);
    }
}
