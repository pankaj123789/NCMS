using F1Solutions.Naati.Common.Contracts.Security;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Azure.KeyVault
{
    public class CertificateProvider : BaseKeyVault, ICertificateProvider
    {
        private readonly string _keyPrefix;


        public CertificateProvider()
        {
            _keyPrefix = ConfigurationManager.AppSettings["SecretPrefix"];
        }
    }
}
