using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190214_AddPanelMembershipCredentialTypesTable
{
    [NaatiMigration(201902141001)]
    public class AddPanelMembershipCredentialTypesTable: NaatiMigration
    {
        public override void Up()
        {
            Create.Table("PanelMembershipCredentialTypesHistory")
                .WithColumn("PanelMembershipId").AsInt32().PrimaryKey()
                .WithColumn("PanelId").AsInt32().NotNullable()
                .WithColumn("PanelName").AsString(200).Nullable()
                .WithColumn("Role").AsString(200).Nullable()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("PersonName").AsString(400).NotNullable().PrimaryKey()
                .WithColumn("NaatiNumber").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("CredentialTypeInternalName").AsString(200).NotNullable().PrimaryKey()
                .WithColumn("CredentialTypeExternalName").AsString(200).Nullable()
                
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
