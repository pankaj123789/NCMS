using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Job
{
    public class UpdateDueDateRequestModel
    {
        public List<int> JobIds { get; set; }
        public DateTime DueDate { get; set; }
    }
}
