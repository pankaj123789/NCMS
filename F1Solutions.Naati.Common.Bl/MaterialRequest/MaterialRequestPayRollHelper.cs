using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;

namespace F1Solutions.Naati.Common.Bl.MaterialRequest
{
    public class MaterialRequestPayRollHelper : IMaterialRequestPayRollHelper
    {
        private readonly IMaterialRequestQueryService _materialRequestQueryService;
        private readonly ISystemQueryService _systemQueryService;

        private double _mCoordinatorLoadingPercentage;

        public MaterialRequestPayRollHelper(IMaterialRequestQueryService materialRequestQueryService, ISystemQueryService systemQueryService)
        {
            _materialRequestQueryService = materialRequestQueryService;
            _systemQueryService = systemQueryService;
        }

        public IEnumerable<MaterialRequestPayrollUserGroupingModel> GetPendingItemsToApprove(int materialRequestId)
        {
            return GetPendingItemsToApprove(new MaterialRequestPayRollRequest() { Take = 200, MaterialRequetsId = materialRequestId });
        }

        public IEnumerable<MaterialRequestPayrollUserGroupingModel> GetPendingItemsToApprove()
        {
            return GetPendingItemsToApprove(new MaterialRequestPayRollRequest() { Take = 200 });
        }

        private IEnumerable<MaterialRequestPayrollUserGroupingModel> GetPendingItemsToApprove(MaterialRequestPayRollRequest request)
        {
            var result = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "MaterialRequestCoordinatorLoadingPercentage" }).Value;

            _mCoordinatorLoadingPercentage = Convert.ToDouble(result);

            var results = _materialRequestQueryService.GetPendingToApproveMaterialRequestPayRolls(request).Results;

