using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Bl
{
    public interface IMaterialRequestPayRollHelper
    {
        IEnumerable<MaterialRequestPayrollUserGroupingModel> GetPendingItemsToApprove();
        IEnumerable<MaterialRequestMemberGroupingModel> GetPendingItemsToPay();

        IEnumerable<MaterialRequestMemberGroupingModel> GetPendingItemsToPay(int materialRequestId);
       IEnumerable<MaterialRequestPayrollUserGroupingModel> GetPendingItemsToApprove(int materialRequestId);
    }

    public class MaterialRequestPayrollUserGroupingModel
    {
        [LookupDisplay(LookupType.NaatiUser)]
        public int UserId { get; set; }

        public IList<MaterialRequestPayrollCredentialUserGroupingModel> Items { get; set; }

        public MaterialRequestPayrollUserGroupingModel()
        {
            Items = new List<MaterialRequestPayrollCredentialUserGroupingModel>();
        }

    }

    public class MaterialRequestPayrollCredentialUserGroupingModel
    {
        [LookupDisplay(LookupType.CredentialType)]
        public int CredentialTypeId { get; set; }

        public DateTime OldestSubmittedate => Items.Min(x => x.SubmittedDate);

        public int TotalRequests => Items.Count;

        public IList<MaterialRequestGroupingModel> Items { get; set; }

        public MaterialRequestPayrollCredentialUserGroupingModel()
        {
            Items = new List<MaterialRequestGroupingModel>();
        }
    }

    public class MaterialRequestGroupingModel
    {
        public int MaterialRequestId { get; set; }

        public bool Approved { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string Skill { get; set; }
        public double? TotalHours => Math.Abs(Items.Sum(x => x.HoursSpent.GetValueOrDefault())) < 0 ? (double?)null : Items.Sum(x => x.HoursSpent.GetValueOrDefault());

        public decimal CostPerHour { get; set; }
        public decimal Amount => Items.Sum(x => x.Amount);

        public string SpecificationCode { get; set; }
        public string GlCode { get; set; }

        public DateTime ModifiedDate { get; set; }
        public IList<MaterialRequestPayrollMemberItemModel> Items { get; set; }

        public MaterialRequestGroupingModel()
        {
            Items = new List<MaterialRequestPayrollMemberItemModel>();
        }
    }


    public class MaterialRequestPayrollMemberItemModel
    {
        public int MaterialRequestMemberId { get; set; }
      
        public string DisplayName { get; set; }

        [LookupDisplay(LookupType.MaterialRequestRoundMembershipType)]
        public int MemberTypeId { get; set; }
        public double? HoursSpent { get; set; }

        public decimal Amount => Items.Sum(x => x.Amount);
        public IList<MaterialRequestApprovalClaimItemModel> Items { get; set; }

        public MaterialRequestPayrollMemberItemModel()
        {
            Items = new List<MaterialRequestApprovalClaimItemModel>();
        }
    }

    public class MaterialRequestApprovalClaimItemModel
    {
        public string ClaimType { get; set; }
        public double? HoursSpent { get; set; }
        public decimal Amount { get; set; }
    }

    public class MaterialRequestMemberGroupingModel
    {
        public int PanelMembershiId { get; set; }
        public string DisplayName { get; set; }

        public int TotalRequests => Items.Sum(x=> x.Claims.Count + x.Loadings.Count);
        public decimal Amount => Items.Sum(x => x.Total);

        public DateTime OldestApprovedDate => Items
            .SelectMany(y => y.Claims.Select(c => c.ApprovedDate).Concat(y.Loadings.Select(l => l.ApprovedDate)))
            .Max();


        public string PaymentReference { get; set; }

        public IList<MaterialRequestPaymentItemDetail> Items { get; set; }
        public MaterialRequestMemberGroupingModel()
        {
            Items = new List<MaterialRequestPaymentItemDetail>();
        }
    }

    public class MaterialRequestPaymentItemDetail
    {
        public int ProductSpecificationId { get; set; }
        public string SpecificationCode { get; set; }
        public string GlCode { get; set; }

        public double Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Total => Claims.Sum(x => x.Amount) + Loadings.Sum(x => x.Amount);

        public IList<MaterialRequestMemberPaymentClaimItemModel> Claims { get; set; }
        public IList<MaterialRequestMemberPaymentLoadingItemModel> Loadings { get; set; }

        public MaterialRequestPaymentItemDetail()
        {
            Claims = new List<MaterialRequestMemberPaymentClaimItemModel>();
            Loadings = new List<MaterialRequestMemberPaymentLoadingItemModel>();
        }
    }


    public class MaterialRequestMemberPaymentClaimItemModel
    {
        public int MaterialRequestMemberId { get; set; }

        public int MaterialRequestId { get; set; }
      

        public bool Removed { get; set; }

       
        public int TaskId { get; set; }

        [LookupDisplay(LookupType.MaterialRequestTaskType)]
        public int TaskTypeId { get; set; }
        public double HoursSpent { get; set; }
        public decimal Amount { get; set; }

        [LookupDisplay(LookupType.NaatiUser)]
        public int ApprovedByUserId { get; set; }
        public DateTime ApprovedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

    public class MaterialRequestMemberPaymentLoadingItemModel
    {
        public int MaterialRequestMemberId { get; set; }

        public int MaterialRequestId { get; set; }
        public double TotalHours { get; set; }
        public decimal TotalClaims { get; set; }
        public decimal Amount { get; set; }
        public bool Removed { get; set; }
        
        [LookupDisplay(LookupType.NaatiUser)]
        public int ApprovedByUserId { get; set; }
        public DateTime ApprovedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}