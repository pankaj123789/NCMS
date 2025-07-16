using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestMarkMemberAsPaidAction : MaterialRequestAction
    {
        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.MaterialRequest;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.MarkAsPaid;

        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.Finalised, MaterialRequestStatusTypeName.Cancelled, MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => CurrentEntryState;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                    VerifyPaymentReference,
                };


                return actions;
            }
        }
        protected override IList<Action> SystemActions
        {
            get
            {
                var actions = new List<Action>
                {
                    AddPayments,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }
        
        protected virtual void VerifyPaymentReference()
        {
            if (string.IsNullOrWhiteSpace(WizardModel.ExaminerPayment?.PaymentReference))
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.PaymentReferenceNotSpecified, WizardModel.ExaminerPayment?.DisplayName));
            }
        }

        protected override string GetNote()
        {
            var member = ActionModel.MaterialRequestInfo.Members.Single(x => x.PanelMemberShipId == WizardModel.ExaminerPayment.PanelMembershiId);
            return string.Format(Naati.Resources.MaterialRequest.MemberPaid, member.GivenName, $"{member.PayRoll.Amount:C}", member.PayRoll.PaymentReference );
        }

      

        private void AddPayments()
        {
            if (ActionModel.MaterialRequestInfo.Members.Any(x => x.PayRoll?.ApprovedDate == null))
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MemberNotApproved, WizardModel.ExaminerPayment.DisplayName));
            }
            
            var foundGroup = MaterialRequestService.GetPendingItemsToPay(WizardModel.MaterialRequestId).Data.Where(x=> x.PanelMembershiId == WizardModel.ExaminerPayment.PanelMembershiId).ToList();

            if (foundGroup == null || foundGroup.Count != 1)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.PendingExaminerPaymentNotFound, WizardModel.ExaminerPayment.DisplayName, WizardModel.MaterialRequestId));
            }
            var memberGrouping = foundGroup.First();

            var pendingPayments = memberGrouping.Items;

            var member = ActionModel.MaterialRequestInfo.Members.Single(
                x => x.PanelMemberShipId == WizardModel.ExaminerPayment.PanelMembershiId);

            var payRoll = member.PayRoll;
            foreach (var pendingPayment in pendingPayments)
            {
                var loadings = pendingPayment.Loadings
                    .Where(x => x.MaterialRequestId == ActionModel.MaterialRequestInfo.MaterialRequestId)
                    .ToList();

                if (loadings.Any(x=> x.ModifiedDate != ActionModel.MaterialRequestInfo.StatusChangeDate))
                {
                    throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.ExaminerItemModified, WizardModel.ExaminerPayment.DisplayName, WizardModel.MaterialRequestId));
                }
                
                var claims = pendingPayment.Claims
                    .Where(x => x.MaterialRequestId == ActionModel.MaterialRequestInfo.MaterialRequestId)
                    .ToList();

                if (claims.Any(x => x.ModifiedDate != ActionModel.MaterialRequestInfo.StatusChangeDate))
                {
                    throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.ExaminerItemModified, WizardModel.ExaminerPayment.DisplayName, WizardModel.MaterialRequestId));
                }
                if (payRoll == null)
                {
                    payRoll = new MaterialRequestPayrollModel();
                    member.PayRoll = payRoll;
                }

                payRoll.Amount = (payRoll.Amount ?? 0) + claims.Sum(x => x.Amount) + loadings.Sum(x => x.Amount);
            }

            payRoll.PaymentDate = DateTime.Now;
            payRoll.PaidByUserId = UserService.Get().Id;
            payRoll.PaymentReference = WizardModel.ExaminerPayment.PaymentReference;
        }

        protected override GenericResponse<UpsertMaterialRequestResultModel> SaveActionData()
        {
            return new UpsertMaterialRequestResultModel() 
            { 
                MaterialRequestIds = new int[0], 
                MaterialRequestRoundIds = new int[0], 
                StoredFileIds = new int[0] 
            };
        }
    }
}
