using FluentMigrator;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20190820_RoleSecurityOverhaul
{
    [NaatiMigration(201908201200)]
    public class RoleSecurityOverhaul : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM tblUserRole");
            Execute.Sql("DELETE FROM tblDocumentTypeRole");

            Delete.ForeignKey("FK_tblUserRole_tblRole").OnTable("tblUserRole");
            Delete.ForeignKey("DocumentTypeRole_RoleId").OnTable("tblDocumentTypeRole");

            Delete.Table("tblRoleScreen");
            Delete.Table("tluRole");
            Delete.Column("RoleId").FromTable("tblUserRole");
        
            Create.Table("tblSecurityRole")
                .WithColumn("SecurityRoleId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable()
                .WithColumn("Description").AsString(100).Nullable();

            Create.Table("tblSecurityNoun")
                .WithColumn("SecurityNounId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable();

            Create.Table("tblSecurityRule")
                .WithColumn("SecurityRuleId").AsInt32().Identity().PrimaryKey()
                .WithColumn("SecurityRoleId").AsInt32().NotNullable().ForeignKey("FK_SecurityRule_SecurityRole", "tblSecurityRole", "SecurityRoleId")
                .WithColumn("SecurityNounId").AsInt32().NotNullable().ForeignKey("FK_SecurityRule_SecurityNoun", "tblSecurityNoun", "SecurityNounId")
                .WithColumn("SecurityVerbMask").AsInt64().NotNullable();

            Create.Column("SecurityRoleId")
                .OnTable("tblUserRole")
                .AsInt32()
                .NotNullable()
                .ForeignKey("FK_UserRole_SecurityRole", "tblSecurityRole", "SecurityRoleId");

            Create.ForeignKey("FK_DocumentTypeRole_SecurityRole")
                .FromTable("tblDocumentTypeRole").ForeignColumn("RoleId")
                .ToTable("tblSecurityRole").PrimaryColumn("SecurityRoleId");

            // NOTE REQUIRED. REMOVE BEFORE PRODUCTION.
            //Create.Table("tblSecurityVerb")
            //    .WithColumn("SecurityVerbId").AsInt32().Identity().PrimaryKey()
            //    .WithColumn("Name").AsString(50).NotNullable()
            //    .WithColumn("DisplayName").AsString(50).NotNullable()
            //    .WithColumn("Value").AsInt32().NotNullable();
        }
    }
}
