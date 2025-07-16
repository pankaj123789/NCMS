using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class NcmsInvalidCookie :EntityBase, IInvalidCookie
    {
        public virtual string Cookie { get; set; }
        public virtual DateTime ExpiryDate { get; set; }

    }
}
