using System;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

namespace MyNaati.Bl.Portal.Security
{
   

    public class DataSecurityProvider : SecurityProvider, IDataSecurityProvider
    {
        public DataSecurityProvider(Func<string> getUserEmail)
            : base(getUserEmail)
        {
        }

        public bool CurrentUserCanAccess(string email)
        {
            return string.Equals(CurrentUserEmail, email, StringComparison.OrdinalIgnoreCase);
        }
    }
}
