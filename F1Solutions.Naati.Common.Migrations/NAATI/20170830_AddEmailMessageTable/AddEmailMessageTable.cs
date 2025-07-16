
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170830_AddEmailMessageTable
{
    [NaatiMigration(201708300939)]
    public class AddEmailMessageTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblEmailMessage")
                .WithColumn("EmailMessageId").AsInt32().Identity().PrimaryKey()
                .WithColumn("RecipientEntityId").AsInt32()
                .WithColumn("RecipientEmail").AsString(500)
                .WithColumn("CredentialApplicationId").AsInt32().Nullable()
                .WithColumn("Bcc").AsString(int.MaxValue).Nullable()
                .WithColumn("Cc").AsString(int.MaxValue).Nullable()
                .WithColumn("Subject").AsString(500).Nullable()
                .WithColumn("Body").AsString(int.MaxValue).Nullable()
                .WithColumn("LastSendResult").AsString(int.MaxValue).Nullable()
                .WithColumn("LastSendAttemptDate").AsDateTime().Nullable()
                .WithColumn("CreatedUserId").AsString(50)
                .WithColumn("CreatedDate").AsDateTime();

            Create.ForeignKey("FK_EmailMessage_Entity")
                .FromTable("tblEmailMessage")
                .ForeignColumn("RecipientEntityId")
                .ToTable("tblEntity")
                .PrimaryColumn("EntityId");
            Create.ForeignKey("FK_EmailMessage_CredentiallApplication")
                .FromTable("tblEmailMessage")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");
        }
    }
}
