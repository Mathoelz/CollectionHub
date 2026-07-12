using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionHub.Functions.Services.Secrets
{
    public interface ISecretProvider
    {
        public Task<string> GetSecretAsync(string secretName);
    }
}
