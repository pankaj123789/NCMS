using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using Ncms.Contracts.Models.MaterialRequest;

namespace Ncms.Bl.MaterialRequestActions
{
    public class MaterialRequestCloneAction : MaterialRequestCreateAction
    {

        protected override MaterialRequestStatusTypeName[] MaterialRequestEntryStates => new MaterialRequestStatusTypeName[] {};


        protected override void ValidateEntryState()
        {
        }

        protected override void ConfigureInstance(MaterialRequestActionModel actionModel, MaterialRequestWizardModel wizardModel,
            MaterialRequestActionOutput output)
        {
            var modelAction = MaterialRequestService.GetEmptyActionModel();
            wizardModel.MaterialRequestId = 0;
            wizardModel.MaterialRequestRoundId = 0;
            base.ConfigureInstance(modelAction, wizardModel, output);
        }
    }
}
