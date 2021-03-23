using System;
using System.Threading.Tasks;
using Bamboozed.Application.Entities;
using Bamboozed.Application.Interfaces;
using MimeKit;

namespace Bamboozed.Application.Services
{
    public class RequestParser: IRequestParser
    {
        public Task<TimeOffRequest> ParseRequest(MimeMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
