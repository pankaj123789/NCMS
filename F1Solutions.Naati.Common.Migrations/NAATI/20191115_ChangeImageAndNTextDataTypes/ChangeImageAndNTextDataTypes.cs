
namespace F1Solutions.Naati.Common.Migrations.NAATI._20191115_ChangeImageAndNTextDataTypes
{
    [NaatiMigration(201911151222)]
    public class ChangeImageAndNTextDataTypes : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("aspnet_Membership").AlterColumn("Comment").AsCustom("NVARCHAR(MAX)").Nullable();
            Alter.Table("tblPersonImage")
                .AlterColumn("Photo").AsCustom("VARBINARY(MAX)").Nullable()
                .AlterColumn("Signature").AsCustom("VARBINARY(MAX)").Nullable()
                .AlterColumn("ApplicationFirstPage").AsCustom("VARBINARY(MAX)").Nullable()
                .AlterColumn("ApplicationLastPage").AsCustom("VARBINARY(MAX)").Nullable();

            //Add index to improve background email task
           Execute.Sql("CREATE INDEX [EmailMessageStatusIdx] ON [dbo].[tblEmailMessage] ([EmailSendStatusTypeId] ASC)");
           Execute.Sql("CREATE INDEX [EmailMessageLastSentIdx] ON [dbo].[tblEmailMessage] ([LastSendAttemptDate] ASC)");
           Execute.Sql("CREATE NONCLUSTERED INDEX[AccountingOpertionDateIdx] ON [dbo].[tblExternalAccountingOperation]([RequestedDateTime] ASC)");
           Execute.Sql("CREATE NONCLUSTERED INDEX[AccountingOpertionStatusIdx] ON [dbo].[tblExternalAccountingOperation]([StatusId] ASC)");
        }
    }
}
