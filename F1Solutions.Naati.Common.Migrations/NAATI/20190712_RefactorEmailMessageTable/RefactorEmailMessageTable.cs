
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190712_RefactorEmailMessageTable
{
    [NaatiMigration(201907121145)]
    public class RefactorEmailMessageTable : NaatiMigration
    {
        public override void Up()
        {
            CreateCredentialApplicationMessage();
            RemoveCredentialApplicationFromEmailMesage();
            CreateMaterialRequestEmailMessage();

        }

        private void CreateCredentialApplicationMessage()
        {
            Create.Table("tblCredentialApplicationEmailMessage")
                .WithColumn("CredentialApplicationEmailMessageId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationId").AsInt32()
                .WithColumn("EmailMessageId").AsInt32();

            Execute.Sql("INSERT INTO tblCredentialApplicationEmailMessage(CredentialApplicationId, EmailMessageId) SELECT CredentialApplicationId,EmailMessageId FROM TBLEMAILMESSAGE WHERE CredentialApplicationId is not null ");

            Create.ForeignKey("CredentialApplicationEmailMessage_CredentialApplication")
                .FromTable("tblCredentialApplicationEmailMessage")
                .ForeignColumn("CredentialApplicationId")
                .ToTable("tblCredentialApplication")
                .PrimaryColumn("CredentialApplicationId");

            Create.ForeignKey("CredentialApplicationEmailMessage_EmailMessage")
                .FromTable("tblCredentialApplicationEmailMessage")
                .ForeignColumn("EmailMessageId")
                .ToTable("tblEmailMessage")
                .PrimaryColumn("EmailMessageId");
        }

        private void RemoveCredentialApplicationFromEmailMesage()
        {
            Delete.ForeignKey("FK_EmailMessage_CredentiallApplication").OnTable("tblEmailMessage");
            Delete.Column("CredentialApplicationId").FromTable("tblEmailMessage");
        }

        private void CreateMaterialRequestEmailMessage()
        {
            Create.Table("tblMaterialRequestEmailMessage")
                .WithColumn("MaterialRequestEmailMessageId").AsInt32().Identity().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32()
                .WithColumn("EmailMessageId").AsInt32();

            Create.ForeignKey("MaterialRequestEmailMessage_MaterialRequest")
                .FromTable("tblMaterialRequestEmailMessage")
                .ForeignColumn("MaterialRequestId")
                .ToTable("tblmaterialRequest")
                .PrimaryColumn("MaterialRequestId");

            Create.ForeignKey("MaterialRequestEmailMessag_EmailMessage")
                .FromTable("tblMaterialRequestEmailMessage")
                .ForeignColumn("EmailMessageId")
                .ToTable("tblEmailMessage")
                .PrimaryColumn("EmailMessageId");
        }
    }
}
