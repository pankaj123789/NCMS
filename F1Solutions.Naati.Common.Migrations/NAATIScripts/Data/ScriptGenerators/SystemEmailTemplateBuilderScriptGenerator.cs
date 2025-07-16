using System.Collections.Generic;using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.EmailTemplates.ActionBuilder;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SystemEmailTemplateBuilderScriptGenerator : IScriptGenerator
    {
        private readonly NaatiScriptRunner _scriptRunner;

        public SystemEmailTemplateBuilderScriptGenerator(NaatiScriptRunner runner)
        {
            _scriptRunner = runner;
        }

        public void RunScripts()
        {
            RunResetDataScripts();
            var builderHelpers = GetBuilderHelpers();
            foreach (var builderHelper in builderHelpers)
            {
                var builders = builderHelper.GetActionBuilders();

                foreach (var builder in builders)
                {
                    var script = builder.GetScript();
                    if (!string.IsNullOrEmpty(script))
                    {
                        _scriptRunner.RunScript(script);
                    }
                }
            }
        }

        public void RunDescendantOrderScripts()
        {
        }

        public IList<IEmailTemplateBuilderHelper> GetBuilderHelpers()
        {
            return new IEmailTemplateBuilderHelper[]
            {
                new CredentialApplicationActionBuilderHelper(),
                new CredentialRequestActionBuilderHelper(), 
                new MaterialRequestActionBuilderHelper(), 
                new MaterialRequestRoundActionBuilderHelper(), 
                new RolePlayerActionBuilderHelper(), 
                new TestSessionsActionBuilderHelper()
            };
        }

        private void RunResetDataScripts()
        {
            _scriptRunner.RunScript("DELETE from tblSystemActionEmailTemplateDetail");
            _scriptRunner.RunScript("DELETE from tblCredentialWorkflowActionEmailTemplate");
            _scriptRunner.RunScript("DELETE from tblSystemActionEmailTemplate");
        }
    }
}