using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;

namespace Bamboozed.Application.Services
{
    public class NotificationService : INotificationService
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
                new StringContent(message, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new Exception($"Status:{response.StatusCode} Response:{body}");
            }
        }

    }
}
