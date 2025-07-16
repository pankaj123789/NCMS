using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ApplicationForms;
using F1Solutions.Naati.Common.Migrations.NAATIScripts.Data.ScriptGenerators;
using F1Solutions.Naati.Common.Migrations.Updater;

namespace F1Solutions.Naati.Common.Migrations
{
    public class NaatiScriptRunner : ScriptRunner
    {
        private readonly bool _writeConsole;
        public readonly ScriptSEnvironmentName CurrentEnvironment = ScriptSEnvironmentName.Prod;
        

        public NaatiScriptRunner(string connectionString, IDictionary<string, string> dbNameTokenMappings, bool writeConsole)
            : base(connectionString, dbNameTokenMappings)
        {
            _writeConsole = writeConsole;

             var environment = (ScriptSEnvironmentName)Enum.Parse(typeof(ScriptSEnvironmentName),ConfigurationManager.AppSettings["EnvironmentType"] ?? ScriptSEnvironmentName.Dev.ToString());
             CurrentEnvironment = environment;
             LogInfo($"Environment type configured: {CurrentEnvironment}");
        }

        protected override void RunVersionInfoMaintenanceScripts()
        {
            // if you remove a migration from the assembly, add the migration number to RemovedMigrations.txt
            var values = NAATIScripts.Scripts.RemovedMigrations.Trim().Trim(',');
            if (values != string.Empty)
            {
                var script = $"delete from VersionInfo where Version in ({NAATIScripts.Scripts.RemovedMigrations.Trim().Trim(',')})";
                RunScript(script);
            }
        }

        protected override void RunSystemValueMaintenanceScripts()
        {
            // if you remove a system value, add the SystemValueKey to RemovedSystemValues.txt. a migration is not required.
            var values = NAATIScripts.Scripts.RemovedSystemValues.Trim().Trim(',');
            if (values != string.Empty)
            {
                var script = $"delete from SystemValue where SystemValueKey in ({NAATIScripts.Scripts.RemovedSystemValues.Trim().Trim(',')})";
                RunScript(script);
            }
        }
        
        public virtual void RunPreMigrationScripts()
        {
            LogInfo("Running naati script runner pre-migrations.");
        }

