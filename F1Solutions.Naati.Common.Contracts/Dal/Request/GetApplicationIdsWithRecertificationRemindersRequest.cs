using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetApplicationIdsWithRecertificationRemindersRequest
    {
        ///If a credential attached to the application has an expiry date matching one of these dates
        ///then the application id will be returned.
        public IEnumerable<DateTime> ExpiryDates { get; set; }
     
    }
}
