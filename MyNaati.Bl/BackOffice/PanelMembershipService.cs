using System;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using MyNaati.Contracts.BackOffice.Panel;

namespace MyNaati.Bl.BackOffice
{
   
    public class PanelMembershipService : IPanelMembershipService
    {
        private IPanelQueryService mPanelService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public PanelMembershipService(IPanelQueryService panelService, IAutoMapperHelper autoMapperHelper)
        {
            mPanelService = panelService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GetPanelsResponse GetPanels(GetPanelsRequest request)
        {
            var serviceRequest = new F1Solutions.Naati.Common.Contracts.Dal.Request.GetPanelsRequest
            {
                NAATINumber = new[] { request.NAATINumber },
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RoleCategoryIds = request.RoleCategoryIds,
                Chair = request.Chair,
                IsVisibleInEportal = request.IsVisibleInEportal
            };

            var result = mPanelService.GetPanels(serviceRequest);
            var data = new GetPanelsResponse
            {
                Panels = (from p in result.Panels
                          select _autoMapperHelper.Mapper.Map<Panel>(p)).ToArray()
            };
            return data;
        }

        public ValidateExaminerSecurityCodeResponse ValidateExaminerSecurityCode(ValidateExaminerSecurityCodeRequest request)
        {
            var result = mPanelService.ValidateExaminerSecurityCode(_autoMapperHelper.Mapper.Map<F1Solutions.Naati.Common.Contracts.Dal.Request.ValidateExaminerSecurityCodeRequest>(request));
            return _autoMapperHelper.Mapper.Map<ValidateExaminerSecurityCodeResponse>(result);
        }


        public SendSecurityCodeResponse SendSecurityCode(SendSecurityCodeRequest request)
        {
            // no longer used; functionality has been removed from the NCMS service
            throw new NotImplementedException();
        }


        public GetMembershipsResponse GetMemberships(GetMembershipsRequest request)
        {
            var result = mPanelService.GetMemberships(_autoMapperHelper.Mapper.Map<F1Solutions.Naati.Common.Contracts.Dal.Request.GetMembershipsRequest>(request));
            var data = new GetMembershipsResponse
            {
                People = (from p in result.People
                          select _autoMapperHelper.Mapper.Map<Membership>(p)).ToArray()
            };
            return data;
        }
    }
}
