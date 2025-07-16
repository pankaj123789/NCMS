
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181217_AddDocumentTypeRoleAndPanelRoleCategory
{
    [NaatiMigration(201812171532)]
    public class AddDocumentTypeRoleAndPanelRoleCategory : NaatiMigration
    {
        public override void Up()
        {
            CreateDocumentTypeRole();
            CreatePersonAttachment();
            RenameRoleTypeTable();
            CreatePanelRoleCategory();
            RemoveAllPanelRoles();
        }

        private void CreateDocumentTypeRole()
        {
            Create.Table("tblDocumentTypeRole")
                .WithColumn("DocumentTypeRoleId").AsInt32().PrimaryKey().Identity()
                .WithColumn("DocumentTypeId").AsInt32()
                .WithColumn("RoleId").AsInt32()
                .WithColumn("Upload").AsBoolean()
                .WithColumn("Download").AsBoolean();

            Create.ForeignKey("DocumentTypeRole_DocumentType")
                .FromTable("tblDocumentTypeRole")
                .ForeignColumn("DocumentTypeId")
                .ToTable("tluDocumentType")
                .PrimaryColumn("DocumentTypeId");

            Create.ForeignKey("DocumentTypeRole_RoleId")
                .FromTable("tblDocumentTypeRole")
                .ForeignColumn("RoleId")
                .ToTable("tluRole")
                .PrimaryColumn("RoleId");
        }

        private void CreatePersonAttachment()
        {

            Create.Table("tblPersonAttachment")
                .WithColumn("PersonAttachmentId").AsInt32().PrimaryKey().Identity()
                .WithColumn("PersonId").AsInt32()
                .WithColumn("StoredFileId").AsInt32()
                .WithColumn("Description").AsString(255);

            Create.ForeignKey("PersonAttachment_Person")
                .FromTable("tblPersonAttachment")
                .ForeignColumn("PersonId")
                .ToTable("tblPerson")
                .PrimaryColumn("PersonId");

            Create.ForeignKey("PersonAttachment_StoredFile")
                .FromTable("tblPersonAttachment")
                .ForeignColumn("StoredFileId")
                .ToTable("tblStoredFile")
                .PrimaryColumn("StoredFileId");
        }

        private void RenameRoleTypeTable()
        {
            Delete.ForeignKey("FK_tblPanelMembership_tluRoleType").OnTable("tblPanelMembership");

            Rename.Column("RoleTypeId").OnTable("tblPanelMembership").To("PanelRoleId");

            Rename.Table("tluRoleType").To("tblPanelRole");

            Rename.Column("RoleTypeId").OnTable("tblPanelRole").To("PanelRoleId");

            Create.ForeignKey("FK_PanelMembership_PanelRole")
                .FromTable("tblPanelMembership")
                .ForeignColumn("PanelRoleId")
                .ToTable("tblPanelRole")
                .PrimaryColumn("PanelRoleId");

            Delete.Column("RowVersion").FromTable("tblPanelRole");
            Delete.Column("RoleVisible").FromTable("tblPanelRole");

            Create.Column("Chair").OnTable("tblPanelRole").AsBoolean().Nullable();
            Update.Table("tblPanelRole").InSchema("dbo").Set(new { Chair = 0 }).AllRows();
            Alter.Column("Chair").OnTable("tblPanelRole").AsBoolean().NotNullable();
        }

        private void CreatePanelRoleCategory()
        {
            Create.Table("tblPanelRoleCategory")
                .WithColumn("PanelRoleCategoryId").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50)
                .WithColumn("MembershipDurationMonths").AsInt32();


            Execute.Sql("SET IDENTITY_INSERT [dbo].[tblPanelRoleCategory] ON ");
            Insert.IntoTable("tblPanelRoleCategory")
                .InSchema("dbo")
                .Row(new {PanelRoleCategoryId = 1, Name = "Initial", DisplayName = "Initial", MembershipDurationMonths = 1 });
            Execute.Sql("SET IDENTITY_INSERT [dbo].[tblPanelRoleCategory] OFF ");
            Alter.Table("tblPanelRole").AddColumn("PanelRoleCategoryId").AsInt32().Nullable();

            
            Update.Table("tblPanelRole").InSchema("dbo").Set(new { PanelRoleCategoryId = 1}).AllRows();
            Alter.Column("PanelRoleCategoryId").OnTable("tblPanelRole").AsInt32().NotNullable();

            Create.ForeignKey("FK_PanelRole_PanelRoleCategory")
                .FromTable("tblPanelRole")
                .ForeignColumn("PanelRoleCategoryId")
                .ToTable("tblPanelRoleCategory")
                .PrimaryColumn("PanelRoleCategoryId");
        }

        private void RemoveAllPanelRoles()
        {
          Execute.Sql("DELETE FROM TBLPANELROLE WHERE PANELROLEID NOT IN (1, 4, 307, 612)");
          Execute.Sql("DELETE FROM tblsystemvalue WHERE valuekey = 'ExaminerRoles'");
          Execute.Sql("DELETE FROM tlupaneltype WHERE panelTypeId not IN (1)");
        }
    }
}
