
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170912_AddEmailMessageAttachmentTable
{
    [NaatiMigration(201709121102)]
    public class AddEmailMessageAttachmentTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblEmailMessageAttachment")
                .WithColumn("EmailMessageAttachmentId").AsInt32().Identity().PrimaryKey()
                .WithColumn("EmailMessageId").AsInt32()
                .WithColumn("StoredFileId").AsInt32()
                .WithColumn("Description").AsString(500).Nullable();

            Create.ForeignKey("FK_EmailMessageAttachment_EmailMessage")
                .FromTable("tblEmailMessageAttachment")
                .ForeignColumn("EmailMessageId")
                .ToTable("tblEmailMessage")
                .PrimaryColumn("EmailMessageId");
            Create.ForeignKey("FK_EmailMessageAttachment_StoredFile")
                .FromTable("tblEmailMessageAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }
    }
}
