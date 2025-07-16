using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class EmailTemplateDetailTypeScriptGenerator: BaseScriptGenerator
    {
        public EmailTemplateDetailTypeScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblEmailTemplateDetailType";

        public override IList<string> Columns => new[]
        {
            "EmailTemplateDetailTypeId",
            "Name",
            "DisplayName"
        };


        public override void RunScripts()
        {
            //CreateOrUpdateTableRow(new[] { "1", "SendToApplicant", "Send to Applicant" });
            //CreateOrUpdateTableRow(new[] { "2", "SendToSponsor", "Send to Sponsor" });
            //CreateOrUpdateTableRow(new[] { "3", "AttachInvoiceToApplicant", "Attach Invoice to Applicant" });
            //CreateOrUpdateTableRow(new[] { "4", "AttachInoviceToSponsor", "Attach Invoice to Sponsor" });
            //CreateOrUpdateTableRow(new[] { "5", "AttachCredentialDocsToApplicant", "Attach Credential Docs to Applicant" });
            //CreateOrUpdateTableRow(new[] { "6", "SendToRolePlayer", "Send to Role-player" });
            //CreateOrUpdateTableRow(new[] { "7", "AttachCandidateBriefToApplicant", "Attach Candiate Brief to Applicant" });
            //CreateOrUpdateTableRow(new[] { "8", "SendToCoordinator", "Send To Coordinator" });
            //CreateOrUpdateTableRow(new[] { "9", "SendToRequestOwner", "Send To Request Owner" });
            //CreateOrUpdateTableRow(new[] { "10", "AttachRoundRequestDocuments", "Attach Request Documents" });
            //CreateOrUpdateTableRow(new[] { "11", "AttachRoundResponseDocuments", "Attach Response Documents" });
            //CreateOrUpdateTableRow(new[] { "12", "SendToCollaborator", "Send to Collaborator" });
            //CreateOrUpdateTableRow(new[] { "13", "AttachCreditNoteToApplicant", "Attach Credit Note To Applicant" });
            //CreateOrUpdateTableRow(new[] { "14", "AttachCreditNoteToSponsor", "Attach Credit Note To Sponsor" });

            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToApplicant, new[] { "Send to Applicant" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToSponsor, new[] { "Send to Sponsor" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachInoviceToApplicant, new[] { "Attach Invoice To Applicant" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachInoviceToSponsor, new[] { "Attach Invoice to Sponsor" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachCredentialDocsToApplicant, new[] { "Attach Credential Docs to Applicant" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToRolePlayer, new[] { "Send to Role-player" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachCandidateBriefToApplicant, new[] { "Attach Candidate Brief to Applicant" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToCoordinator, new[] { "Send To Coordinator" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToRequestOwner, new[] { "Send To Request Owner" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachRoundRequestDocuments, new[] { "Attach Request Documents" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachRoundResponseDocuments, new[] { "Attach Response Documents" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.SendToCollaborator, new[] { "Send to Collaborator" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachCreditNoteToApplicant, new[] { "Attach Credit Note To Applicant" });
            CreateOrUpdateTableRow(EmailTemplateDetailTypeName.AttachCreditNoteToSponsor, new[] { "Attach Credit Note To Sponsor" });
        }
    }
}
