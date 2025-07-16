using System;
using F1Solutions.Naati.Common.Contracts.Bl;

namespace MyNaati.Contracts.BackOffice.Panel
{

    public interface IPanelMembershipService : IInterceptableservice
    {
        GetPanelsResponse GetPanels(GetPanelsRequest request);

        
        GetMembershipsResponse GetMemberships(GetMembershipsRequest request);

        
        ValidateExaminerSecurityCodeResponse ValidateExaminerSecurityCode(ValidateExaminerSecurityCodeRequest request);

        
        SendSecurityCodeResponse SendSecurityCode(SendSecurityCodeRequest request);
    }

    
    public class SendSecurityCodeResponse
    {
        
        public bool Success { get; set; }
    }

    
    public class SendSecurityCodeRequest
    {
        
        public int NAATINumber { get; set; }
    }

    
    public class ValidateExaminerSecurityCodeRequest
    {
        
        public string SecurityCode { get; set; }

        
        public int NAATINumber { get; set; }
    }

    
    public class ValidateExaminerSecurityCodeResponse
    {
        
        public bool Valid { get; set; }
    }

    
    public class GetPanelsRequest
    {
        
        public int NAATINumber { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public int[] RoleCategoryIds { get; set; }
        
        public bool? IsVisibleInEportal { get; set; }

        public bool? Chair { get; set; }
    }

    
    public class GetMembershipsRequest
    {
        
        public int PanelId { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }

    
    public class GetPanelsResponse
    {
        
        public Panel[] Panels { get; set; }
    }

    
    public class GetMembershipsResponse
    {
        
        public Membership[] People { get; set; }
    }

    public class Panel
    {
        
        public int PanelId { get; set; }
        
        public int LanguageId { get; set; }
        
        public string Name { get; set; }
        
        public string Note { get; set; }
        
        public int PanelTypeId { get; set; }
        
        public DateTime ComissionedDate { get; set; }
    }

    public class Membership
    {
        
        public int PanelMembershipId { get; set; }
        
        public int NAATINumber { get; set; }
        
        public string Name { get; set; }
    }
}
