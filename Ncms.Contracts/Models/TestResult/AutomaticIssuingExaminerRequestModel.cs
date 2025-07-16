using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.TestResult
{
    public class AutomaticIssuingExaminerRequestModel
    {
        public int TestSittingId { get; set; }
        public bool? AutomaticIssuingExaminer { get; set; }
    }
}
