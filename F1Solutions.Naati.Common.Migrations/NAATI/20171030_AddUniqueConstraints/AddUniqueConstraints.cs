
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171030_AddUniqueConstraints
{
    [NaatiMigration(201710301400)]
    public class AddUniqueConstraints : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormSection] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormSection]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormId] , [DisplayOrder])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormQuestion] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormQuestion]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormSectionId] , [DisplayOrder])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormQuestionAnswerOption] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormQuestionAnswerOption]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormQuestionId] , [DisplayOrder])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormAnswerOptionActionType] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormAnswerOptionActionType]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormAnswerOptionId] , [CredentialApplicationFormActionTypeId], [Order])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormAnswerOptionDocumentType] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormAnswerOptionDocumentType]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormAnswerOptionId] , [DocumentTypeId])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialCredentialRequest] 
                ADD  CONSTRAINT [U_tblCredentialCredentialRequest]
                UNIQUE NONCLUSTERED ([CredentialId] , [CredentialRequestId])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationTypeDocumentType] 
                ADD  CONSTRAINT [U_tblCredentialApplicationTypeDocumentType]
                UNIQUE NONCLUSTERED ([CredentialApplicationTypeId] , [DocumentTypeId])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationTypeCredentialType] 
                ADD  CONSTRAINT [U_tblCredentialApplicationTypeCredentialType]
                UNIQUE NONCLUSTERED ([CredentialApplicationTypeId] , [CredentialTypeId])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFieldOptionOption] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFieldOptionOption]
                UNIQUE NONCLUSTERED ([CredentialApplicationFieldId] , [CredentialApplicationFieldOptionId])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialTypeTemplate] 
                ADD  CONSTRAINT [U_tblCredentialTypeTemplate]
                UNIQUE NONCLUSTERED ([CredentialTypeId] , [DocumentNameTemplate])");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblPerson] 
                ADD  CONSTRAINT [U_tblPerson_PractitionerNumber]
                UNIQUE NONCLUSTERED ([PersonId], [PractitionerNumber])");
        }
    }
}
