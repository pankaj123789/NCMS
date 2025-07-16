using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class MaterialRequestActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();

            var createMaterialRequestNoneEvent264 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateMaterialRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(264)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(createMaterialRequestNoneEvent264);

            var cloneMaterialRequestNoneEvent264 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CloneMaterialRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(264)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(cloneMaterialRequestNoneEvent264);

            var createMaterialRequestRoundNoneEvent264 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CreateMaterialRequestRound)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(264)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(createMaterialRequestRoundNoneEvent264);

            var updateMaterialRequestCoordinatorChangedEvent264 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.UpdateMaterialRequest)
                .WithEvent(SystemActionEventTypeName.CoordinatorChanged)
                .IsExecuted()
                .ThenUseEmailTemplate(264)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(updateMaterialRequestCoordinatorChangedEvent264);

            var updateMaterialRequestNoneEvent271 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.UpdateMaterialRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(271)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(updateMaterialRequestNoneEvent271);

            var cancelMaterialRequestNoneEvent265 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.CancelMaterialRequest)
                .WithEvent(SystemActionEventTypeName.None)
                .IsExecuted()
                .ThenUseEmailTemplate(265)
                .To(EmailTemplateDetailTypeName.SendToCoordinator);

            builders.Add(cancelMaterialRequestNoneEvent265);

            var updateMaterialRequestMembersCollaboratorAddedEvent272 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.UpdateMaterialRequestMembers)
                .WithEvent(SystemActionEventTypeName.CollaboratorAdded)
                .IsExecuted()
                .ThenUseEmailTemplate(272)
                .To(EmailTemplateDetailTypeName.SendToCollaborator);

            builders.Add(updateMaterialRequestMembersCollaboratorAddedEvent272);

            var updateMaterialRequestMembersCollaboratorRemovedEvent273 = EmailTemplateBuilderExtension
                .WhenAction(SystemActionTypeName.UpdateMaterialRequestMembers)
                .WithEvent(SystemActionEventTypeName.CollaboratorRemoved)
                .IsExecuted()
                .ThenUseEmailTemplate(273)
                .To(EmailTemplateDetailTypeName.SendToCollaborator);

            builders.Add(updateMaterialRequestMembersCollaboratorRemovedEvent273);

            return builders;
        }
    }
}