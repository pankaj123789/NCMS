using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class JobExaminerPayrollStatus: EntityBase
    {
        public virtual JobExaminer JobExaminer { get; set; }
        public virtual PayrollStatus PayrollStatus { get; set; }
    }
}
