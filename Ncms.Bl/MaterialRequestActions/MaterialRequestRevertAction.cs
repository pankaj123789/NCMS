using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestRevertAction : MaterialRequestAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.Finalised, MaterialRequestStatusTypeName.Cancelled };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => CurrentEntryState == MaterialRequestStatusTypeName.Finalised ? MaterialRequestStatusTypeName.AwaitingFinalisation : MaterialRequestStatusTypeName.InProgress;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
                    ValidateOutputTestMaterial,
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
                    DisableTestMaterial,
                    RemoveLink,
                    RevertRound,
                    SetOwner,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }

        private void RevertRound()
        {
            var latestRound = ActionModel.Rounds.OrderBy(x=> x.RoundNumber).Last();
            if (latestRound.StatusTypeId== (int)MaterialRequestRoundStatusTypeName.Cancelled)
            {
                latestRound.StatusTypeId = (int) MaterialRequestRoundStatusTypeName.SentForDevelopment;
            }
        }
        
        protected void ValidateOutputTestMaterial()
        {
            var testMasterial = MaterialRequestService.SearchTestMaterials(ActionModel.MaterialRequestInfo.OutputTestMaterial.Id.ToString(), false).Data.SingleOrDefault();

            if (testMasterial == null || testMasterial.StatusId != (int)TestMaterialStatusTypeName.New)
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.TestMaterialHasBeenAssigned);
            }
            if ( MaterialRequestService.IsUsedAsSourceMaterial(ActionModel.MaterialRequestInfo.OutputTestMaterial.Id).Data)
            {
                throw new UserFriendlySamException(Naati.Resources.MaterialRequest.MaterialHasBeenUsedAsSource);
            }
        }

        protected void DisableTestMaterial()
        {
            ActionModel.MaterialRequestInfo.OutputTestMaterial.Available = false;
        }

        protected void RemoveLink()
        {
            var sourceMaterialId = ActionModel.MaterialRequestInfo.SourceTestMaterial?.Id;
            if (sourceMaterialId.HasValue)
            {
                var linkToRemove = ActionModel.MaterialRequestInfo.OutputTestMaterial.Links.FirstOrDefault(x => x.TypeId == (int)TestMaterialLinkTypeName.Child && x.ToTestMaterialId == sourceMaterialId.Value);

                if (linkToRemove != null)
                {
                    ActionModel.MaterialRequestInfo.OutputTestMaterial.Links.Remove(linkToRemove);
                }
            }
          
        }
    }
}
