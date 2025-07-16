using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts
{
    public class RefreshUserNotificationsRequest
    {
        public IEnumerable<string> UserNames { get; set; }
    }
}
