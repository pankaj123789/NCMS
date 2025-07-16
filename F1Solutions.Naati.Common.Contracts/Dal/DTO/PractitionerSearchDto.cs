using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Newtonsoft.Json;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class PractitionerSearchDto
    {
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public int NaatiNumber { get; set; }
        public ICollection<PractitionerCredentialTypeDto> CredentialTypes { get; set; }
        public ICollection<PractitionerAddressDto> Addresses { get; set; }
        public ICollection<string> PhonesInPd { get; set; }
        public ICollection<string> EmailsInPd { get; set; }
        public string Website { get; set; }
        public bool ShowPhotoOnline { get; set; }
        public bool Deceased { get; set; }
        public int Hash { get; set; }

        public PractitionerAddressDto DefaultAddress
        {
            get
            {
                return Addresses.FirstOrDefault(x =>
                           x.OdAddressVisibilityTypeId != (int) OdAddressVisibilityTypeName.DoNotShow) ??
                       Addresses.FirstOrDefault(x => x.IsPrimaryAddress);
            }
        }

        public ICollection<int> Language1Ids { get; set; }
        public ICollection<int> Language2Ids { get; set; }
    }

    public class ApiPublicPractitionerSearchDto
    {
        public int PersonId { get; set; }
        public string Surname { get; set; }
        public string GivenName { get; set; }
        public string OtherNames { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public bool Deceased { get; set; }
        
        public ICollection<ApiPubicPractitionerCredentialTypeDto> CredentialTypes { get; set; }
        public ApiPublicPractitionerAddressDto Address { get; set; }
        public ICollection<ApiPublicContactDetailsDto> ContactDetails { get; set; }


    }
}
