using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using Bamboozed.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bamboozed.Application.Services
{
    public class NotificationService
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string NotificationEndpointKey = "NotificationEndpoint";
        private readonly ISettingsService _settingsService;

        public NotificationService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task Notify(string message)
        {
            var endpoint = _settingsService.Get(NotificationEndpointKey);

            using var response = await HttpClient.PostAsync(endpoint,
                new StringContent("\"" + message + "\"", Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status:{response.StatusCode} Response:{body}");
            }
        }

        public async Task Notify(NotificationRequest request)
        {
            var endpoint = _settingsService.Get(NotificationEndpointKey);

            var requestBody = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            using var response = await HttpClient.PostAsync(endpoint,
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status:{response.StatusCode} Response:{body}");
            }
        }
    }
}
