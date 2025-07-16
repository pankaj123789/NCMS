using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder
{
    public class TestSessionsActionBuilderHelper : IEmailTemplateBuilderHelper
    {
        public IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders()
        {
            var builders = new List<ISystemActionEmailTemplateScriptBuilder>();

            //var createTestMaterialReminderEmailEvent = EmailTemplateBuilderExtension
            //    .WhenAction(SystemActionTypeName.SendTestSessionSittingMaterialReminder)
            //    .WithEvent(SystemActionEventTypeName.None)
            //    .IsExecuted()
            //    .ThenUseEmailTemplate(284)
            //    .To(EmailTemplateDetailTypeName.SendToCoordinator);

            //builders.Add(createTestMaterialReminderEmailEvent);

            return builders;
        }
    }
}