using System;
using System.Threading.Tasks;
using Bamboozed.Application.Entities;
using Bamboozed.Application.Interfaces;

namespace Bamboozed.Application.Services
{
    public class BambooService : IBambooService
    {
        private const string LoginSettingsKey = "BambooLogin";
        private const string PasswordSettingsKey = "BambooPassword";

        public Task ApproveTimeOff(TimeOffRequest timeOffRequest)
        {
            throw new NotImplementedException();
        }
    }
}
