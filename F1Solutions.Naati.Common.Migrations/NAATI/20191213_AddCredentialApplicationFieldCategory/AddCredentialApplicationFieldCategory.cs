using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20191213_AddCredentialApplicationFieldCategory
{
    [NaatiMigration(201912131030)]
    public class AddCredentialApplicationFieldCategory :NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialApplicationFieldCategory")
                .WithColumn("CredentialApplicationFieldCategoryId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable();

            Execute.Sql("SET IDENTITY_INSERT tblCredentialApplicationFieldCategory ON;");
            Insert.IntoTable("tblCredentialApplicationFieldCategory")
                .Row(new {CredentialApplicationFieldCategoryId = 1, Name = "None", DisplayName = "None"});
            Execute.Sql("SET IDENTITY_INSERT tblCredentialApplicationFieldCategory OFF;");

            Alter.Table("tblCredentialApplicationField")
                .AddColumn("CredentialApplicationFieldCategoryId")
                .AsInt32().Nullable()
                .ForeignKey("FK_CredentialApplicationField_CredentialApplicationFieldCategory", "tblCredentialApplicationFieldCategory", "CredentialApplicationFieldCategoryId");

            Update.Table("tblCredentialApplicationField").Set(new { CredentialApplicationFieldCategoryId = "1" }).AllRows();

            Alter.Table("tblCredentialApplicationField").AlterColumn("CredentialApplicationFieldCategoryId").AsInt32().NotNullable();
        }
    }
}
