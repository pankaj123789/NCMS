using System;
using System.Collections.Generic;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Migrations.Updater;

namespace F1Solutions.Naati.Common.Migrations
{
    public class ReportsScriptRunner : ScriptRunner 
    {
        private readonly bool _writeConsole;

        public ReportsScriptRunner(string connectionString, IDictionary<string, string> dbNameTokenMappings, bool writeConsole)
            : base(connectionString, dbNameTokenMappings)
        {
            _writeConsole = writeConsole;
        }

        public override void RunPostMigrationScripts()
        {
            // Functions
            CreateOrUpdateDbObject("GetDates", "dbo", ReportsScripts.Scripts.GetDates);
            CreateOrUpdateDbObject("GetPhoneNumber", "dbo", ReportsScripts.Scripts.GetPhoneNumber);
            CreateOrUpdateDbObject("GetExaminerComponentMarks", "dbo", ReportsScripts.Scripts.GetExaminerComponentMarks);
            CreateOrUpdateDbObject("GetExaminerOverallMark", "dbo", ReportsScripts.Scripts.GetExaminerOverallMark);
            CreateOrUpdateDbObject("GetTestResultOverallMark", "dbo", ReportsScripts.Scripts.GetTestResultOverallMark);
            CreateOrUpdateDbObject("GetSystemValue", "dbo", ReportsScripts.Scripts.GetSystemValue);

            // Views // add new views credentialRequests, and Credentials
            CreateOrUpdateDbObject("Application", "dbo", ReportsScripts.Scripts.ApplicationView);
            CreateOrUpdateDbObject("Person", "dbo", ReportsScripts.Scripts.PersonView);
            CreateOrUpdateDbObject("Test", "dbo", ReportsScripts.Scripts.TestView);
            CreateOrUpdateDbObject("Mark", "dbo", ReportsScripts.Scripts.MarkView);
            CreateOrUpdateDbObject("Panel", "dbo", ReportsScripts.Scripts.PanelView);
            CreateOrUpdateDbObject("PanelMembers", "dbo", ReportsScripts.Scripts.PanelMembersView);
            CreateOrUpdateDbObject("TestResult", "dbo", ReportsScripts.Scripts.TestResultView);
            CreateOrUpdateDbObject("CredentialRequests", "dbo", ReportsScripts.Scripts.CredentialRequestsView);
            CreateOrUpdateDbObject("Credentials", "dbo", ReportsScripts.Scripts.CredentialsView);
            CreateOrUpdateDbObject("Organisation", "dbo", ReportsScripts.Scripts.OrganisationView);
            CreateOrUpdateDbObject("OrganisationContacts", "dbo", ReportsScripts.Scripts.OrganisationContactsView);
            CreateOrUpdateDbObject("ApplicationCustomFields", "dbo", ReportsScripts.Scripts.ApplicationCustomFieldsView);
            CreateOrUpdateDbObject("TestSessions", "dbo", ReportsScripts.Scripts.TestSessionsView);
            CreateOrUpdateDbObject("RubricTestComponentResult", "dbo", ReportsScripts.Scripts.RubricTestComponentResultView);
            CreateOrUpdateDbObject("TestRubricComponent", "dbo", ReportsScripts.Scripts.TestRubricComponentView);
            CreateOrUpdateDbObject("MarkRubric", "dbo", ReportsScripts.Scripts.MarkRubricView);
            CreateOrUpdateDbObject("TestResultRubric", "dbo", ReportsScripts.Scripts.TestResultRubricView);
            CreateOrUpdateDbObject("TestSittingTestMaterial", "dbo", ReportsScripts.Scripts.TestSittingTestMaterialView);
            CreateOrUpdateDbObject("TestSessionRolePlayer", "dbo", ReportsScripts.Scripts.TestSessionRolePlayerView);
            CreateOrUpdateDbObject("TestSessionRolePlayerDetails", "dbo", ReportsScripts.Scripts.TestSessionRolePlayerDetailsView);

            CreateOrUpdateDbObject("ProfessionalDevelopment", "dbo", ReportsScripts.Scripts.ProfessionalDevelopment);
            CreateOrUpdateDbObject("WorkPractice", "dbo", ReportsScripts.Scripts.WorkPractice);
            CreateOrUpdateDbObject("PanelMembershipCredentialTypes", "dbo", ReportsScripts.Scripts.PanelMembershipCredentialTypesView);
            CreateOrUpdateDbObject("ExaminerJob", "dbo", ReportsScripts.Scripts.ExaminerJobView);

            CreateOrUpdateDbObject("MaterialRequest", "dbo", ReportsScripts.Scripts.MaterialRequest);
            CreateOrUpdateDbObject("MaterialRequestRound", "dbo", ReportsScripts.Scripts.MaterialRequestRound);
            CreateOrUpdateDbObject("MaterialRequestPanelMember", "dbo", ReportsScripts.Scripts.MaterialRequestPanelMember);
            CreateOrUpdateDbObject("MaterialRequestTaskHours", "dbo", ReportsScripts.Scripts.MaterialRequestTaskHours);
            CreateOrUpdateDbObject("MaterialRequestPayroll", "dbo", ReportsScripts.Scripts.MaterialRequestPayroll);
            CreateOrUpdateDbObject("TestMaterial", "dbo", ReportsScripts.Scripts.TestMaterialView);
            CreateOrUpdateDbObject("EndorsedQualification", "dbo", ReportsScripts.Scripts.EndorsedQualificationView);

            // Stored Procedures // add new procedures credentialRequests, and Credentials
            CreateOrUpdateDbObject("ReportingSnapshot_Application", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Application);
            CreateOrUpdateDbObject("ReportingSnapshot_Person", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Person);
            CreateOrUpdateDbObject("ReportingSnapshot_Test", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Test);
            CreateOrUpdateDbObject("ReportingSnapshot_Mark", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Mark);
            CreateOrUpdateDbObject("ReportingSnapshot_Panel", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Panel);
            CreateOrUpdateDbObject("ReportingSnapshot_PanelMembers", "dbo", ReportsScripts.Scripts.ReportingSnapshot_PanelMembers);
            CreateOrUpdateDbObject("ReportingSnapshot_TestResult", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestResult);
            CreateOrUpdateDbObject("ReportingSnapshot_CredentialRequests", "dbo", ReportsScripts.Scripts.ReportingSnapshot_CredentialRequests);
            CreateOrUpdateDbObject("ReportingSnapshot_Credentials", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Credentials);
            CreateOrUpdateDbObject("ReportingSnapshot_Organisation", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Organisation);
            CreateOrUpdateDbObject("ReportingSnapshot_OrganisationContacts", "dbo", ReportsScripts.Scripts.ReportingSnapshot_OrganisationContacts);
            CreateOrUpdateDbObject("ReportingSnapshot_ApplicationCustomFIelds", "dbo", ReportsScripts.Scripts.ReportingSnapshot_ApplicationCustomFIelds);
            CreateOrUpdateDbObject("ReportingSnapshot_TestSessions", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestSessions);
            CreateOrUpdateDbObject("ReportingSnapshot_TestResultRubric", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestResultRubric);
            CreateOrUpdateDbObject("ReportingSnapshot_MarkRubric", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MarkRubric);
            CreateOrUpdateDbObject("ReportingSnapshot_TestSittingTestMaterial", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestSittingTestMaterial);

            CreateOrUpdateDbObject("ReportingSnapshot_ProfessionalDevelopment", "dbo", ReportsScripts.Scripts.ReportingSnapshot_ProfessionalDevelopment);
            CreateOrUpdateDbObject("ReportingSnapshot_WorkPractice", "dbo", ReportsScripts.Scripts.ReportingSnapshot_WorkPractice);
            CreateOrUpdateDbObject("ReportingSnapshot_TestSessionRolePlayer", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestSessionRolePlayer);
            CreateOrUpdateDbObject("ReportingSnapshot_TestSessionRolePlayerDetails", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestSessionRolePlayerDetails);
            CreateOrUpdateDbObject("ReportingSnapshot_PanelMembershipCredentialTypes", "dbo", ReportsScripts.Scripts.ReportingSnapshot_PanelMembershipCredentialTypes);
            CreateOrUpdateDbObject("ReportingSnapshop_ExaminerJob", "dbo", ReportsScripts.Scripts.ReportingSnapshop_ExaminerJob);

            CreateOrUpdateDbObject("ReportingSnapshot_MaterialRequest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MaterialRequest);
            CreateOrUpdateDbObject("ReportingSnapshot_MaterialRequestRound", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MaterialRequestRound);
            CreateOrUpdateDbObject("ReportingSnapshot_MaterialRequestPanelMember", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MaterialRequestPanelMember);
            CreateOrUpdateDbObject("ReportingSnapshot_MaterialRequestTaskHours", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MaterialRequestTaskHours);
            CreateOrUpdateDbObject("ReportingSnapshop_MaterialRequestPayroll", "dbo", ReportsScripts.Scripts.ReportingSnapshop_MaterialRequestPayroll);
            CreateOrUpdateDbObject("ReportingSnapshop_TestMaterial", "dbo", ReportsScripts.Scripts.ReportingSnapshop_TestMaterial);
            CreateOrUpdateDbObject("ReportingSnapshot_Main", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Main);
            CreateOrUpdateDbObject("ReportingSnapshop_EndorsedQualification", "dbo", ReportsScripts.Scripts.ReportingSnapshop_EndorsedQualification);

            CreateOrUpdateDbObject("ReportingSnapshot_TestResultExaminerRubricMarkingHistory", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestResultExaminerRubricMarkingHistory);
            CreateOrUpdateDbObject("ReportingSnapshot_TestResultExaminerStandardMarkingHistory", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestResultExaminerStandardMarkingHistory);

            CreateOrUpdateDbObject("LogExecutionInfo", "dbo", ReportsScripts.Scripts.LogExecutionInfo);
            CreateOrUpdateDbObject("LogExecutionWarning", "dbo", ReportsScripts.Scripts.LogExecutionWarning);
            CreateOrUpdateDbObject("LogExecutionError", "dbo", ReportsScripts.Scripts.LogExecutionError);

            CreateOrUpdateDbObject("ReportingSnapshot_Runner", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Runner);

            //changes to support latest
            CreateOrUpdateDbObject("ReportingSnapshot_ApplicationCustomFIelds_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_ApplicationCustomFIelds_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_Application_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Application_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_CredentialRequests_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_CredentialRequests_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_MarkRubric_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_MarkRubric_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_Mark_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Mark_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_TestResult_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestResult_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_TestSessions_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_TestSessions_Latest);
            CreateOrUpdateDbObject("ReportingSnapshot_Test_Latest", "dbo", ReportsScripts.Scripts.ReportingSnapshot_Test_Latest);

        }

        private void LogInfo(string info)
        {
            LoggingHelper.LogInfo(info);
            if (_writeConsole)
            {
                Console.WriteLine(info);
            }
        }
    }
}
