using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestUploadFinalDocumentsAction : MaterialRequestUpdateAction
    {
        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new[] { MaterialRequestStatusTypeName.AwaitingFinalisation };

        protected override MaterialRequestStatusTypeName MaterialRequestExitState => MaterialRequestStatusTypeName.Finalised;

        protected override IList<Action> Preconditions
        {
            get
            {
                var actions = new List<Action>
                {
                    ValidateUserPermissions,
                    ValidateEntryState,
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
                    UpdateMaterialRequestDetails,
                    AddDocuments,
                    AddOutputMaterialLink,
                    EnableTestMaterial,
                    ClearOwner,
                    CreateNote,
                    SetExitState
                };

                return actions;
            }
        }

        private void AddOutputMaterialLink()
        {
            var sourceMaterialId = ActionModel.MaterialRequestInfo.SourceTestMaterial?.Id;

            if (sourceMaterialId.HasValue)
            {
                ActionModel.MaterialRequestInfo.OutputTestMaterial.Links.Add(new TestMaterialLinkModel()
                {
                    FromTestMaterialId = ActionModel.MaterialRequestInfo.OutputTestMaterial.Id,
                    ToTestMaterialId = sourceMaterialId.Value,
                    TypeId = (int)TestMaterialLinkTypeName.Child
                });
            }
        }

        private void EnableTestMaterial()
        {
            ActionModel.MaterialRequestInfo.OutputTestMaterial.Available = true;
        }
      
    }
}
