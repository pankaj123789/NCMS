using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class InstituteSearchDto
    {
        public int InstituteId { get; set; }

        
        public int? NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryContactNo { get; set; }
        public string PrimaryEmail { get; set; }
        public int? NoOfContacts { get; set; }
        public int EntityId { get; set; }

        public ICollection<int> ContactIds { get; set; }
    }
}
