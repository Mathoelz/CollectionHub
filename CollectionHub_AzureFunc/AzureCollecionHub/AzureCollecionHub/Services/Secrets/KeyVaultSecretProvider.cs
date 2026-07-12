using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Secrets
{
    public class KeyVaultSecretProvider : ISecretProvider
    {

        private readonly SecretClient _secretClient;
        private readonly Dictionary<string, string> _cache = [];

        public KeyVaultSecretProvider(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task InitializeAsync()
        {
            var secret = await _secretClient.GetSecretAsync("CosmosKey");
            _cache.Add("CosmosKey", secret.Value.Value);
        }

        private async Task<string> GetVaultSecretAsync(string secretName)
        {
            try
            {
                var secret = await _secretClient.GetSecretAsync(secretName);

                return secret.Value.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Secret '{secretName}' konnte nicht geladen werden.",
                    ex);
            }
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            if(_cache.ContainsKey(secretName))
            {
                return _cache[secretName];
            }
            else
            {
                var secret = await GetVaultSecretAsync(secretName);
                _cache.Add(secretName, secret);
                return secret;
            }
        }
        
        public string GetSecret(string secretName)
        {
            return _cache[secretName];
        }
    }
}
