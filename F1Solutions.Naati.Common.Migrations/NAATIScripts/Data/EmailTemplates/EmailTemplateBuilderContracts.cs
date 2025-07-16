using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.Builder;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates
{
    public interface IEmailTemplateScriptBuilder : IScriptBuilder
    {
    }

    public interface IEmailTemplateBuilderHelper
    {
        IEnumerable<ISystemActionEmailTemplateScriptBuilder> GetActionBuilders();
    }

    public interface ISystemActionEmailTemplateScriptBuilder : IEmailTemplateScriptBuilder
    {
        SystemActionTypeName SystemActionType { get; }
        SystemActionEventTypeName SystemActionEventType { get; }

        int EmailTemplateId { get; }
    }

    public interface ICredentialWorkflowActionEmailTemplateScriptBuilder : IEmailTemplateScriptBuilder
    {
        ISystemActionEmailTemplateScriptBuilder SystemActionEmailTemplateScriptBuilder { get; }
        CredentialApplicationTypeName CredentialApplicationType { get; }
    }

    public interface ISystemActionEmailTemplateScriptDetailScriptBuilder : IEmailTemplateScriptBuilder
    {
        ISystemActionEmailTemplateScriptBuilder SystemActionEmailTemplateScriptBuilder { get; }
        EmailTemplateDetailTypeName EmailTemplateDetailType { get; }
    }

    public interface ISystemActionTypeEventTypeSelector
    {
        ICredentialApplicationTypeSelector WithEvent(SystemActionEventTypeName eventType);
    }

    public interface ICredentialApplicationTypeSelector
    {
        IEmailTemplateSelector IsExecuted();
        IEmailTemplateSelector IsExecutedOnApplicationTypes(params CredentialApplicationTypeName[] applicationTypes);
        IEmailTemplateSelector IsExecutedOnAnyApplicationType();
    }

    public interface IEmailTemplateSelector
    {
        IEmailTemplateDetailsSelector ThenUseEmailTemplate(int emailTemplateId);
    }
    public interface IEmailTemplateDetailsSelector
    {
        ISystemActionEmailTemplateScriptBuilder To(params EmailTemplateDetailTypeName[] detailTypes);
    }
    public abstract class BaseEmailTemplateScriptBuilder : BaseScriptBuilder<IEmailTemplateScriptBuilder>, IEmailTemplateScriptBuilder
    {
        protected BaseEmailTemplateScriptBuilder(int entityId) : base(entityId)
        {
        }

        public virtual IEnumerable<string> CommonColumns => new[] { "ModifiedByNaati", "ModifiedDate", "ModifiedUser" };
        public virtual IEnumerable<string> CommonValues => new[] { 0.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.000"), 40.ToString() };

        protected override IEnumerable<string> GetColumnsToScript()
        {
            return Columns.Concat(CommonColumns);
        }

        protected override IEnumerable<string> GetValuesToInsert()
        {
            return Values.Concat(CommonValues);
        }
    }
}
