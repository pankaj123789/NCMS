using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdateDueDateRequest
    {
        public List<int> JobIds { get; set; }
        public DateTime DueDate { get; set; }
    }
}