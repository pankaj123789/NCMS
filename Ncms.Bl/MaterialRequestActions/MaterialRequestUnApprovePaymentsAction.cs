using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestUnApprovePaymentsAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.Finalised, MaterialRequestStatusTypeName.Cancelled, MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => CurrentEntryState;

        protected override SecurityVerbName? RequiredSecurityVerb => SecurityVerbName.Update;

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
                    UnApproveMembers,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }

        protected override string GetNote()
        {
            return Naati.Resources.MaterialRequest.PaymentUnApproved;
        }

        protected void UnApproveMembers()
        {
            foreach (var member in ActionModel.MaterialRequestInfo.Members)
            {
                var payRoll = member.PayRoll;
                if (payRoll == null)
                {
                    payRoll = new MaterialRequestPayrollModel();
                    member.PayRoll = payRoll;
                }

                payRoll.ApprovedDate = null;
                payRoll.ApprovedByUserId = null;
            }
        }

        private void ValidateMembers()
        {
            if (ActionModel.MaterialRequestInfo.Members.Any(x => x.PayRoll?.ApprovedDate == null))
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.MaterialRequestHasBeenModfied, WizardModel.MaterialRequestId));
            }

            if (ActionModel.MaterialRequestInfo.Members.Any(x => x.PayRoll.PaymentDate != null))
            {
                throw new UserFriendlySamException(string.Format(Naati.Resources.MaterialRequest.ExaminerHasBeenPaid, WizardModel.MaterialRequestId));
            }
        }

    }
}
