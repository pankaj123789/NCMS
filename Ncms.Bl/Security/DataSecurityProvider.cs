using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.Security
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
