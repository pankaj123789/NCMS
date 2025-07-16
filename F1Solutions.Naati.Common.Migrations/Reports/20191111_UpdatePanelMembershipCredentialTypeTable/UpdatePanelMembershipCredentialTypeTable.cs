using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Migrations;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191111_UpdatePanelMembershipCredentialTypeTable
{
    [NaatiMigration(201911111117)]
    public class UpdatePanelMembershipCredentialTypeTable :NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"Drop table PanelMembershipCredentialTypesHistory");

            Create.Table("PanelMembershipCredentialTypesHistory")
                .WithColumn("PanelMemberShipCredentialTypeId").AsInt32().PrimaryKey()
                .WithColumn("PanelMembershipId").AsInt32()
                .WithColumn("PanelId").AsInt32().NotNullable()
                .WithColumn("PanelName").AsString(200).Nullable()
                .WithColumn("Role").AsString(200).Nullable()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("PersonName").AsString(400).NotNullable()
                .WithColumn("NaatiNumber").AsInt32().NotNullable()
                .WithColumn("CredentialTypeInternalName").AsString(200).NotNullable()
                .WithColumn("CredentialTypeExternalName").AsString(200).Nullable()

                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();
        }
    }
}
