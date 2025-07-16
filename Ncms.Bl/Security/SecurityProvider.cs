using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Bl.Security
{
    public class SecurityProvider
    {
        private readonly Func<string> mGetUserEmail;

        public SecurityProvider(Func<string> getUserEmailEmail)
        {
            mGetUserEmail = getUserEmailEmail;
        }

        public string CurrentUserEmail
        {
            get
            {
                var currentUerEmail = mGetUserEmail();

                return currentUerEmail;
            }
        }
    }
}
