using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class StoredFileStatusType : EntityBase
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int DisplayOrder { get; set; }
    }
}
