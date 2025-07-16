using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SystemValueRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}
