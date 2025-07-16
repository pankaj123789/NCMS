
namespace F1Solutions.Naati.Common.Migrations.NAATI._201810301_SchemaChangeForConcededPass
{
    [NaatiMigration(201810301543)]
    public class SchemaChangeForConcededPass : NaatiMigration
    {
        public override void Up()
        {
            CreateCredentialRequestAssociationType();
            AddColumnsToTestResult();
            CreateCredentialRequestCredentialRequest();
            CreateCredentialTypeDowngradePath();
            MigrateData();
        }

        private void CreateCredentialRequestAssociationType()
        {
            Create.Table("tblCredentialRequestAssociationType")
                .WithColumn("CredentialRequestAssociationTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);
        }

        private void AddColumnsToTestResult()
        {
            Alter.Table("tblTestResult").AddColumn("EligibleForConcededPass").AsBoolean().WithDefaultValue(0);
            Alter.Table("tblTestResult").AddColumn("EligibleForSupplementary").AsBoolean().WithDefaultValue(0);
        }

        private void CreateCredentialRequestCredentialRequest()
        {
            Create.Table("tblCredentialRequestCredentialRequest")
                .WithColumn("CredentialRequestCredentialRequestId").AsInt32().Identity().PrimaryKey()
                .WithColumn("OriginalCredentialRequestId").AsInt32()
                .WithColumn("AssociatedCredentialRequestId").AsInt32()
                .WithColumn("AssociationTypeId").AsInt32();

            Create.ForeignKey("FK_CredentialRequestCredentialRequest_OriginalCredentialRequest")
                .FromTable("tblCredentialRequestCredentialRequest")
                .ForeignColumn("OriginalCredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");

            Create.ForeignKey("FK_CredentialRequestCredentialRequest_AssociatedCredentialRequest")
                .FromTable("tblCredentialRequestCredentialRequest")
                .ForeignColumn("AssociatedCredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");

            Create.ForeignKey("FK_CredentialRequestCredentialRequest_CredentialRequestAssociationType")
                .FromTable("tblCredentialRequestCredentialRequest")
                .ForeignColumn("AssociationTypeId")
                .ToTable("tblCredentialRequestAssociationType")
                .PrimaryColumn("CredentialRequestAssociationTypeId");
        }

        private void CreateCredentialTypeDowngradePath()
        {
            Create.Table("tblCredentialTypeDowngradePath")
                .WithColumn("CredentialTypeDowngradePathId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialTypeFromId").AsInt32()
                .WithColumn("CredentialTypeToId").AsInt32();

            Create.ForeignKey("FK_CredentialTypeDowngradePathFrom_CredentialType")
                .FromTable("tblCredentialTypeDowngradePath")
                .ForeignColumn("CredentialTypeFromId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialTypeDowngradePathTo_CredentialType")
                .FromTable("tblCredentialTypeDowngradePath")
                .ForeignColumn("CredentialTypeToId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");
        }

        private void MigrateData()
        {
            Update.Table("tblTestResult").Set(new { EligibleForConcededPass = 1, ResultTypeId = 4 }).Where(new { ResultTypeId = 3 });
            Update.Table("tblTestResult").Set(new { EligibleForSupplementary = 1, ResultTypeId = 4 }).Where(new { ResultTypeId = 6 });
        }
    }
}
