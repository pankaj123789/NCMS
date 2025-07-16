using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Security;

namespace F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators
{
    public class SecurityRuleScriptGenerator : BaseScriptGenerator
    {
        public SecurityRuleScriptGenerator(NaatiScriptRunner runner) : base(runner)
        {
        }

        public override string TableName => "tblSecurityRule";

        public override IList<string> Columns => new[]
        {
            "SecurityRuleId",
            "SecurityRoleId",
            "SecurityNounId",
            "SecurityVerbMask"
        };

        protected void CreateOrUpdateTableRow(int id, int roleId, SecurityNounName noun, SecurityVerbName verbs)
        {
            var values = new List<string>
            {
                id.ToString(),
                roleId.ToString(),
                ((int) noun).ToString(),
                ((long) verbs).ToString()
            };
            base.CreateOrUpdateTableRow(values);
        }

        public override void RunScripts()
        {
            //delete all because natural key is two fields so insertorupdate wont current work
            //changeset 83391 has code that will do it.
            ScriptRunner.RunScript("DELETE FROM tblSecurityRule");
            var id = 1;


            //SeniorManagement

            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Reactivate | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate | SecurityVerbName.AssignPastSession);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Audit, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Bill, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.CertificationPeriod, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Contact, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Credential, SecurityVerbName.Configure | SecurityVerbName.Download | SecurityVerbName.Extend | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Assign | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Email, SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.EmailTemplate, SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.EndorsedQualification, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Examiner, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.ExaminerMarks, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.FinanceOther, SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Invoice, SecurityVerbName.Create | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Language, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Logbook, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.MaterialRequest, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Notes | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Organisation, SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.OrganisationHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PaidReview, SecurityVerbName.Create | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Panel, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PanelMember, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Payment, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PayRun, SecurityVerbName.Approve | SecurityVerbName.Create | SecurityVerbName.Finalise | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Person, SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PersonFinanceDetails, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.RolePlayer, SecurityVerbName.Configure | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.RubricResult, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Skill, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.SupplementaryTest, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.System, SecurityVerbName.Manage | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestAsset, SecurityVerbName.Delete);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Revert | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestSitting, SecurityVerbName.Invalidate | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.TestSpecification, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Create | SecurityVerbName.Upload | SecurityVerbName.Download);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.User, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 1, SecurityNounName.Venue, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);

            //SystemAdministrator

            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Application, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Audit, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Contact, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Credential, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.CredentialRequest, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Dashboard, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Email, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Organisation, SecurityVerbName.Configure | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.Person, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.System, SecurityVerbName.Manage | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.TestMaterial, SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.TestSession, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.TestSitting, SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.TestSpecification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 2, SecurityNounName.User, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);

            //TestingOperationsAdmin

            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Reactivate | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.CertificationPeriod, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Contact, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Credential, SecurityVerbName.Configure | SecurityVerbName.Download | SecurityVerbName.Extend | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Email, SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.EndorsedQualification, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Examiner, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.ExaminerMarks, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.FinanceOther, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Invoice, SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Language, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Logbook, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.MaterialRequest, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Update | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Organisation, SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.OrganisationHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PaidReview, SecurityVerbName.Create | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Panel, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PanelMember, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PayRun, SecurityVerbName.Read | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Person, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PersonFinanceDetails, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.RolePlayer, SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.RubricResult, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Skill, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.SupplementaryTest, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestAsset, SecurityVerbName.Delete);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Revert | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestSitting, SecurityVerbName.Invalidate | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.TestSpecification, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Create | SecurityVerbName.Upload | SecurityVerbName.Download);
            CreateOrUpdateTableRow(id++, 3, SecurityNounName.Venue, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);

            //TestingOperations

            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Create | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.CertificationPeriod, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Contact, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Credential, SecurityVerbName.Configure | SecurityVerbName.Download | SecurityVerbName.Issue | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Email, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Examiner, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.ExaminerMarks, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.FinanceOther, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Invoice, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Logbook, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Organisation, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.PaidReview, SecurityVerbName.Create);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.PanelMember, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.PayRun, SecurityVerbName.Read | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Person, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.RolePlayer, SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.RubricResult, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.SupplementaryTest, SecurityVerbName.Create | SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.TestSitting, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.Venue, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 4, SecurityNounName.CredentialRequest, SecurityVerbName.ProcessRefund | SecurityVerbName.RejectRefund | SecurityVerbName.RequestRefund);

            //ResourceAndScheduling

            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Application, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Bill, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Credential, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.CredentialRequest, SecurityVerbName.Assign | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Document, SecurityVerbName.Download | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Email, SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.MaterialRequest, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Notes | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Organisation, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.PanelMember, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.PayRun, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Person, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.RolePlayer, SecurityVerbName.Notify);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.TestSitting, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.TestSpecification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 5, SecurityNounName.Venue, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);

            //RegionalManager

            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Audit, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Contact, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Credential, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Email, SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.ExaminerMarks, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.FinanceOther, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Invoice, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.MaterialRequest, SecurityVerbName.Read | SecurityVerbName.Update | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Organisation, SecurityVerbName.Configure | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.PanelMember, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Person, SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.RolePlayer, SecurityVerbName.Configure | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.RubricResult, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestAsset, SecurityVerbName.Delete);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestResult, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestSitting, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.TestSpecification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 6, SecurityNounName.Venue, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);

            //RegionalOperations

            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Audit, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Email, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.ExaminerMarks, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.FinanceOther, SecurityVerbName.Manage | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Invoice, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Organisation, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Person, SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.RolePlayer, SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.RubricResult, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.TestResult, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.TestSitting, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Uninvite | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 7, SecurityNounName.Venue, SecurityVerbName.Read | SecurityVerbName.Search);

            //Communications

            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Application, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Credential, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.CredentialRequest, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Email, SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.EmailTemplate, SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Organisation, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.OrganisationHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.Person, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.RubricResult, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.TestMaterial, SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.TestSession, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 8, SecurityNounName.TestSitting, SecurityVerbName.Read | SecurityVerbName.Search);

            //Finance

            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Application, SecurityVerbName.Close | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Audit, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Bill, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.CredentialRequest, SecurityVerbName.ApproveRefund | SecurityVerbName.Assess | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.ProcessRefund | SecurityVerbName.Read | SecurityVerbName.RejectRefund | SecurityVerbName.RequestRefund | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Email, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.ExaminerMarks, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.FinanceOther, SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Invoice, SecurityVerbName.Create | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.MaterialRequest, SecurityVerbName.MarkAsPaid | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Organisation, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.OrganisationHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.PaidReview, SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.PanelMember, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Payment, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.PayRun, SecurityVerbName.Approve | SecurityVerbName.Create | SecurityVerbName.Finalise | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.Person, SecurityVerbName.Configure | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.PersonFinanceDetails, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.RubricResult, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.SupplementaryTest, SecurityVerbName.Manage | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.TestResult, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.TestSession, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 9, SecurityNounName.TestSitting, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Uninvite);

            //DevelopmentTeam

            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Application, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Bill, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.CertificationPeriod, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Credential, SecurityVerbName.Configure | SecurityVerbName.Extend | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Assign | SecurityVerbName.Cancel | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Document, SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Email, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.EmailTemplate, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.EndorsedQualification, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Examiner, SecurityVerbName.Delete | SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.ExaminerMarks, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.FinanceOther, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Invoice, SecurityVerbName.Create | SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Language, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Logbook, SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.MaterialRequest, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.Notes | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Organisation, SecurityVerbName.Configure | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.PaidReview, SecurityVerbName.Create);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Panel, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.PanelMember, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.PayRun, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Person, SecurityVerbName.Configure | SecurityVerbName.Create | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.PersonMyNaatiRegistration, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.RolePlayer, SecurityVerbName.Configure | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.RubricResult, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Skill, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.SupplementaryTest, SecurityVerbName.Create);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestAsset, SecurityVerbName.Delete);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestMaterial, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Download | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Upload | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Revert | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Notify | SecurityVerbName.Read | SecurityVerbName.Reject | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestSitting, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update | SecurityVerbName.Validate);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.TestSpecification, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 10, SecurityNounName.Venue, SecurityVerbName.Read | SecurityVerbName.Search);

            //TestInvigilator

            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Application, SecurityVerbName.PreviewEmail | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Credential, SecurityVerbName.Issue);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.CredentialRequest, SecurityVerbName.Issue | SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Document, SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Upload);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Examiner, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.ExaminerMarks, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.FinanceOther, SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Organisation, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.OrganisationHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.PaidReview, SecurityVerbName.Create);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.Person, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.RolePlayer, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.RubricResult, SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.SupplementaryTest, SecurityVerbName.Create);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.TestMaterial, SecurityVerbName.Download | SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.Override | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.TestSession, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 11, SecurityNounName.TestSitting, SecurityVerbName.Manage | SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update | SecurityVerbName.Validate);

            //GeneralAccount

            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Application, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.CertificationPeriod, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Contact, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Credential, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.CredentialRequest, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Email, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.EndorsedQualification, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.ExaminerMarks, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Logbook, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Notes, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Read | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Organisation, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Panel, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.Person, SecurityVerbName.Read | SecurityVerbName.Search | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.PersonHistory, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.RubricResult, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.TestResult, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.TestSession, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 12, SecurityNounName.TestSitting, SecurityVerbName.Read | SecurityVerbName.Search);

            //SystemRole

            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Application, SecurityVerbName.Create | SecurityVerbName.Delete | SecurityVerbName.Manage | SecurityVerbName.PreviewEmail | SecurityVerbName.Reject | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Bill, SecurityVerbName.Approve);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Credential, SecurityVerbName.Issue);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.CredentialRequest, SecurityVerbName.Assess | SecurityVerbName.Create | SecurityVerbName.Issue | SecurityVerbName.Manage | SecurityVerbName.ProcessRefund | SecurityVerbName.RejectRefund | SecurityVerbName.RequestRefund | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Email, SecurityVerbName.Override | SecurityVerbName.Send);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Examiner, SecurityVerbName.Read | SecurityVerbName.Search);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.FinanceOther, SecurityVerbName.Manage | SecurityVerbName.Reject);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.General, SecurityVerbName.Read);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.Invoice, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.MaterialRequest, SecurityVerbName.MarkAsPaid | SecurityVerbName.Notes | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.PaidReview, SecurityVerbName.Create | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.PersonFinanceDetails, SecurityVerbName.Manage);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.RolePlayer, SecurityVerbName.Notify | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.RubricResult, SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.SupplementaryTest, SecurityVerbName.Create | SecurityVerbName.Manage | SecurityVerbName.Withdraw);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.TestResult, SecurityVerbName.Issue | SecurityVerbName.Revert | SecurityVerbName.Update);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.TestSession, SecurityVerbName.Assign | SecurityVerbName.Notify | SecurityVerbName.Reject);
            CreateOrUpdateTableRow(id++, 13, SecurityNounName.TestSitting, SecurityVerbName.Invalidate | SecurityVerbName.Send | SecurityVerbName.Uninvite | SecurityVerbName.Update);

            //ApiAdministrator
            CreateOrUpdateTableRow(id++, 14, SecurityNounName.ApiAdministrator, SecurityVerbName.Create | SecurityVerbName.Update);

        }
    }
}