        public override void RunPostMigrationScripts()
        {
            RunSystemValueMaintenanceScripts();
            RunVersionInfoMaintenanceScripts();
            
            CreateOrUpdateDbObject("fnGetNextNaatiNumber", "dbo", NAATIScripts.Scripts.fnGetNextNaatiNumber);

            CreateOrUpdateDbObject("PersonSelect", "dbo", NAATIScripts.Scripts.PersonSelect);
            CreateOrUpdateDbObject("PhoneSelect", "dbo", NAATIScripts.Scripts.PhoneSelect);
            CreateOrUpdateDbObject("EmailSelect", "dbo", NAATIScripts.Scripts.EmailSelect);
            CreateOrUpdateDbObject("AddressSelect", "dbo", NAATIScripts.Scripts.AddressSelect);
            //CreateOrUpdateDbObject("RegionSelect", "dbo", NAATIScripts.Scripts.RegionSelect);
            CreateOrUpdateDbObject("NH_EntityInsert", "dbo", NAATIScripts.Scripts.NH_EntityInsert);
            CreateOrUpdateDbObject("RequestJobToken", "dbo", NAATIScripts.Scripts.RequestJobToken);
            CreateOrUpdateDbObject("ReleaseJobToken", "dbo", NAATIScripts.Scripts.ReleaseJobToken);
            CreateOrUpdateDbObject("GetSingleKey", "dbo", NAATIScripts.Scripts.GetSingleKey);
            CreateOrUpdateDbObject("DeletedApplications", "dbo", NAATIScripts.Scripts.DeletedApplications);
            CreateOrUpdateDbObject("LegacyAccreditationMerge", "dbo", NAATIScripts.Scripts.LegacyAccreditationMerge);
            CreateOrUpdateDbObject("RevertTestResult", "dbo", NAATIScripts.Scripts.RevertTestResult);
            CreateOrUpdateDbObject("RebuildIndexes", "dbo", NAATIScripts.Scripts.RebuildIndexes);
            CreateOrUpdateDbObject("PrerequisiteApplications", "dbo", NAATIScripts.Scripts.PrerequisiteApplications);
            CreateOrUpdateDbObject("PrerequisiteApplicationsNullableApplications", "dbo", NAATIScripts.Scripts.PrerequisiteApplicationsNullableApplications);
            CreateOrUpdateDbObject("GetStoredFilesForFileDeletion", "dbo", NAATIScripts.Scripts.GetStoredFilesForFileDeletion);
            CreateOrUpdateDbObject("PrerequisiteSummary", "dbo", NAATIScripts.Scripts.PrerequisiteSummary);
            CreateOrUpdateDbObject("GetRemainingStoredFilesForDeletionCount", "dbo", NAATIScripts.Scripts.GetRemainingStoredFilesForDeletionCount);
            //CreateOrUpdateDbObject("GetNewApplicationsThatCanProgressToEligibleForTesting", "dbo", NAATIScripts.Scripts.GetNewApplicationsThatCanProgressToEligibleForTesting);
            CreateOrUpdateDbObject("GetNewCredentialsThatCanProgressToEligibleForTesting", "dbo", NAATIScripts.Scripts.GetNewCredentialsThatCanProgressToEligibleForTesting);
            CreateOrUpdateDbObject("CheckNewCredentialSiblingsCanProgressToEligibleForTesting", "dbo", NAATIScripts.Scripts.CheckNewCredentialSiblingsCanProgressToEligibleForTesting);
            CreateOrUpdateDbObject("SupplementaryMaterialsFilterTestComponentsThatPassed", "dbo", NAATIScripts.Scripts.SupplementaryMaterialsFilterTestComponentsThatPassed);

            CreateOrUpdateDbObject("vwMaxPersonNameEffectiveDate", "dbo", NAATIScripts.Scripts.vwMaxPersonNameEffectiveDate);
            CreateOrUpdateDbObject("vwDistinctPersonName", "dbo", NAATIScripts.Scripts.vwDistinctPersonName);
            CreateOrUpdateDbObject("vwPersonDistinct", "dbo", NAATIScripts.Scripts.vwPersonDistinct);
            CreateOrUpdateDbObject("vwDistinctInstitutionName", "dbo", NAATIScripts.Scripts.vwDistinctInstitutionName);
            CreateOrUpdateDbObject("vwJobExaminerPayrollStatus", "dbo", NAATIScripts.Scripts.vwJobExaminerPayrollStatus);
            CreateOrUpdateDbObject("vwMarkingsForPayroll", "dbo", NAATIScripts.Scripts.vwMarkingsForPayroll);
            CreateOrUpdateDbObject("vwTestStatus", "dbo", NAATIScripts.Scripts.vwTestStatus);
            CreateOrUpdateDbObject("vwSkillDisplayName", "dbo", NAATIScripts.Scripts.vwSkillDisplayName);
            CreateOrUpdateDbObject("vwRolePlayerLastTestSession", "dbo", NAATIScripts.Scripts.vwRolePlayerLastTestSession);
            CreateOrUpdateDbObject("vwTestMaterialLastUsed", "dbo", NAATIScripts.Scripts.vwTestMaterialLastUsed);
            CreateOrUpdateDbObject("vwTestMaterialRequestRoundLatest", "dbo", NAATIScripts.Scripts.vwTestMaterialRequestRoundLatest);
            CreateOrUpdateDbObject("vwIssuedCredentialCredentialRequest", "dbo", NAATIScripts.Scripts.vwIssuedCredentialCredentialRequest);
            CreateOrUpdateDbObject("vwTestMaterialCreationPayments", "dbo", NAATIScripts.Scripts.vwTestMaterialCreationPayments);

            RunScript(NAATIScripts.Scripts.FT_Audit);

            var scriptGenerators = new IScriptGenerator[] {
                new SkillTypeScriptGenerator(this),
                new SystemActionTypeScriptGenerator(this),
                new EmailTemplateScriptGenerator(this),
                new LanguageGroupScriptGenerator(this),
                new LanguageScriptGenerator(this),
                new SkillScriptGenerator(this),
                new ProductTypeScriptGenerator(this),
                new ProductCategoryScriptGenerator(this),
                new GlCodeScriptGenerator(this),
                new ProductSpecificationScriptGenerator(this),
                new CredentialApplicationTypeCategoryScriptGenerator(this),
                new CredentialApplicationTypeScriptGenerator(this),
                new SystemActionEventTypeScriptGenerator(this),
                new EmailTemplateDetailTypeScriptGenerator(this),
                new SystemEmailTemplateBuilderScriptGenerator(this), 
                new CredentialCategoryScriptGenerator(this),
                new CredentialTypeScriptGenerator(this),
                new CredentialApplicationRefundPolicyScriptGenerator(this),
                new CredentialFeeProductScriptGenerator(this),
                new FeeTypeScriptGenerator(this),
                new CredentialRequestStatusTypeScriptGenerator(this),
                new CredentialApplicationStatusTypeScriptGenerator(this),
                new DataTypeScriptGenerator(this),
                new CredentialApplicationFieldCategoryScriptGenerator(this), 
                new CredentialApplicationFieldScriptGenerator(this),
                new CredentialApplicationFieldOptionScriptGenerator(this),
                new CredentialApplicationFieldOptionOptionScriptGenerator(this),
                new CredentialApplicationTypeCredentialTypeScriptGenerator(this),
                new DocumentTypeCategoryScriptGenerator(this),
                new VenueScriptGenerator(this),
                new DocumentTypeScriptGenerator(this),
                new CredentialApplicationTypeDocumentTypeScriptGenerator(this),
                new TestStatusTypeScriptGenerator(this),
                new ExaminerStatusTypeScriptGenerator(this),
                new ResultTypeResultGenerator(this),
                new TestComponentBaseTypeScriptGenerator(this),
                new CredentialStatusTypeScriptGenerator(this),
                new CredentialRequestPathTypeScriptGenerator(this),
                new MarkingResultTypeScriptGenerator(this),
                new CredentialTypeUpgradePathScriptGenerator(this),
                new CredentialTypeCrossSkillScriptGenerator(this),
                new CountryScriptGenerator(this),
                new PdPointsLimitTypeScriptGenerator(this),
                new ProfessionalDevelopmentSectionScriptGenerator(this),
                new RefundMethodTypeScriptGenerator(this),
                new ProfessionalDevelopmentCategoryGroupScriptGenerator(this),
                new ProfessionalDevelopmentCategoryScriptGenerator(this),
                new ProfessionalDevelopmentSectionCategoryScriptGenerator(this),
                new ProfessionalDevelopmentRequirementScriptGenerator(this),
                new ProfessionalDevelopmentCategoryRequirementScriptGenerator(this),
                new StateScriptGenerator(this),
                new ODAddressVisibilityTypeScriptGenerator(this),
                new TestLocationScriptGenerator(this),
                new CredentialApplicationTypeTestLocationScriptGenerator(this),
                new SystemValueScriptGenerator(this),
                new PanelTypeScriptGenerator(this),
                new CredentialRequestAssociationTypeScriptGenerator(this),
                new CredentialTypeTemplateScriptGenerator(this),
                new PanelRoleCategoryScriptGenerator(this),
                new PanelRoleScriptGenerator(this),
                new RolePlayerRoleTypeGenerator(this),
                new RolePlayerStatusTypeGenerator(this),
                new TestResultEligibilityTypeScriptGenerator(this),
                new FormsScriptGenerator(this),
                new EmailSendStatusTypeScriptGenerator(this),
                new TestMaterialTypeScriptGenerator(this),
                new TestMaterialLinkTypeScriptGenerator(this),
                new MaterialRequestRoundPanelMembershipTypeScriptGenerator(this),
                new MaterialRequestStatusTypeScripGenerator(this),
                new MaterialRequestRoundStatusTypeScriptGenerator(this),
                new MaterialRequestTaskTypeScriptGenerator(this),
                new TestMaterialDomainScriptGenerator(this),
                new CredentialTypeTestMaterialDomainScriptGenerator(this),
                new SecurityRoleScriptGenerator(this),
                new SecurityNounScriptGenerator(this),
                new SecurityRuleScriptGenerator(this),
                new DocumentTypeRoleScriptGenerator(this),
                new RefundMethodTypeScriptGenerator(this),
                new ExternalAccountingOperationTypeScriptGenerator(this),
                new NotificationTypeScriptGenerator(this),
                new RefundPolicyParameterScriptGenerator(this),
                new PaymentmethodScriptGenerator(this),
                new CredentialPreRequisiteScriptGenerator(this),
                new CredentialTypeDowngradePathScriptGenerator(this),
                new StoredFileStatusTypeScriptGenerator(this),
                new StoredFileDeletePolicyScriptGenerator(this),
                new StoredFileDeletePolicyDocumentTypeScriptGenerator(this)
            };


            foreach (var scriptGenerator in scriptGenerators)
            {
                LogInfo($"Running {scriptGenerator.GetType().Name}");
                scriptGenerator.RunScripts();
            }

            foreach (var scriptGenerator in scriptGenerators.Reverse())
            {
                LogInfo($"Running Reverse {scriptGenerator.GetType().Name}");
                scriptGenerator.RunDescendantOrderScripts();
            }
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