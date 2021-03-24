using Bamboozed.Application.Entities;
using MimeKit;

namespace Bamboozed.Application.Interfaces
{
    public interface IRequestParser
    {
        TimeOffRequest ParseRequest(MimeMessage message);
    }
}
