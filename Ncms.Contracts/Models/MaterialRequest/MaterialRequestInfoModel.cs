using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestInfoModel
    {
        
        public int MaterialRequestId { get; set; }

        
        [LookupDisplay(LookupType.Panel, "PanelName")]
        public int PanelId { get; set; }

        public string RequestTitle { get; set; }

        [LookupDisplay(LookupType.MaterialRequestStatus, "StatusTypeDisplayName")]
        public int StatusTypeId { get; set; }
        [LookupDisplay(LookupType.NaatiUser, "CreatedByUserName")]
        public int CreatedByUserId { get; set; }
        [LookupDisplay(LookupType.NaatiUser, "OwnedByUserName")]
        public int? OwnedByUserId { get; set; }
        public int StatusChangeUserId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OwnedByUserEmail { get; set; }
        public string EnteredOffice { get; set; }

      
        [LookupDisplay(LookupType.MaterialRequestSpecification)]
        public int ProductSpecificationId { get; set; }

        [LookupDisplay(LookupType.MaterialRequestSpecificationCode, "SpecificationCode")]
        public int SpecificationId => ProductSpecificationId;



        public int ProductSpecificationIdCode => ProductSpecificationId;
        public double MaxBillableHours { get; set; }
        public decimal ProductSpecificationCostPerUnit { get; set; }
      
        public IList<MaterialRequestPanelMembershipModel> Members { get; set; }

        public string CoordinatedByName => Members
            ?.FirstOrDefault(x => x.MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator)
            ?.GivenName;
        public TestMaterialModel SourceTestMaterial { get; set; }
        public TestMaterialModel OutputTestMaterial { get; set; }
        
    }


}
