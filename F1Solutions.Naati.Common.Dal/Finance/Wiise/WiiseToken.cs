using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseToken
    {
        public WiiseToken(string token, string tenant)
        {
            this.Value = token;
            this.Tenant = tenant;
        }
        public string Value { get; }
        public string Tenant { get; }
    }
}
