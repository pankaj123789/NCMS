using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class StoredFileDeletePolicy : EntityBase
    {
        public virtual int PolicyExecutionOrder { get; set; }
        public virtual string PolicyDescription { get; set; }
        public virtual int DaysToKeep { get; set; }
    }
}
