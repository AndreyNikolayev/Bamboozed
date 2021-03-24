using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bamboozed.Application.Interfaces;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace Bamboozed.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private const string KeyVaultUrlKey = "KeyVaultUrl";

        private readonly ISettingsService _settingsService;

        public PasswordService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<string> Get(string email)
        {
            var vaultBaseUrl = _settingsService.Get(KeyVaultUrlKey);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var vaultResponse = await kv.GetSecretAsync(vaultBaseUrl, CryptoService.Encrypt(email));

            return vaultResponse.Value;
        }

        public Task Set(string email, string password)
        {
            var vaultBaseUrl = _settingsService.Get(KeyVaultUrlKey);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            return kv.SetSecretAsync(vaultBaseUrl, CryptoService.Encrypt(email), password);
        }
    }
}
