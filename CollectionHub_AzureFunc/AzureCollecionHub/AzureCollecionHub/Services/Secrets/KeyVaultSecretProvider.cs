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

        public KeyVaultSecretProvider(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            throw new NotImplementedException();
        }
    }
}
