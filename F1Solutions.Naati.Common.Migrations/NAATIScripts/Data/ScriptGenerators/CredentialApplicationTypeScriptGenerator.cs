using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class CredentialApplicationTypeScriptGenerator : BaseScriptGenerator
    {
        public CredentialApplicationTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }

        public override string TableName => "tblCredentialApplicationType";

        public override IList<string> Columns => new[]
        {
            "CredentialApplicationTypeId",
            "Name",
            "DisplayName",
            "Online",
            "RequiresChecking",
            "RequiresAssessment",
            "BackOffice",
            "PendingAllowed",
            "AssessmentReviewAllowed",
            "DisplayBills",
            "CredentialApplicationTypeCategoryId",
            "AllowMultiple"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Transition, new [] { "Transition", "0", "1", "1", "1", "1", "0", "1", "2", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Certification, new[] { "Certification", "1", "1", "1", "1", "1", "0", "1", "1", "1" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CCL, new[] { "Credentialed Community Language Test (v1)", "1", "1", "0", "0", "0", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Ethics, new[] { "Ethical Competency Test", "0", "0", "0", "1", "0", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Intercultural, new[] { "Intercultural Competency Test", "0", "0", "0", "1", "0", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CertificationPractitioner, new[] { "Certification Practitioner", "1", "1", "1", "1", "1", "0", "1", "1", "1" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CCLV2, new[] { "Credentialed Community Language Test (v2)", "1", "1", "0", "0", "0", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Recertification, new[] { "Recertification", "1", "1", "1", "1", "1", "0", "1", "3", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Cla, new[] { "Community Language Aide", "1", "1", "0", "1", "0", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.IndigenousCertification, new[] { "Indigenous Certification", "0", "1", "1", "1", "1", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.IndigenousEthics, new[] { "Indigenous Ethical Competency Test", "0", "1", "0", "1", "0", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.IndigenousIntercultural, new[] { "Indigenous Intercultural Competency Test", "0", "1", "0", "1", "0", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CCLV3, new[] { "Credentialed Community Language Test", "1", "1", "0", "1", "0", "0", "0", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CSLIKnowledgeTest, new[] { "Specialist Legal Interpreter Knowledge Test", "0", "0", "0", "1", "0", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.CSHIKnowledgeTest, new[] { "Specialist Health Interpreter Knowledge Test", "0", "0", "0", "1", "0", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.Accreditation, new[] { "Accreditation", "0", "1", "1", "1", "1", "0", "1", "1", "0" });
            CreateOrUpdateTableRow(CredentialApplicationTypeName.PracticeTest, new[] { "Practice Test", "1", "0", "0", "1", "0", "0", "1", "1", "0" });
        }
    }
}