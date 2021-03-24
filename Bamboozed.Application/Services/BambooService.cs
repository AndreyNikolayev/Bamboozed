using System;
using System.Net;
using System.Threading.Tasks;
using Bamboozed.Application.Entities;
using Bamboozed.Application.Interfaces;
using RestSharp;

namespace Bamboozed.Application.Services
{
    public class BambooService : IBambooService
    {
        private const string LoginSettingsKey = "BambooLogin";
        private const string PasswordSettingsKey = "BambooPassword";

        private readonly ISettingsService _settingsService;

        public BambooService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task ApproveTimeOff(TimeOffRequest timeOffRequest)
        {
            var client = new RestClient
            {
                CookieContainer = new CookieContainer()
            };

            if (!await Login(client))
            {
                throw new Exception("Unauthorized to Bamboo");
            };

            var request = new RestRequest(timeOffRequest.ApproveLink, Method.GET);
            await client.ExecuteAsync(request);
        }


        private async Task<bool> Login(IRestClient client)
        {
            var request = new RestRequest("https://trinetix.bamboohr.com/login.php", Method.POST);
            request.AddHeader("origin", "https://trinetix.bamboohr.com");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("tz", "Europe/Helsinki");
            request.AddParameter("username", _settingsService.Get(LoginSettingsKey));
            request.AddParameter("password", _settingsService.Get(PasswordSettingsKey));
            request.AddParameter("login", "Log in");
            await client.ExecuteAsync(request);

            return client.CookieContainer.Count >= 9;
        }
    }
}
