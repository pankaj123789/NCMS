using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateJobExaminersRequest
    {
        public IEnumerable<ExaminerRequest> Examiners { get; set; }
    }
}