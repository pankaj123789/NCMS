using System;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.Application
{
    public class CredentialResultModel 
    {
        public int CredentialId { get; set; }
        public int CredentialRequestId { get; set; }
        public int CredentialApplicationId { get; set; }
        public int NaatiNumber { get; set; }
        public string CredentialTypeInternalName { get; set; }
        public string Direction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime IssuedDate { get; set; }

        [LookupDisplay(LookupType.CredentialStatusType)]
        public int StatusId { get; set; }
        public bool ShowInOnlineDirectory { get; set; }
        public string Category { get; set; }
    }
}
