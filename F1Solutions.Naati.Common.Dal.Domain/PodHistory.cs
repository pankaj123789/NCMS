using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class PodHistory : EntityBase
    {
        public virtual string PodName { get; set; }
        public virtual DateTime StartedDate { get; set; }
        public virtual DateTime? TerminationDate { get; set; }
        public virtual string FolderPath { get; set; }
        public virtual string DeletionError { get; set; }
    }
}
