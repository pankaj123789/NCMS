
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181109_AddCredentialTypesToMemberShip
{

    [NaatiMigration(201811091003)]
    public class AddCredentialTypesToMemberShip : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblPanelMemberShipCredentialType")
                .WithColumn("PanelMemberShipCredentialTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PanelMemberShipId").AsInt32()
                .WithColumn("CredentialTypeId").AsInt32();

            Create.ForeignKey("PanelMemberShipCredentialType_PanelMemberShip")
                .FromTable("tblPanelMemberShipCredentialType")
                .ForeignColumn("PanelMemberShipId")
                .ToTable("tblPanelMemberShip")
                .PrimaryColumn("PanelMemberShipId");

            Create.ForeignKey("PanelMemberShipCredentialType_CredentialType")
                .FromTable("tblPanelMemberShipCredentialType")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Alter.Table("tluPanelType").AddColumn("AllowCredentialTypeLink").AsBoolean().Nullable();
            Update.Table("tluPanelType").Set(new { AllowCredentialTypeLink = 0 }).AllRows();
            Alter.Column("AllowCredentialTypeLink").OnTable("tluPanelType").AsBoolean().NotNullable();

            Execute.Sql(@"INSERT INTO tblpanelmembershipCredentialtype (PanelMembershipId,CredentialTypeId )
                            SELECT pm.PanelMembershipId, ct.CredentialTypeId 
                            from tblPanelMembership pm 
                            cross join tblCredentialType ct
                            where ct.CredentialTypeId in (2,6,7,14)");

        }
    }
}
