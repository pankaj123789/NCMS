using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestComponentTypeStandardMarkingScheme : EntityBase
    {
        public virtual TestComponentType TestComponentType { get; set; }
        public virtual int TotalMarks { get; set; }
        public virtual double PassMark { get; set; }
        public virtual User ModifiedUser { get; set; }
        public virtual bool ModifiedByNaati { get; set; }
        public virtual DateTime ModifiedDate { get; set; }
    }
}
