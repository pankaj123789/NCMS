using System;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.Common;

namespace MyNaati.Bl.Portal.Security
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
