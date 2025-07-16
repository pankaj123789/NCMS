
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190417_AddEmailSendStatusType
{
    [NaatiMigration(201904171500)]
    public class AddEmailSendStatusType: NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblEmailSendStatusType")
                .WithColumn("EmailSendStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable();

            Execute.Sql("SET IDENTITY_INSERT tblEmailSendStatusType ON");

            Insert.IntoTable("tblEmailSendStatusType").Row(new
            {
                EmailSendStatusTypeId = 1,
                Name = "Requested",
                DisplayName = "Requested",
            });

            Insert.IntoTable("tblEmailSendStatusType").Row(new
            {
                EmailSendStatusTypeId = 2,
                Name = "Sending",
                DisplayName = "Sending",
            });

            Insert.IntoTable("tblEmailSendStatusType").Row(new
            {
                EmailSendStatusTypeId = 3,
                Name = "Retry",
                DisplayName = "Failed - pending retry",
            });

            Insert.IntoTable("tblEmailSendStatusType").Row(new
            {
                EmailSendStatusTypeId = 4,
                Name = "Failed",
                DisplayName = "Failed permanently",
            });

            Insert.IntoTable("tblEmailSendStatusType").Row(new
            {
                EmailSendStatusTypeId = 5,
                Name = "Successful",
                DisplayName = "Successful",
            });

            Execute.Sql("SET IDENTITY_INSERT tblEmailSendStatusType OFF");

            Alter.Table("tblEmailMessage").AddColumn("EmailSendStatusTypeId").AsInt32().Nullable()
                .ForeignKey("FK_EmailMessage_EmailSendStatusType", "tblEmailSendStatusType", "EmailSendStatusTypeId");

            // set status of existing emails
            Execute.Sql(@"    
                            Update tblEmailMessage set EmailSendStatusTypeId = 5
                            Go
                            Update tblEmailMessage set EmailSendStatusTypeId = 3 where LastSendResult <> 'Successful' or LastSendResult is null
                            Go            
                        ");

            Alter.Table("tblEmailMessage").AlterColumn("EmailSendStatusTypeId").AsInt32().NotNullable();
        }
    }
}
