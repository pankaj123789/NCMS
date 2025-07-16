namespace F1Solutions.Naati.Common.Migrations.NAATI._20210517_AddPanelMemberShipCoordinatorCredentialType
{
    [NaatiMigration(202105171632)]
    public class AddPanelMemberShipCoordinatorMaterialType : NaatiMigration
    {
        public override void Up()
        {
            Delete.Column("IsCoordinator").FromTable("tblPanelMemberShipMaterialCredentialType");

            Create.Table("tblPanelMemberShipCoordinatorCredentialType")
                .WithColumn("PanelMemberShipCoordinatorCredentialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PanelMembershipId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32();

            Create.ForeignKey("PanelMemberShipCoordinatorCredentialType_PanelMemberShip")
               .FromTable("tblPanelMemberShipCoordinatorCredentialType")
               .ForeignColumn("PanelMemberShipId")
               .ToTable("tblPanelMemberShip")
               .PrimaryColumn("PanelMemberShipId");

            Create.ForeignKey("PanelMemberShipCoordinatorCredentialType_CredentialType")
                .FromTable("tblPanelMemberShipCoordinatorCredentialType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

        }
    }
}
