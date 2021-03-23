using System.Threading.Tasks;
using Bamboozed.Application.Entities;

namespace Bamboozed.Application.Interfaces
{
    public interface ISettingsService
    {
        string Get(string key);
    }
}
