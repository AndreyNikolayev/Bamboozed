using System.Threading.Tasks;
using Bamboozed.Application.Entities;

namespace Bamboozed.Application.Interfaces
{
    public interface IBambooService
    {
        Task ApproveTimeOff(TimeOffRequest timeOffRequest);
    }
}
