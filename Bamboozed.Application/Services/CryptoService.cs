using System.Linq;
using System.Text.RegularExpressions;
using Bamboozed.Application.Interfaces;

namespace Bamboozed.Application.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class CryptoService
    {
        private const string CryptoSaltKey = "CryptoSalt";
        private const string CryptoSecretKey = "CryptoSecret";

        private readonly ISettingsService _settingsService;

        public CryptoService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public string Encrypt(string value)
        {
            using var aesAlg = new RijndaelManaged();
            var key = new Rfc2898DeriveBytes(_settingsService.Get(CryptoSecretKey), Encoding.UTF8.GetBytes(_settingsService.Get(CryptoSaltKey)));

            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = aesAlg.Key.Take(16).ToArray();
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var msEncrypt = new MemoryStream();
            msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
            msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(value);
            var encrypted = Convert.ToBase64String(msEncrypt.ToArray());
            
            var alphanumeric = Regex.Replace(encrypted, "[^A-Za-z0-9]", "");
            return alphanumeric + Regex.Replace(value, "[^A-Za-z0-9]", ""); ;
        }
    }
}
