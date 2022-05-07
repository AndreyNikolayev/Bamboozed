using System;
using System.Net;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using Bamboozed.Domain.TimeOffRequest;
using RestSharp;

namespace Bamboozed.Application.Services
{
    public class BambooService
    {
        private readonly IPasswordService _passwordService;

        public BambooService(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public async Task ApproveTimeOff(TimeOffRequest timeOffRequest)
        {
            var client = new RestClient
            {
                CookieContainer = new CookieContainer()
            };

            if (!await Login(client, timeOffRequest.ApproverEmail))
            {
                throw new Exception("Unauthorized to Bamboo");
            };

            var request = new RestRequest(timeOffRequest.ApproveLink, Method.GET);
            await client.ExecuteAsync(request);
        }


        private async Task<bool> Login(IRestClient client, string approverEmail)
        {
            var password = await _passwordService.Get(approverEmail);

            var request = new RestRequest("https://trinetix.bamboohr.com/login.php", Method.POST);
            request.AddHeader("origin", "https://trinetix.bamboohr.com");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("tz", "Europe/Helsinki");
            request.AddParameter("username", approverEmail);
            request.AddParameter("password", password);
            request.AddParameter("login", "Log in");
            var response = await client.ExecuteAsync(request);

            return client.CookieContainer.Count >= 3;
        }
    }
}
