using System.Threading.Tasks;

namespace Bamboozed.Application.Interfaces
{
    public interface IPasswordService
    {
        Task<string> Get(string key);
        Task Set(string key, string value);
    }
}
