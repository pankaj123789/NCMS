using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public interface IInvalidCookie : IAuditObject
    {
        string Cookie { get; set; }
        DateTime ExpiryDate { get; set; }
    }
}
