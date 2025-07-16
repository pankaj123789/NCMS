using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class FeedbackDalResponse
    {
        public IEnumerable<ExaminerFeedback> ExaminerFeedback { get; set; }
    }

    public class ExaminerFeedback
    {
        public string ExaminerName { get; set; }
        public int NaatiNumber { get; set; }
        public string Feedback { get; set; }
    }
}