            var groups = results.GroupBy(x => x.OwnerUserId)
                .AsParallel()
                .Select(GetUserGrouping).Where(x => x.Items.Any()).ToList();
            return  groups;
        }


        public IEnumerable<MaterialRequestMemberGroupingModel> GetPendingItemsToPay(int materialRequestId)
        {
            return GetPendingItemsToPay(new MaterialRequestPayRollRequest { Take = 200, MaterialRequetsId = materialRequestId });
        }

        public IEnumerable<MaterialRequestMemberGroupingModel> GetPendingItemsToPay()
        {
            return GetPendingItemsToPay(new MaterialRequestPayRollRequest { Take = 200 });
        }

        private IEnumerable<MaterialRequestMemberGroupingModel> GetPendingItemsToPay(MaterialRequestPayRollRequest request)
        {
            var result = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "MaterialRequestCoordinatorLoadingPercentage" }).Value;

            _mCoordinatorLoadingPercentage = Convert.ToDouble(result);

            var results = _materialRequestQueryService.GetPendingToPayMaterialRequestPayRolls(request).Results;

            var requestsByMember = new Dictionary<int, List<MaterialRequestPayRollInfoDto>>();
            var membersById = new Dictionary<int, MaterialRequestPanelMembershipDto>();
            foreach (var materialRequestPayrollInfoDto in results)
            {
                GetRequestsByMembers(materialRequestPayrollInfoDto, requestsByMember, membersById);  
            }

            var groups = new List<MaterialRequestMemberGroupingModel>();
            foreach (var requestGroup in requestsByMember)
            {
                var member = membersById[requestGroup.Key];
                var requests = requestGroup.Value;

                var groupModel = new MaterialRequestMemberGroupingModel
                {
                    PanelMembershiId = member.PanelMemberShipId,
                    DisplayName = member.GivenName,
                    Items = requests.GroupBy(x => x.ProductSpecificationId)
                        .SelectMany(g=> GetMemberProductSpecificationPaymentItems(g, member.PanelMemberShipId))
                        .ToList()
                };

                if (groupModel.Items.Any())
                {
                    groups.Add(groupModel);
                }

            }

            return groups;
        }

        private void GetRequestsByMembers(
            MaterialRequestPayRollInfoDto materialRequestPayRollInfoDto, 
            Dictionary<int, List<MaterialRequestPayRollInfoDto>> requestsByMember, 
            Dictionary<int,MaterialRequestPanelMembershipDto> membersById
        )
        {
            foreach (var member in materialRequestPayRollInfoDto.Members.Where(x => x.PayRoll?.ApprovedDate != null && x.PayRoll.PaymentDate == null))
            {
                List<MaterialRequestPayRollInfoDto> payrollList = null;
                if (!requestsByMember.TryGetValue(member.PanelMemberShipId, out payrollList))
                {
                    payrollList = new List<MaterialRequestPayRollInfoDto>();
                    requestsByMember[member.PanelMemberShipId] = payrollList;
                }
                membersById[member.PanelMemberShipId] = member;
                payrollList.Add(materialRequestPayRollInfoDto);
            }
        }

        private IList<MaterialRequestPaymentItemDetail> GetMemberProductSpecificationPaymentItems(IGrouping<int, MaterialRequestPayRollInfoDto> group, int panelMembershipId)
        {
            var groupList = group.ToList();

            var items = new List<MaterialRequestPaymentItemDetail>();
            var paymentItems = GetPaymentItems(panelMembershipId, groupList);
            if (paymentItems.claims.Any())
            {
                var item = new MaterialRequestPaymentItemDetail
                {
                    ProductSpecificationId = group.Key,
                    GlCode = groupList.First().GlCode,
                    SpecificationCode = groupList.First().SpecificationCode,
                    UnitCost = groupList.First().CostPerUnit,
                    Claims = paymentItems.claims,
                    Quantity = paymentItems.claims.Sum(x => x.HoursSpent)
                };

                items.Add(item);
            }

            if (paymentItems.loading.Any())
            {
                var item = new MaterialRequestPaymentItemDetail
                {
                    ProductSpecificationId = group.Key,
                    SpecificationCode = groupList.First().SpecificationCode + string.Format(" {0}% loading", _mCoordinatorLoadingPercentage), // TODO : Move to resources
                    GlCode = groupList.First().GlCode,
                    UnitCost = groupList.First().CostPerUnit,
                    Loadings = paymentItems.loading,
                    Quantity = 1
                };

                items.Add(item);
            }

            return items;
        }

        private (IList<MaterialRequestMemberPaymentClaimItemModel> claims, IList<MaterialRequestMemberPaymentLoadingItemModel> loading) GetPaymentItems(int panelMembershipId, IList<MaterialRequestPayRollInfoDto> materialRequests)
        {
            var claims = new List<MaterialRequestMemberPaymentClaimItemModel>();
            var loadings = new List<MaterialRequestMemberPaymentLoadingItemModel>();

            foreach (var materialRequest in materialRequests)
            {
                var member = materialRequest.Members.First(x => x.PanelMemberShipId == panelMembershipId);
                foreach (var memberTask in member.Tasks)
                {
                    if (memberTask.HoursSpent <= 0)
                    {
                        continue;
                    }
                    var taskItem = new MaterialRequestMemberPaymentClaimItemModel
                    {
                        HoursSpent = memberTask.HoursSpent,
                        Amount = Convert.ToDecimal(memberTask.HoursSpent) * materialRequest.CostPerUnit,
                        ApprovedByUserId = member.PayRoll.ApprovedByUserId.GetValueOrDefault(),
                        ApprovedDate = member.PayRoll.ApprovedDate.GetValueOrDefault(),
                        MaterialRequestId = materialRequest.MaterialRequestId,
                        MaterialRequestMemberId = member.Id,
                        TaskId = memberTask.Id,
                        TaskTypeId = memberTask.MaterialRequestTaskTypeId,
                        ModifiedDate = materialRequest.ModifiedDate
                    };

                    claims.Add(taskItem);
                }

                if (member.MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator)
                {
                    var totalHours = materialRequest.Members.SelectMany(x => x.Tasks)
                        .Where(y => y.HoursSpent > 0)
                        .Sum(w => w.HoursSpent);
                    var totalClaims = Convert.ToDecimal(totalHours) * materialRequest.CostPerUnit;

                    var loadingItem = new MaterialRequestMemberPaymentLoadingItemModel
                    {
                        Amount = totalClaims * Convert.ToDecimal(_mCoordinatorLoadingPercentage / 100.0),
                        ApprovedDate = member.PayRoll.ApprovedDate.GetValueOrDefault(),
                        ApprovedByUserId = member.PayRoll.ApprovedByUserId.GetValueOrDefault(),
                        MaterialRequestId = materialRequest.MaterialRequestId,
                        MaterialRequestMemberId = member.Id,
                        TotalClaims = totalClaims,
                        TotalHours = totalHours,
                        ModifiedDate = materialRequest.ModifiedDate
                    };

                    loadings.Add(loadingItem);
                }
            }

            return (claims, loadings);
        }

        private MaterialRequestPayrollUserGroupingModel GetUserGrouping(IGrouping<int, MaterialRequestPayRollInfoDto> group)
        {
            return new MaterialRequestPayrollUserGroupingModel
            {
                UserId = group.Key,
                Items = group.GroupBy(x => x.CredentialTypeId).Select(GetCredentialTypeGrouping).Where(x => x.Items.Any()).ToList()
            };
        }

        private MaterialRequestPayrollCredentialUserGroupingModel GetCredentialTypeGrouping(IGrouping<int, MaterialRequestPayRollInfoDto> group)
        {
            return new MaterialRequestPayrollCredentialUserGroupingModel
            {
                CredentialTypeId = group.Key,
                Items = group.Select(GetMaterialRequestGrouping).Where(x => x.Items.Any()).ToList()
            };
        }

        private MaterialRequestGroupingModel GetMaterialRequestGrouping(MaterialRequestPayRollInfoDto materialRequestPayRoll)
        {
            return new MaterialRequestGroupingModel
            {
                MaterialRequestId = materialRequestPayRoll.MaterialRequestId,
                CostPerHour = materialRequestPayRoll.CostPerUnit,
                SubmittedDate = materialRequestPayRoll.SubmittedDate,
                Skill = materialRequestPayRoll.Skill ?? materialRequestPayRoll.Language,
                GlCode = materialRequestPayRoll.GlCode,
                SpecificationCode = materialRequestPayRoll.SpecificationCode,
                ModifiedDate = materialRequestPayRoll.ModifiedDate,
                Items = materialRequestPayRoll.Members.Select(y => GetMemberDetails(materialRequestPayRoll, y)).Where(y => y.Items.Any()).ToList()
            };
        }

        private MaterialRequestPayrollMemberItemModel GetMemberDetails(MaterialRequestPayRollInfoDto payroll, MaterialRequestPanelMembershipDto member)
        {
            return new MaterialRequestPayrollMemberItemModel
            {
                MaterialRequestMemberId = member.Id,
                DisplayName = member.GivenName,
                HoursSpent = GetHoursSpent(member),
                MemberTypeId = member.MemberTypeId,
                Items = GetClaims(payroll, member)
            };
        }


        private double? GetHoursSpent(MaterialRequestPanelMembershipDto member)
        {
            var hoursSpent = member.Tasks.Sum(x => x.HoursSpent);
            if (hoursSpent <= 0)
            {
                return null;
            }

            return hoursSpent;
        }

        private IList<MaterialRequestApprovalClaimItemModel> GetClaims(MaterialRequestPayRollInfoDto payRoll, MaterialRequestPanelMembershipDto member)
        {
            var claims = new List<MaterialRequestApprovalClaimItemModel>();

            var hoursSpent = member.Tasks.Sum(x => x.HoursSpent);
            claims.Add(new MaterialRequestApprovalClaimItemModel()
            {
                ClaimType ="Claims",// todo : Move to resourcesS
                Amount = Convert.ToDecimal(hoursSpent) * payRoll.CostPerUnit,
                HoursSpent = hoursSpent
            });

            if (member.MemberTypeId == (int)MaterialRequestPanelMembershipTypeName.Coordinator)
            {
                var totalHoursSpent = payRoll.Members.SelectMany(x => x.Tasks).Sum(x => x.HoursSpent);
                claims.Add(new MaterialRequestApprovalClaimItemModel()
                {
                    ClaimType = $"Loading ({_mCoordinatorLoadingPercentage}% of total claims)",// Todo: Move to resources,
                    Amount = Convert.ToDecimal((totalHoursSpent * _mCoordinatorLoadingPercentage) / 100.0) * payRoll.CostPerUnit,
                    HoursSpent = null
                });

            }
            return claims;
        }

    }




}
