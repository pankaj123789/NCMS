using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class BasicApplicationSearchDto
    {
        public int Id { get; set; }

        public string ApplicationStatus { get; set; }

        public DateTime EnteredDate { get; set; }

        public DateTime StatusChangeDate { get; set; }
    }
}
