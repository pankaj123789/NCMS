using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class RolePlayerActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();

            //TFS 190029

            //var allocateRolePlayerNoneEvent233 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.AllocateRolePlayer)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(233)
            //    .To(EmailTemplateDetailTypeName.SendToRolePlayer);

            //builders.Add(allocateRolePlayerNoneEvent233);

            //var notifyTestSessionRehearsalDetailsNoneEvent234 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.NotifyTestSessionRehearsalDetails)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(234)
            //    .To(EmailTemplateDetailTypeName.SendToRolePlayer);

            //builders.Add(notifyTestSessionRehearsalDetailsNoneEvent234);

            //var rolePlayerRemoveFromTestSessionNoneEvent235 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.RolePlayerRemoveFromTestSession)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(235)
            //    .To(EmailTemplateDetailTypeName.SendToRolePlayer);

            //builders.Add(rolePlayerRemoveFromTestSessionNoneEvent235);

            //var rolePlayerNotifyAllocationUpdateNoneEvent236 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.RolePlayerNotifyAllocationUpdate)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(236)
            //    .To(EmailTemplateDetailTypeName.SendToRolePlayer);

            //builders.Add(rolePlayerNotifyAllocationUpdateNoneEvent236);

            //var rolePlayerMarkAsRemovedNoneEvent235 = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.RolePlayerMarkAsRemoved)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(235)
            //    .To(EmailTemplateDetailTypeName.SendToRolePlayer);

            //builders.Add(rolePlayerMarkAsRemovedNoneEvent235);

            return builders;
        }
    }
}