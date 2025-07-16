using System.Collections.Generic;

namespace Ncms.Contracts.Models.Test
{
    public class AddExaminerResponseModel
    {
        public IEnumerable<int> JobIds { get; set; }
        public IEnumerable<int> JobExaminersIds { get; set; }
    }
}
