using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191202_UpdateAllReportingTableForCloudMigration
{
    [NaatiMigration(201912021047)]
    public class UpdateAllReportingTableForCloudMigration: NaatiMigration
    {
        public override void Up()
        {
            //OrganisationContactsHistory--------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE OrganisationContactsHistory DROP CONSTRAINT PK_OrganisationContactsHistory");

            Create.Column("RowStatus").OnTable("OrganisationContactsHistory").AsString(50).Nullable();
            Execute.Sql("Update OrganisationContactsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update OrganisationContactsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update OrganisationContactsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("OrganisationContactsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("OrganisationContactsHistory");
            Delete.Column("DeletedDate").FromTable("OrganisationContactsHistory");

            Create.Column("Inactive").OnTable("OrganisationContactsHistory").AsBoolean().Nullable();
            Execute.Sql("Update OrganisationContactsHistory Set Inactive = 0");
            Alter.Column("Inactive").OnTable("OrganisationContactsHistory").AsBoolean().NotNullable();

            Execute.Sql(@"ALTER TABLE OrganisationContactsHistory ADD CONSTRAINT PK_OrganisationContactsHistory PRIMARY KEY (ContactPersonId,ModifiedDate,RowStatus);");

            //PanelHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE PanelHistory DROP CONSTRAINT PanelId_ModifiedDate_PK
                          DROP INDEX IF EXISTS IX_Panel_ObsoletedDate ON PanelHistory");

            Create.Column("RowStatus").OnTable("PanelHistory").AsString(50).Nullable();
            Execute.Sql("Update PanelHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update PanelHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update PanelHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("PanelHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("PanelHistory");
            Delete.Column("DeletedDate").FromTable("PanelHistory");

            Execute.Sql(@"ALTER TABLE PanelHistory ADD CONSTRAINT PK_PanelHistory PRIMARY KEY (PanelId,ModifiedDate,RowStatus)");

            //OrganisationHistory----------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE OrganisationHistory DROP CONSTRAINT PK_OrganisationHistory");

            Create.Column("RowStatus").OnTable("OrganisationHistory").AsString(50).Nullable();
            Execute.Sql("Update OrganisationHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update OrganisationHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update OrganisationHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("OrganisationHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("OrganisationHistory");
            Delete.Column("DeletedDate").FromTable("OrganisationHistory");

            Execute.Sql(@"ALTER TABLE OrganisationHistory ADD CONSTRAINT PK_OrganisationHistory PRIMARY KEY (OrganisationId,ModifiedDate,RowStatus)");

            //ApplicationHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE ApplicationHistory DROP CONSTRAINT PK_Application");

            Create.Column("RowStatus").OnTable("ApplicationHistory").AsString(50).Nullable();
            Execute.Sql("Update ApplicationHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update ApplicationHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update ApplicationHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("ApplicationHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("ApplicationHistory");
            Delete.Column("DeletedDate").FromTable("ApplicationHistory");

            Execute.Sql(@"ALTER TABLE ApplicationHistory ADD CONSTRAINT PK_Application PRIMARY KEY (ApplicationId,ModifiedDate,RowStatus)");

            //CredentialRequestsHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE CredentialRequestsHistory DROP CONSTRAINT PK_CredentialRequestsHistory");

            Create.Column("RowStatus").OnTable("CredentialRequestsHistory").AsString(50).Nullable();
            Execute.Sql("Update CredentialRequestsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update CredentialRequestsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update CredentialRequestsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("CredentialRequestsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("CredentialRequestsHistory");
            Delete.Column("DeletedDate").FromTable("CredentialRequestsHistory");

            Execute.Sql(@"ALTER TABLE CredentialRequestsHistory ADD CONSTRAINT PK_CredentialRequestsHistory PRIMARY KEY (CredentialRequestId,ModifiedDate,RowStatus)");

            //CredentialsHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE CredentialsHistory DROP CONSTRAINT PK_CredentialsHistory");

            Create.Column("RowStatus").OnTable("CredentialsHistory").AsString(50).Nullable();
            Execute.Sql("Update CredentialsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update CredentialsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update CredentialsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("CredentialsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("CredentialsHistory");
            Delete.Column("DeletedDate").FromTable("CredentialsHistory");

            Execute.Sql(@"ALTER TABLE CredentialsHistory ADD CONSTRAINT PK_CredentialsHistory PRIMARY KEY (CredentialId,ModifiedDate,RowStatus)");

            //ExaminerJobHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE ExaminerJobHistory DROP CONSTRAINT PK_ExaminerJobHistory");

            Create.Column("RowStatus").OnTable("ExaminerJobHistory").AsString(50).Nullable();
            Execute.Sql("Update ExaminerJobHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update ExaminerJobHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update ExaminerJobHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("ExaminerJobHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("ExaminerJobHistory");
            Delete.Column("DeletedDate").FromTable("ExaminerJobHistory");

            Execute.Sql(@"ALTER TABLE ExaminerJobHistory ADD CONSTRAINT PK_ExaminerJobHistory PRIMARY KEY (JobExaminerId,ModifiedDate,RowStatus)");

            //MarkHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MarkHistory DROP CONSTRAINT ModifiedDate_Identifier_PK");

            Create.Column("RowStatus").OnTable("MarkHistory").AsString(50).Nullable();
            Execute.Sql("Update MarkHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MarkHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MarkHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MarkHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MarkHistory");
            Delete.Column("DeletedDate").FromTable("MarkHistory");

            Execute.Sql(@"ALTER TABLE MarkHistory ADD CONSTRAINT ModifiedDate_Identifier_PK PRIMARY KEY (MarkId,ModifiedDate,RowStatus)");

            //MarkRubricHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MarkRubricHistory DROP CONSTRAINT PK_MarkRubricHistory");

            Create.Column("RowStatus").OnTable("MarkRubricHistory").AsString(50).Nullable();
            Execute.Sql("Update MarkRubricHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MarkRubricHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MarkRubricHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MarkRubricHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MarkRubricHistory");
            Delete.Column("DeletedDate").FromTable("MarkRubricHistory");

            Execute.Sql(@"ALTER TABLE MarkRubricHistory ADD CONSTRAINT PK_MarkRubricHistory PRIMARY KEY (TestResultId,TestComponentId,JobExaminerId,RubricAssessementCriterionResultId,ModifiedDate,RowStatus)");

            //MaterialRequestHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MaterialRequestHistory DROP CONSTRAINT PK_MaterialRequestHistory");

            Create.Column("RowStatus").OnTable("MaterialRequestHistory").AsString(50).Nullable();
            Execute.Sql("Update MaterialRequestHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MaterialRequestHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MaterialRequestHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MaterialRequestHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MaterialRequestHistory");
            Delete.Column("DeletedDate").FromTable("MaterialRequestHistory");

            Execute.Sql(@"ALTER TABLE MaterialRequestHistory ADD CONSTRAINT PK_MaterialRequestHistory PRIMARY KEY (MaterialRequestId,ModifiedDate,RowStatus)");

            Execute.Sql(@"EXEC sp_rename '[MaterialRequestHistory].[CreatedBy ]', 'CreatedBy', 'COLUMN';
                          EXEC sp_rename '[MaterialRequestHistory].[OwnedBy ]', 'OwnedBy', 'COLUMN';");

            //MaterialRequestPanelMemberHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MaterialRequestPanelMemberHistory DROP CONSTRAINT PK_MaterialRequestPanelMemberHistory");

            Create.Column("RowStatus").OnTable("MaterialRequestPanelMemberHistory").AsString(50).Nullable();
            Execute.Sql("Update MaterialRequestPanelMemberHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MaterialRequestPanelMemberHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MaterialRequestPanelMemberHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MaterialRequestPanelMemberHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MaterialRequestPanelMemberHistory");
            Delete.Column("DeletedDate").FromTable("MaterialRequestPanelMemberHistory");

            Execute.Sql(@"ALTER TABLE MaterialRequestPanelMemberHistory ADD CONSTRAINT PK_MaterialRequestPanelMemberHistory PRIMARY KEY (MaterialRequestPanelMembershipId,ModifiedDate,RowStatus)");

            //MaterialRequestPayrollHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MaterialRequestPayrollHistory DROP CONSTRAINT PK_MaterialRequestPayrollHistory");

            Create.Column("RowStatus").OnTable("MaterialRequestPayrollHistory").AsString(50).Nullable();
            Execute.Sql("Update MaterialRequestPayrollHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MaterialRequestPayrollHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MaterialRequestPayrollHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MaterialRequestPayrollHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MaterialRequestPayrollHistory");
            Delete.Column("DeletedDate").FromTable("MaterialRequestPayrollHistory");

            Execute.Sql(@"ALTER TABLE MaterialRequestPayrollHistory ADD CONSTRAINT PK_MaterialRequestPayrollHistory PRIMARY KEY (MaterialRequestPayrollId,ModifiedDate,RowStatus)");

            //MaterialRequestRoundHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MaterialRequestRoundHistory DROP CONSTRAINT PK_MaterialRequestRoundHistory");

            Create.Column("RowStatus").OnTable("MaterialRequestRoundHistory").AsString(50).Nullable();
            Execute.Sql("Update MaterialRequestRoundHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MaterialRequestRoundHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MaterialRequestRoundHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MaterialRequestRoundHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MaterialRequestRoundHistory");
            Delete.Column("DeletedDate").FromTable("MaterialRequestRoundHistory");

            Execute.Sql(@"ALTER TABLE MaterialRequestRoundHistory ADD CONSTRAINT PK_MaterialRequestRoundHistory PRIMARY KEY (MaterialRequestRoundId,ModifiedDate,RowStatus)");


            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //MaterialRequestTaskHoursHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE MaterialRequestTaskHoursHistory DROP CONSTRAINT PK_MaterialRequestTaskHoursHistory");

            Create.Column("RowStatus").OnTable("MaterialRequestTaskHoursHistory").AsString(50).Nullable();
            Execute.Sql("Update MaterialRequestTaskHoursHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update MaterialRequestTaskHoursHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update MaterialRequestTaskHoursHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("MaterialRequestTaskHoursHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("MaterialRequestTaskHoursHistory");
            Delete.Column("DeletedDate").FromTable("MaterialRequestTaskHoursHistory");

            Execute.Sql(@"ALTER TABLE MaterialRequestTaskHoursHistory ADD CONSTRAINT PK_MaterialRequestTaskHoursHistory PRIMARY KEY (MaterialRequestPanelMembershipTaskId,ModifiedDate,RowStatus)");

            //PanelMembershipCredentialTypesHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE PanelMembershipCredentialTypesHistory DROP CONSTRAINT PK_PanelMembershipCredentialTypesHistory");

            Create.Column("RowStatus").OnTable("PanelMembershipCredentialTypesHistory").AsString(50).Nullable();
            Execute.Sql("Update PanelMembershipCredentialTypesHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update PanelMembershipCredentialTypesHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update PanelMembershipCredentialTypesHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("PanelMembershipCredentialTypesHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("PanelMembershipCredentialTypesHistory");
            Delete.Column("DeletedDate").FromTable("PanelMembershipCredentialTypesHistory");

            Execute.Sql(@"ALTER TABLE PanelMembershipCredentialTypesHistory ADD CONSTRAINT PK_PanelMembershipCredentialTypesHistory PRIMARY KEY (PanelMemberShipCredentialTypeId,ModifiedDate,RowStatus)");

            //PanelMembersHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE PanelMembersHistory DROP CONSTRAINT PanelMembersId_PanelId_PK
                          DROP INDEX IF EXISTS IX_PanelMembers_ObsoletedDate ON PanelMembersHistory");

            Create.Column("RowStatus").OnTable("PanelMembersHistory").AsString(50).Nullable();
            Execute.Sql("Update PanelMembersHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update PanelMembersHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update PanelMembersHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("PanelMembersHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("PanelMembersHistory");
            Delete.Column("DeletedDate").FromTable("PanelMembersHistory");

            Execute.Sql(@"ALTER TABLE PanelMembersHistory ADD CONSTRAINT PK_PanelMembersHistory PRIMARY KEY (PanelMembersId,ModifiedDate,RowStatus)");

            //PersonHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE PersonHistory DROP CONSTRAINT PK_whPerson
                          DROP INDEX IF EXISTS IX_Person_ObsoletedDate ON PersonHistory");

            Create.Column("RowStatus").OnTable("PersonHistory").AsString(50).Nullable();
            Execute.Sql("Update PersonHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update PersonHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update PersonHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("PersonHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("PersonHistory");
            Delete.Column("DeletedDate").FromTable("PersonHistory");

            Execute.Sql(@"ALTER TABLE PersonHistory ADD CONSTRAINT PK_whPerson PRIMARY KEY (PersonId,ModifiedDate,RowStatus)");

            //ProfessionalDevelopmentHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE ProfessionalDevelopmentHistory DROP CONSTRAINT PK_ProfessionalDevelopmentHistory");

            Create.Column("RowStatus").OnTable("ProfessionalDevelopmentHistory").AsString(50).Nullable();
            Execute.Sql("Update ProfessionalDevelopmentHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update ProfessionalDevelopmentHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update ProfessionalDevelopmentHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("ProfessionalDevelopmentHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("ProfessionalDevelopmentHistory");
            Delete.Column("DeletedDate").FromTable("ProfessionalDevelopmentHistory");

            Execute.Sql(@"ALTER TABLE ProfessionalDevelopmentHistory ADD CONSTRAINT PK_ProfessionalDevelopmentHistory PRIMARY KEY (ProfessionalDevelopmentActivityId,PersonId,CustomerNumber,PractitionerNumber,ApplicationID,CertificationPeriodID,SectionID,CategoryID,RequirementID,ModifiedDate,RowStatus)");

            //TestHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestHistory DROP CONSTRAINT Uniq_Test");
            Create.Column("RowStatus").OnTable("TestHistory").AsString(50).Nullable();
            Execute.Sql("Update TestHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestHistory");
            Delete.Column("DeletedDate").FromTable("TestHistory");

            Execute.Sql(@"ALTER TABLE TestHistory ADD  CONSTRAINT Uniq_Test UNIQUE NONCLUSTERED 
                        (
	                        TestSittingId ASC,
	                        CredentialRequestId ASC,
	                        TestResultId ASC,
	                        ModifiedDate ASC,
                            RowStatus ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");

            //TestMaterialHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestMaterialHistory DROP CONSTRAINT PK_TestMaterialHistory");

            Create.Column("RowStatus").OnTable("TestMaterialHistory").AsString(50).Nullable();
            Execute.Sql("Update TestMaterialHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestMaterialHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestMaterialHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestMaterialHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestMaterialHistory");
            Delete.Column("DeletedDate").FromTable("TestMaterialHistory");

            Execute.Sql(@"ALTER TABLE TestMaterialHistory ADD CONSTRAINT PK_TestMaterialHistory PRIMARY KEY (TestMaterialId,ModifiedDate,RowStatus)");

            //TestResultHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestResultHistory DROP CONSTRAINT PK_TestResultHistory");

            Create.Column("RowStatus").OnTable("TestResultHistory").AsString(50).Nullable();
            Execute.Sql("Update TestResultHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestResultHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestResultHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestResultHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestResultHistory");
            Delete.Column("DeletedDate").FromTable("TestResultHistory");

            Execute.Sql(@"ALTER TABLE TestResultHistory ADD CONSTRAINT PK_TestResultHistory PRIMARY KEY (TestResultId,ModifiedDate,RowStatus)");

            //TestResultRubricHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestResultRubricHistory DROP CONSTRAINT PK_TestResultRubricHistory");

            Create.Column("RowStatus").OnTable("TestResultRubricHistory").AsString(50).Nullable();
            Execute.Sql("Update TestResultRubricHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestResultRubricHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestResultRubricHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestResultRubricHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestResultRubricHistory");
            Delete.Column("DeletedDate").FromTable("TestResultRubricHistory");

            Execute.Sql(@"ALTER TABLE TestResultRubricHistory ADD CONSTRAINT PK_TestResultRubricHistory PRIMARY KEY (TestResultId,ModifiedDate,RowStatus)");

            //TestSessionRolePlayerDetailsHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestSessionRolePlayerDetailsHistory DROP CONSTRAINT PK_TestSessionRolePlayerDetailsHistory");

            Create.Column("RowStatus").OnTable("TestSessionRolePlayerDetailsHistory").AsString(50).Nullable();
            Execute.Sql("Update TestSessionRolePlayerDetailsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestSessionRolePlayerDetailsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestSessionRolePlayerDetailsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestSessionRolePlayerDetailsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestSessionRolePlayerDetailsHistory");
            Delete.Column("DeletedDate").FromTable("TestSessionRolePlayerDetailsHistory");

            Execute.Sql(@"ALTER TABLE TestSessionRolePlayerDetailsHistory ADD CONSTRAINT PK_TestSessionRolePlayerDetailsHistory PRIMARY KEY (TestSessionRolePlayerDetailId,PersonId,ModifiedDate,RowStatus)");

            //TestSessionRolePlayerHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestSessionRolePlayerHistory DROP CONSTRAINT PK_TestSessionRolePlayerHistory");

            Create.Column("RowStatus").OnTable("TestSessionRolePlayerHistory").AsString(50).Nullable();
            Execute.Sql("Update TestSessionRolePlayerHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestSessionRolePlayerHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestSessionRolePlayerHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestSessionRolePlayerHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestSessionRolePlayerHistory");
            Delete.Column("DeletedDate").FromTable("TestSessionRolePlayerHistory");

            Execute.Sql(@"ALTER TABLE TestSessionRolePlayerHistory ADD CONSTRAINT PK_TestSessionRolePlayerHistory PRIMARY KEY (TestSessionRolePlayerId,PersonId,ModifiedDate,RowStatus)");


            //TestSessionsHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestSessionsHistory DROP CONSTRAINT PK_TestSessionsHistory");

            Create.Column("RowStatus").OnTable("TestSessionsHistory").AsString(50).Nullable();
            Execute.Sql("Update TestSessionsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestSessionsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestSessionsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestSessionsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestSessionsHistory");
            Delete.Column("DeletedDate").FromTable("TestSessionsHistory");

            Execute.Sql(@"ALTER TABLE TestSessionsHistory ADD CONSTRAINT PK_TestSessionsHistory PRIMARY KEY (TestSittingId,TestSessionId,ModifiedDate,RowStatus)");

            //TestSittingTestMaterialHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE TestSittingTestMaterialHistory DROP CONSTRAINT PK_TestSittingTestMaterialHistory");

            Create.Column("RowStatus").OnTable("TestSittingTestMaterialHistory").AsString(50).Nullable();
            Execute.Sql("Update TestSittingTestMaterialHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update TestSittingTestMaterialHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update TestSittingTestMaterialHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("TestSittingTestMaterialHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("TestSittingTestMaterialHistory");
            Delete.Column("DeletedDate").FromTable("TestSittingTestMaterialHistory");

            Execute.Sql(@"ALTER TABLE TestSittingTestMaterialHistory ADD CONSTRAINT PK_TestSittingTestMaterialHistory PRIMARY KEY (TestSittingTestMaterialId,ModifiedDate,RowStatus)");

            //WorkPracticeHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE WorkPracticeHistory DROP CONSTRAINT PK_WorkPracticeHistory");

            Create.Column("RowStatus").OnTable("WorkPracticeHistory").AsString(50).Nullable();
            Execute.Sql("Update WorkPracticeHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update WorkPracticeHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update WorkPracticeHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("WorkPracticeHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("WorkPracticeHistory");
            Delete.Column("DeletedDate").FromTable("WorkPracticeHistory");

            Execute.Sql(@"ALTER TABLE WorkPracticeHistory ADD CONSTRAINT PK_WorkPracticeHistory PRIMARY KEY (WorkPracticeId,PersonID,CustomerNumber,PractitionerNumber,ApplicationID,ModifiedDate,RowStatus)");


            //ApplicationCustomFieldsHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE ApplicationCustomFieldsHistory DROP CONSTRAINT PK_ApplicationCustomFieldsHistory");

            Create.Column("RowStatus").OnTable("ApplicationCustomFieldsHistory").AsString(50).Nullable();
            Execute.Sql("Update ApplicationCustomFieldsHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update ApplicationCustomFieldsHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update ApplicationCustomFieldsHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("ApplicationCustomFieldsHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("ApplicationCustomFieldsHistory");
            Delete.Column("DeletedDate").FromTable("ApplicationCustomFieldsHistory");

            Execute.Sql(@"ALTER TABLE ApplicationCustomFieldsHistory ADD CONSTRAINT PK_ApplicationCustomFieldsHistory PRIMARY KEY (ApplicationCustomFieldId,ModifiedDate,RowStatus)");

            //EndorsedQualificationHistory-----------------------------------------------------------------------------------------------------------------------------------------
            Execute.Sql(@"ALTER TABLE EndorsedQualificationHistory DROP CONSTRAINT PK_EndorsedQualificationHistory");

            Create.Column("RowStatus").OnTable("EndorsedQualificationHistory").AsString(50).Nullable();
            Execute.Sql("Update EndorsedQualificationHistory Set RowStatus = 'Obsolete' where ObsoletedDate is not null");
            Execute.Sql("Update EndorsedQualificationHistory Set RowStatus = 'Deleted', ObsoletedDate = DeletedDate where DeletedDate is not null");
            Execute.Sql("Update EndorsedQualificationHistory Set RowStatus = 'Latest' where RowStatus is null");
            Alter.Column("RowStatus").OnTable("EndorsedQualificationHistory").AsString(50).NotNullable();

            // Delete.Column("ObsoletedDate").FromTable("EndorsedQualificationHistory");
            Delete.Column("DeletedDate").FromTable("EndorsedQualificationHistory");

            Execute.Sql(@"ALTER TABLE EndorsedQualificationHistory ADD CONSTRAINT PK_EndorsedQualificationHistory PRIMARY KEY (EndorsedQualificationId,ModifiedDate,RowStatus)");
        }
    }
}
