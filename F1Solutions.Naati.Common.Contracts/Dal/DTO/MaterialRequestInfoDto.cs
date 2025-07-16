using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestInfoDto
    {
        public int MaterialRequestId { get; set; }
        public int PanelId { get; set; }
        public string RequestTitle { get; set; }
        public int StatusTypeId { get; set; }
        public string StatusTypeDisplayName { get; set; }
        public int CreatedByUserId { get; set; }
        public string RequestTypeDisplayName { get; set; }
        public int EnteredOfficeId { get; set; }
        public string OwnedByUserEmail { get; set; }
        public int StatusChangeUserId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public string EnteredOffice { get; set; }
        public TestMaterialDto SourceTestMaterial { get; set; }
        public TestMaterialDto OutputTestMaterial { get; set; }

        public int? OwnedByUserId { get; set; }
        public int ProductSpecificationId { get; set; }

        public double MaxBillableHours { get; set; }
        public MaterialRequestRoundDto LastRound { get; set; }

        public IEnumerable<MaterialRequestPanelMembershipDto> Members { get; set; }
        public decimal ProductSpecificationCostPerUnit { get; set; }
        public int TestMaterialDomainId { get; set; }
    }
}
