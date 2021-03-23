using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bamboozed.Application.Entities;
using MimeKit;

namespace Bamboozed.Application.Interfaces
{
    public interface IRequestParser
    {
        Task<TimeOffRequest> ParseRequest(MimeMessage message);
    }
}
