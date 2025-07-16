using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpdatePersonDetailsRequest
    {
        public int? BirthCountryId { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Deceased { get; set; }
        public bool DoNotSendCorrespondence { get; set; }
        public string Gender { get; set; }
        public int PersonId { get; set; }
        public int EntityTypeId { get; set; }
        public string NaatiNumberDisplay { get; set; }
    }
}