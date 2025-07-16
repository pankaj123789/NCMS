using F1Solutions.Naati.Common.Contracts.Dal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public interface IWiiseAuthorisationService : ITokenAuthorisationService
    {
        string GetTenant();
        string GetTokenAndTenant(string accessCode, out string tenantId);
    }
}
