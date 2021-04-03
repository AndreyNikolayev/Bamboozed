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
        private readonly CryptoService _cryptoService;

        public PasswordService(ISettingsService settingsService,
            CryptoService cryptoService)
        {
            _settingsService = settingsService;
            _cryptoService = cryptoService;
        }

        public async Task<string> Get(string email)
        {
            var vaultBaseUrl = _settingsService.Get(KeyVaultUrlKey);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var vaultResponse = await kv.GetSecretAsync(vaultBaseUrl, _cryptoService.Encrypt(email));

            return vaultResponse.Value;
        }

        public Task Set(string email, string password)
        {
            var vaultBaseUrl = _settingsService.Get(KeyVaultUrlKey);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            return kv.SetSecretAsync(vaultBaseUrl, _cryptoService.Encrypt(email), password);
        }
    }
}
