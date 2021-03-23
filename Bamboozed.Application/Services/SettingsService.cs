using System;
using Bamboozed.Application.Interfaces;

namespace Bamboozed.Application.Services
{
    public class SettingsService : ISettingsService
    {
        public string Get(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }
    }
}
