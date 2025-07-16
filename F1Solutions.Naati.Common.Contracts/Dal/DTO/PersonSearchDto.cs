using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PersonSearchDto
    {
        public int PersonId { get; set; }
        public int NaatiNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryContactNumber { get; set; }
        public ICollection<PersonType> PersonTypes { get; set; }
        public string PractitionerNumber { get; set; }
        public bool IsEportalActive { get; set; }
        public int EntityId { get; set; }
    }
}