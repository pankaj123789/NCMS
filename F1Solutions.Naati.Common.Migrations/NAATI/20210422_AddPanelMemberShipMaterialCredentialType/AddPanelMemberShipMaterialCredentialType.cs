namespace F1Solutions.Naati.Common.Migrations.NAATI._20210422_AddPanelMemberShipMaterialCredentialType
{
    [NaatiMigration(202104221330)]
    public class AddPanelMemberShipMaterialCredentialType : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblPanelMemberShipMaterialCredentialType")
                .WithColumn("PanelMemberShipMaterialCredentialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PanelMemberShipId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("IsCoordinator").AsBoolean();

            Create.ForeignKey("PanelMemberShipMaterialCredentialType_PanelMemberShip")
               .FromTable("tblPanelMemberShipMaterialCredentialType")
               .ForeignColumn("PanelMemberShipId")
               .ToTable("tblPanelMemberShip")
               .PrimaryColumn("PanelMemberShipId");

            Create.ForeignKey("PanelMemberShipMaterialCredentialType_CredentialType")
                .FromTable("tblPanelMemberShipMaterialCredentialType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

        }
    }
}
