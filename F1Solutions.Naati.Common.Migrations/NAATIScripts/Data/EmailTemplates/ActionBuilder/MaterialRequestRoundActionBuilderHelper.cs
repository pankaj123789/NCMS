using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class MaterialRequestRoundActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();

            var submitRoundForApprovalNoneEvent268 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitRoundForApproval)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(268)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(submitRoundForApprovalNoneEvent268);

            var submitRoundForApprovalNoneEvent269 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.SubmitRoundForApproval)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(269)
                .To(EmailTemplateDetailTypeName.SendToRequestOwner);

            builders.Add(submitRoundForApprovalNoneEvent269);

            var approveMaterialRequestRoundNoneEvent263 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.ApproveMaterialRequestRound)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(263)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(approveMaterialRequestRoundNoneEvent263);

            var rejectMaterialRequestRoundNoneEvent266 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RejectMaterialRequestRound)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(266)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(rejectMaterialRequestRoundNoneEvent266);

            var revertMaterialRequestRoundNoneEvent267 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.RevertMaterialRequestRound)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(267)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(revertMaterialRequestRoundNoneEvent267);

            var updateMaterialRequestRoundNoneEvent270 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.UpdateMaterialRequestRound)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(270)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(updateMaterialRequestRoundNoneEvent270);

            return builders;
        }
    }
}