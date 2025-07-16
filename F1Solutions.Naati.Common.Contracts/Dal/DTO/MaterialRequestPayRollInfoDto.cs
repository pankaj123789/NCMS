using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestPayRollInfoDto
    {
        public int MaterialRequestId { get; set; }
        public int CredentialTypeId { get; set; }
        public int ProductSpecificationId { get; set; }
        public decimal CostPerUnit { get; set; }
        public string GlCode { get; set; }

        public string SpecificationCode { get; set; }
        public bool GstApplies { get; set; }

        public int OwnerUserId { get; set; }

        public string Skill { get; set; }
        public string Language { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IEnumerable<MaterialRequestPanelMembershipDto> Members { get; set; }
    }
}
