using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20171129_AddGroupColumnToQuestionLogic
{
    [NaatiMigration(201711291400)]
    public class AddGroupColumnToQuestionLogic: NaatiMigration
    {
        public override void Up()
        {

            Alter.Column("CredentialApplicationFormQuestionAnswerOptionId")
                .OnTable("tblCredentialApplicationFormQuestionLogic")
                .AsInt32()
                .Nullable();

            Create.Column("CredentialTypeId").OnTable("tblCredentialApplicationFormQuestionLogic").AsInt32().Nullable();

            Create.ForeignKey("FK_CredentialApplicationFormQuestionLogic_CredentialType")
                .FromTable("tblCredentialApplicationFormQuestionLogic")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.Column("[Group]").OnTable("tblCredentialApplicationFormQuestionLogic").AsInt32().WithDefaultValue(0);
            Create.Column("[Order]").OnTable("tblCredentialApplicationFormQuestionLogic").AsInt32().WithDefaultValue(0);

            Execute.Sql("UPDATE tblCredentialApplicationFormQuestionLogic SET [Order]=CredentialApplicationFormQuestionLogicId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationFormQuestionLogic] 
                ADD  CONSTRAINT [U_tblCredentialApplicationFormQuestionLogic]
                UNIQUE NONCLUSTERED ([CredentialApplicationFormQuestionId] , [Group],  [Order])");

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
