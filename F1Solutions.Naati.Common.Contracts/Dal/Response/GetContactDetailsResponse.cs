using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class GetContactDetailsResponse 
    {
        public IEnumerable<AddressDetailsDto> Addresses { get; set; }
        public IEnumerable<PhoneDetailsDto> Phones { get; set; }
        public IEnumerable<EmailDetailsDto> Emails { get; set; }
        public IEnumerable<WebsiteDetailsDto> Websites { get; set; }
        public bool ShowWebsite { get; set; }
        public bool IsMyNaatiRegistered { get; set; }
    }
}