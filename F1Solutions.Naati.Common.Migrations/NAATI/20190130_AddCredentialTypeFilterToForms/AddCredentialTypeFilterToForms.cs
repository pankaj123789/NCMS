
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190130_AddCredentialTypeFilterToForms
{
    [NaatiMigration(201901301603)]
    public class AddCredentialTypeFilterToForms : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialApplicationFormCredentialType")
                .WithColumn("CredentialApplicationFormCredentialTypeId").AsInt32().NotNullable().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationFormId").AsInt32().NotNullable()
                .WithColumn("CredentialTypeId").AsInt32().NotNullable();
            
            Create.ForeignKey("FK_CredentialApplicationFormCredentialType_CredentialApplicationForm")
                .FromTable("tblCredentialApplicationFormCredentialType")
                .ForeignColumn("CredentialApplicationFormId")
                .ToTable("tblCredentialApplicationForm")
                .PrimaryColumn("CredentialApplicationFormId");

            Create.ForeignKey("FK_CredentialApplicationFormCredentialType_CredentialType")
                .FromTable("tblCredentialApplicationFormCredentialType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

        }
    }
}
