using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class StoredFileDeletePolicyDocumentType : EntityBase
    {
        public virtual DocumentType DocumentType { get; set; }
        public virtual StoredFileDeletePolicy StoredFileDeletePolicy { get; set; }
        public virtual string Description { get; set; }
        public virtual string EntityType { get; set; }
    }
}
