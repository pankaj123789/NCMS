using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncms.Contracts.Models.Test
{
    public class FeedbackResponseModel
    {
        public FeedbackResponseModel()
        {
            ExaminerFeedback = new List<ExaminerFeedbackModel>();
        }

        public List<ExaminerFeedbackModel> ExaminerFeedback { get; set; }
    }

    public class ExaminerFeedbackModel
    {
        public string ExaminerName { get; set; }
        public int NaatiNumber { get; set; }
        public string Feedback { get; set; }
    }
}
