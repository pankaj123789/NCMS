using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestApprovePaymentAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.Finalised, MaterialRequestStatusTypeName.Cancelled, MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => CurrentEntryState;

        protected override SecurityNounName? RequiredSecurityNoun => SecurityNounName.Bill;
        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Approve;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                    ValidateMembers
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
                    ApproveMembers,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }


        protected override string GetNote()
        {
            return Naati.Resources.MaterialRequest.PaymentApproved;
        }

        protected void ApproveMembers()
        {
            foreach (var member in ActionModel.MaterialRequestInfo.Members)
            {
                var payRoll = member.PayRoll;
                if (payRoll == null)
                {
                    payRoll = new MaterialRequestPayrollModel();
                    member.PayRoll = payRoll;
                }

                payRoll.ApprovedDate = DateTime.Now;
                payRoll.ApprovedByUserId =UserService.Get().Id;
            }
        }


        private void ValidateMembers()
        {
            if (ActionModel.MaterialRequestInfo.Members.Any(x => x.PayRoll?.ApprovedDate != null))
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MembersHasBeenAlreadyApproved, WizardModel.MaterialRequestId));
            }

            var userGrouping = MaterialRequestService.GetPendingItemsToApprove(WizardModel.MaterialRequestId).Data.FirstOrDefault();

            if (userGrouping == null  || userGrouping.Items.Count != 1 || userGrouping.Items.First().Items.Count != 1)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.PendingPaymentNotFound, WizardModel.MaterialRequestId));
            }

            var materialRequestGroup = userGrouping.Items.First().Items.First();

            if (materialRequestGroup.ModifiedDate != ActionModel.MaterialRequestInfo.StatusChangeDate)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MaterialRequestHasBeenModfied, WizardModel.MaterialRequestId));
            }

            if (materialRequestGroup.CostPerHour != ActionModel.MaterialRequestInfo.ProductSpecificationCostPerUnit)
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MaterialRequestHasBeenModfied, WizardModel.MaterialRequestId));
            }
        }
       
    }
}
