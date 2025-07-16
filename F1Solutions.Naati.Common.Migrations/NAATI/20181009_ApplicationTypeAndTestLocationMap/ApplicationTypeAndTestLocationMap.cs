
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181009_ApplicationTypeAndTestLocationMap
{
    [NaatiMigration(201810091514)]
    public class ApplicationTypeAndTestLocationMap : NaatiMigration
    {
        public override void Up()
        {
			Create.Table("tblCredentialApplicationTypeTestLocation")
				.WithColumn("CredentialApplicationTypeTestLocationId").AsInt32().PrimaryKey().Identity()
				.WithColumn("CredentialApplicationTypeId").AsInt32().NotNullable().ForeignKey("tblCredentialApplicationType", "CredentialApplicationTypeId")
				.WithColumn("TestLocationId").AsInt32().NotNullable().ForeignKey("tblTestLocation", "TestLocationId");

			Create.UniqueConstraint().OnTable("tblCredentialApplicationTypeTestLocation").Columns(new[] { "CredentialApplicationTypeId", "TestLocationId" });
		}
    }
}

