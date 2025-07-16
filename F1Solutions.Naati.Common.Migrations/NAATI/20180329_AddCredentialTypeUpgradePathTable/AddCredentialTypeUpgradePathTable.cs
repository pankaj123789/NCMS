
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180329_AddCredentialTypeUpgradePathTable
{
    [NaatiMigration(201803291100)]
    public class AddCredentialTypeUpgradePathTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialTypeUpgradePath")
                .WithColumn("CredentialTypeUpgradePathId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialTypeFromId").AsInt32().Nullable()
                .WithColumn("CredentialTypeToId").AsInt32().NotNullable();
    

            Create.ForeignKey("FK_CredentialTypeUpgradePathFrom_CredentialType")
                .FromTable("tblCredentialTypeUpgradePath")
                .ForeignColumn("CredentialTypeFromId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialTypeUpgradePathTo_CredentialType")
                .FromTable("tblCredentialTypeUpgradePath")
                .ForeignColumn("CredentialTypeToId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");
        }
    }
}
