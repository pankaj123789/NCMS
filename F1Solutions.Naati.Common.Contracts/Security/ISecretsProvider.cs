using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Security
{
    public interface ISecretsProvider
    {
        Task<string> GetAsync(string key);
        string Get(string key);
    }
}
