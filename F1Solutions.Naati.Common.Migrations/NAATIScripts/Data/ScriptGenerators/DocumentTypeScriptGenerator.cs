using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class DocumentTypeScriptGenerator : BaseScriptGenerator
    {
        public DocumentTypeScriptGenerator(NaatiScriptRunner runner)
            : base(runner) { }
        public override string TableName => "tluDocumentType";

        public override IList<string> Columns => new[]
        {
            "DocumentTypeId",
            "Name",
            "DisplayName",
            "ExaminerToolsDownload",
            "ExaminerToolsUpload",
            "DocumentTypeCategoryId"
        };

        public override void RunScripts()
        {
            CreateOrUpdateTableRow(new[] { "1", "Undefined", "Undefined", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "2", "UnmarkedTestAsset", "Unmarked Test Asset", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "3", "MarkedTestAsset", "Marked Test Asset", "0", "1", "1" });
            CreateOrUpdateTableRow(new[] { "4", "EnglishMarking", "English Marking", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "5", "TestMaterial", "Test Material", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "6", "ReviewReport", "Review Report", "0", "1", "1" });
            CreateOrUpdateTableRow(new[] { "7", "GeneralTestDocument", "General Test Document", "0", "0", "1" });
            CreateOrUpdateTableRow(new[] { "8", "General", "General document", "0", "0", "3" });
            CreateOrUpdateTableRow(new[] { "9", "Identification", "Identification", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "10", "WorkPracticeEvidence", "Work Practice Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "11", "Transcript", "Transcript", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "12", "TrainingEvidence", "Training Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "13", "AuslanProficiencyEvidence", "Auslan Proficiency Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "14", "EnglishProficiencyEvidence", "English Proficiency Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "15", "EthicalCompetencyEvidence", "Ethical Competency Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "16", "InterculturalCompetencyEvidence", "Intercultural Competency Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "17", "ApplicationsOther", "Other Application Related", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "18", "ChuchotageEvidence", "Chuchotage Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "19", "CertificateTemplate", "Certificate Template", "0", "0", "3" });
            CreateOrUpdateTableRow(new[] { "20", "Certificate", "Certificate", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "21", "ApplicationForm", "Application Form", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "22", "CredentialLetterTemplate", "Credential Letter Template", "0", "0", "3" });
            CreateOrUpdateTableRow(new[] { "23", "CredentialLetter", "Credential Letter", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "24", "PurchaseOrder", "Purchase Order", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "25", "AiicMembershipEvidence", "AIIC Membership Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "26", "AitcMembershipEvidence", "AITC Membership Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "27", "WorkPractice", "Work Practice", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "28", "ProfessionalDevelopmentActivity", "Professional Development Activity", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "29", "Invoice", "Invoice", "0", "0", "3" });
            CreateOrUpdateTableRow(new[] { "30", "CandidateTestMaterial", "Candidate Test Material", "0", "0", "4" });
            CreateOrUpdateTableRow(new[] { "31", "ExaminerTestMaterial", "Examiner Test Material", "0", "0", "4" });
            CreateOrUpdateTableRow(new[] { "32", "ProfessionalDevelopmentEvidence", "Professional Development Evidence", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "33", "CandidateCoverSheet", "Candidate Cover Sheet", "0", "0", "5" });
            CreateOrUpdateTableRow(new[] { "34", "CandidateInformation", "Candidate Information", "0", "0", "5" });
            CreateOrUpdateTableRow(new[] { "35", "Script", "Script", "1", "0", "4" });
            CreateOrUpdateTableRow(new[] { "36", "ProblemSheet", "Problem Sheet", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "37", "MedicalCertificate", "Medical Certificate", "1", "0", "1" });
            CreateOrUpdateTableRow(new[] { "38", "NaatiEmployment", "NAATI Employment", "0", "0", "6" });
            CreateOrUpdateTableRow(new[] { "39", "PersonOther", "General", "0", "0", "6" });
            CreateOrUpdateTableRow(new[] { "40", "CandidateBrief", "Candidate Brief", "0", "0", "4" });

            CreateOrUpdateTableRow(new[] { "41", "TranscriptOrProofOfEnrolment", "Transcript or Proof of Enrolment", "0", "0", "2" });
            CreateOrUpdateTableRow(new[] { "42", "SourceTemplate", "Source Template", "1", "1", "7" });
            CreateOrUpdateTableRow(new[] { "43", "BlankTemplate", "Blank Template", "1", "1", "7" });
            CreateOrUpdateTableRow(new[] { "44", "MaterialRequestChecklist", "Request Checklist", "1", "1", "7" });
            CreateOrUpdateTableRow(new[] { "45", "Guideline", "Guideline", "1", "1", "7" });
            CreateOrUpdateTableRow(new[] { "46", "DraftDocument", "Draft Document", "1", "1", "8" });
            CreateOrUpdateTableRow(new[] { "47", "MaterialRequestSubmissionChecklist", "Submission Checklist", "1", "1", "8" });
            CreateOrUpdateTableRow(new[] { "48", "CreditNote", "CreditNote", "0", "0", "3" });

        }
    }
}
