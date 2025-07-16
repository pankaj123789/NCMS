using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms.Base;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms
{
    public class FormsScriptGenerator : IScriptGenerator
    {
        private readonly NaatiScriptRunner _scriptRunner;

        public FormsScriptGenerator(NaatiScriptRunner runner)
        {
            this._scriptRunner = runner;
        }

        public void RunScripts()
        {

            var sharedBuilders = new SharedBuilders();

            var formBuilders = new IFormBuilderHelper[]
            {
                new Cclv1ApplicationFormBuilder(sharedBuilders),
                new CertificationApplicationFormBuilder(sharedBuilders),
                new PractitionerApplicationFormBuilder(sharedBuilders),
                new CclApplicationFormBuilder(sharedBuilders),
                new CclUnavailableApplicationFormBuilder(sharedBuilders),
                new ClaApplicationBuilderHelper(sharedBuilders),
                new RecertificationApplicationFormBuilder(sharedBuilders),
                new AdvancedCertificationApplicationFormBuilder(sharedBuilders),
                new PracticeTestFormApplicationFormBuilder(sharedBuilders)
            };

            foreach (var formBuilder in formBuilders)
            {
                formBuilder.CreateForm();
            }

            RunResetDataScripts();

            foreach (var builder in sharedBuilders.GetBuilders())
            {
                var script = builder.GetScript();
                if(!string.IsNullOrEmpty(script))
                {
                    _scriptRunner.RunScript(script);
                }
            }

        }

        private void RunResetDataScripts()
        {
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormAnswerOptionDocumentType");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormAnswerOptionActionType");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormQuestionLogic");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormQuestionAnswerOption");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormAnswerOption");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormQuestion");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormQuestionType");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormSection");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormCredentialType ");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationForm");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormAnswerType");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormActionType");
            _scriptRunner.RunScript("DELETE from tblCredentialApplicationFormUserType");
        }
        public void RunDescendantOrderScripts()
        {
        }
    }
}
