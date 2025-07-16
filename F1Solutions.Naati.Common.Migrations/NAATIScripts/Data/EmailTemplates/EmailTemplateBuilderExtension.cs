using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates
{
    public class EmailTemplateBuilderExtension
    {
        public static ISystemActionTypeEventTypeSelector WhenAction(SystemActionTypeName actionType)
        {
            return new SystemActionEmailTemplateScriptBuilder(actionType);
        }
    }
    public class SystemActionEmailTemplateScriptDetailScriptBuilder : BaseEmailTemplateScriptBuilder, ISystemActionEmailTemplateScriptDetailScriptBuilder
    {
        private static int _nextSystemActionEmailTemplateDetailScriptBuilderId = 1;
        public SystemActionEmailTemplateScriptDetailScriptBuilder(ISystemActionEmailTemplateScriptBuilder systemActionEmailTemplateScriptBuilder, EmailTemplateDetailTypeName emailTemplateDetailType) : base(_nextSystemActionEmailTemplateDetailScriptBuilderId++)
        {
            SystemActionEmailTemplateScriptBuilder = systemActionEmailTemplateScriptBuilder;
            EmailTemplateDetailType = emailTemplateDetailType;
        }

        protected override string TableName => "tblSystemActionEmailTemplateDetail";

        protected override IEnumerable<string> Values => new[]
        {
            this.SystemActionEmailTemplateScriptBuilder.Id.ToString(), ((int)this.EmailTemplateDetailType).ToString()
        };

        protected override IEnumerable<string> Columns => new[]
            { "SystemActionEmailTemplateDetailId", "SystemActionEmailTemplateId", "EmailTemplateDetailTypeId" };

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetParentBuilders()
        {
            return new[] { this.SystemActionEmailTemplateScriptBuilder };
        }

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IEmailTemplateScriptBuilder>();
        }

        public ISystemActionEmailTemplateScriptBuilder SystemActionEmailTemplateScriptBuilder { get; }

        public EmailTemplateDetailTypeName EmailTemplateDetailType { get; }
    }
    public class CredentialWorkflowActionEmailTemplateScriptBuilder : BaseEmailTemplateScriptBuilder, ICredentialWorkflowActionEmailTemplateScriptBuilder
    {
        private static int _nextCredentialWorkflowActionEmailTemplateId = 1;

        public CredentialWorkflowActionEmailTemplateScriptBuilder(ISystemActionEmailTemplateScriptBuilder systemActionEmailTemplateScriptBuilder, CredentialApplicationTypeName credentialApplicationType) : base(_nextCredentialWorkflowActionEmailTemplateId++)
        {
            SystemActionEmailTemplateScriptBuilder = systemActionEmailTemplateScriptBuilder;
            CredentialApplicationType = credentialApplicationType;
        }

        protected override string TableName => "tblCredentialWorkflowActionEmailTemplate";

        protected override IEnumerable<string> Values => new[]
        {
            ((int)this.CredentialApplicationType).ToString(), this.SystemActionEmailTemplateScriptBuilder.Id.ToString()
        };

        protected override IEnumerable<string> Columns => new[]
            { "CredentialWorkflowActionEmailTemplateId", "CredentialApplicationTypeId", "SystemActionEmailTemplateId" };

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetParentBuilders()
        {
            return new[] { this.SystemActionEmailTemplateScriptBuilder };
        }

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetChildBuilders()
        {
            return Enumerable.Empty<IEmailTemplateScriptBuilder>();
        }

        public ISystemActionEmailTemplateScriptBuilder SystemActionEmailTemplateScriptBuilder { get; }

        public CredentialApplicationTypeName CredentialApplicationType { get; }
    }
    public class SystemActionEmailTemplateScriptBuilder : BaseEmailTemplateScriptBuilder,
        ISystemActionEmailTemplateScriptBuilder,
        ISystemActionTypeEventTypeSelector, ICredentialApplicationTypeSelector, IEmailTemplateSelector, IEmailTemplateDetailsSelector
    {
        private static int _nextSystemActionEmailTemplateId = 1;

        private readonly ICollection<ICredentialWorkflowActionEmailTemplateScriptBuilder> _credentialWorkflowActionEmailTemplates;
        private readonly ICollection<ISystemActionEmailTemplateScriptDetailScriptBuilder> _systemActionEmailTemplateDetails;
        internal SystemActionEmailTemplateScriptBuilder(SystemActionTypeName actionType) : base(_nextSystemActionEmailTemplateId++)
        {
            SystemActionType = actionType;
            _credentialWorkflowActionEmailTemplates = new HashSet<ICredentialWorkflowActionEmailTemplateScriptBuilder>();
            _systemActionEmailTemplateDetails = new HashSet<ISystemActionEmailTemplateScriptDetailScriptBuilder>();
        }

        protected override string TableName => "tblSystemActionEmailTemplate";

        protected override IEnumerable<string> Values => new[]
        {
            ((int)this.SystemActionType).ToString(), this.EmailTemplateId.ToString(),
            ((int)this.SystemActionEventType).ToString(), 1.ToString()
        };

        protected override IEnumerable<string> Columns => new[] { "SystemActionEmailTemplateId", "SystemActionTypeId", "EmailTemplateId", "SystemActionEventTypeId", "Active" };

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetParentBuilders()
        {
            return new IEmailTemplateScriptBuilder[] { };
        }

        protected override IEnumerable<IEmailTemplateScriptBuilder> GetChildBuilders()
        {
            return _credentialWorkflowActionEmailTemplates.Cast<IEmailTemplateScriptBuilder>()
                .Concat(_systemActionEmailTemplateDetails);
        }

        public SystemActionTypeName SystemActionType { get; private set; }

        public SystemActionEventTypeName SystemActionEventType { get; private set; }

        public int EmailTemplateId { get; private set; }

        public ICredentialApplicationTypeSelector WithEvent(SystemActionEventTypeName eventType)
        {
            this.SystemActionEventType = eventType;
            return this;
        }

        public IEmailTemplateSelector IsExecuted()
        {
            if (IsInRange(1, 999, (int)this.SystemActionType) || IsInRange(1000, 1999, (int)this.SystemActionType))
            {
                throw new Exception($"Method not allowed for application actions. Action: {this.SystemActionType.ToString()}");
            }

            return this;
        }

        public IEmailTemplateSelector IsExecutedOnAnyApplicationType()
        {
            return IsExecutedOnApplicationTypes(Enum.GetValues(typeof(CredentialApplicationTypeName)).Cast<CredentialApplicationTypeName>().ToArray());
        }

        public IEmailTemplateSelector IsExecutedOnApplicationTypes(params CredentialApplicationTypeName[] applicationTypes)
        {
            if (!IsInRange(1, 999, (int)this.SystemActionType) && !IsInRange(1000, 1999, (int)this.SystemActionType))
            {
                throw new Exception($"System Action Type {this.SystemActionType.ToString()} is not an application type");
            }

            if (!applicationTypes.Any())
            {
                throw new Exception($"No applications have been configured for action type {this.SystemActionType.ToString()}");
            }

            foreach (var applicationType in applicationTypes)
            {
                var builder = new CredentialWorkflowActionEmailTemplateScriptBuilder(this, applicationType);
                _credentialWorkflowActionEmailTemplates.Add(builder);
            }

            return this;
        }

        private bool IsInRange(int from, int to, int value)
        {
            return from <= value && value <= to;
        }

        public IEmailTemplateDetailsSelector ThenUseEmailTemplate(int emailTemplateId)
        {
            EmailTemplateId = emailTemplateId;
            return this;
        }

        public ISystemActionEmailTemplateScriptBuilder To(params EmailTemplateDetailTypeName[] detailTypes)
        {
            foreach (var detail in detailTypes)
            {
                var builder = new SystemActionEmailTemplateScriptDetailScriptBuilder(this, detail);
                _systemActionEmailTemplateDetails.Add(builder);
            }

            return this;
        }
    }


}
