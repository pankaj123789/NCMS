
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180305_AddPractitionerFlagToForms
{
    [NaatiMigration(201803051501)]
    public class AddPractitionerFlagToForms : NaatiMigration
    {
        public override void Up()
        {
            // wipe form tables to add the new column, This tables will be populated again in the post scripts CredentialApplicationForms.sql
            Execute.Sql("DELETE from tblCredentialApplicationFormAnswerOptionDocumentType");
            Execute.Sql("DELETE from tblCredentialApplicationFormAnswerOptionActionType");
            Execute.Sql("DELETE from tblCredentialApplicationFormQuestionLogic");
            Execute.Sql("DELETE from tblCredentialApplicationFormQuestionAnswerOption");
            Execute.Sql("DELETE from tblCredentialApplicationFormAnswerOption");
            Execute.Sql("DELETE from tblCredentialApplicationFormQuestion");
            Execute.Sql("DELETE from tblCredentialApplicationFormQuestionType");
            Execute.Sql("DELETE from tblCredentialApplicationFormSection");
            Execute.Sql("DELETE from tblCredentialApplicationForm");
            Execute.Sql("DELETE from tblCredentialApplicationFormAnswerType");
            Execute.Sql("DELETE from tblCredentialApplicationFormActionType");

            Delete.Column("LoginRequired").FromTable("tblCredentialApplicationForm");

            Create.Table("tblCredentialApplicationFormUserType")
                .WithColumn("CredentialApplicationFormUserTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Column("CredentialApplicationFormUserTypeId").OnTable("tblCredentialApplicationForm").AsInt32();
            
            Create.ForeignKey("FK_CredentialApplicationForm_CredentialApplicationFormUserType")
                .FromTable("tblCredentialApplicationForm")
                .ForeignColumn("CredentialApplicationFormUserTypeId")
                .ToTable("tblCredentialApplicationFormUserType")
                .PrimaryColumn("CredentialApplicationFormUserTypeId");
        }
    }
}
