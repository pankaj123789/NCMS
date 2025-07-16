using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialCategoryScriptGenerator : BaseScriptGenerator
    {
        public CredentialCategoryScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialCategory";
        public override IList<string> Columns => new[] {
                                                           "CredentialCategoryId",
                                                           "Name",
                                                           "DisplayName",
                                                           "WorkPracticePoints",
                                                           "WorkPracticeUnits",
                                                       };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Translator", "Translator", "10000", "Words" });
            CreateOrUpdateTableRow(new[] { "2", "Interpreter", "Interpreter", "40", "Hours/Assignments" });
            CreateOrUpdateTableRow(new[] { "3", "DeafInterpreter", "Deaf Interpreter", "40", "Hours/Assignments" });
            CreateOrUpdateTableRow(new[] { "4", "CCL", "Credentialed Community Language Test", null, null });
            CreateOrUpdateTableRow(new[] { "5", "CLA", "Community Language Aide", null, null });
            CreateOrUpdateTableRow(new[] { "6", "Ethics", "Ethical Competency Test", null, null });
            CreateOrUpdateTableRow(new[] { "7", "Intercultural", "Intercultural Competency Test", null, null });
            CreateOrUpdateTableRow(new[] { "8", "Migration", "Migration Assessment", null, null });
            CreateOrUpdateTableRow(new[] { "9", "CSLI Knowledge Test", "Specialist Legal Interpreter Knowledge Test", null, null });
            CreateOrUpdateTableRow(new[] { "10", "CSHI Knowledge Test", "Specialist Health Interpreter Knowledge Test", null, null });
            CreateOrUpdateTableRow(new[] { "11", "Accreditation", "Accreditation", null, null });
            CreateOrUpdateTableRow(new[] { "12", "PracticeTest", "Practice Test", null, null });
        }
    }
}