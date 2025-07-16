using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20180801_AddCredentialApplicationTypeCategory
{
    [NaatiMigration(201808011601)]
    public class AddCredentialApplicationTypeCategory:NaatiMigration
    {
        public override void Up()
        {

            AddCredentialTypeCategory();

            AddPdTransitionStartDateKey();

        }

        void AddCredentialTypeCategory()
        {
            Create.Column("CredentialApplicationTypeCategoryId")
                .OnTable("tblCredentialApplicationType").AsInt32().Nullable();

            Create.Table("tblCredentialApplicationTypeCategory")
                .WithColumn("CredentialApplicationTypeCategoryId").AsInt32().PrimaryKey().Identity().Identity()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);


            Create.ForeignKey("FK_CredentialApplicationType_CredentialApplicationTypeCategory")
                .FromTable("tblCredentialApplicationType")
                .ForeignColumn("CredentialApplicationTypeCategoryId")
                .ToTable("tblCredentialApplicationTypeCategory")
                .PrimaryColumn("CredentialApplicationTypeCategoryId");

            Insert.IntoTable("tblCredentialApplicationTypeCategory").Row(new { Name = "None", DisplayName = "None" });

            Update.Table("tblCredentialApplicationType").Set(new { CredentialApplicationTypeCategoryId = 1 }).AllRows();
            Alter.Column("CredentialApplicationTypeCategoryId")
                .OnTable("tblCredentialApplicationType")
                .AsInt32()
                .NotNullable();
        }

        void AddPdTransitionStartDateKey()
        {
            Insert.IntoTable("tblSystemValue")
                .Row(new {ValueKey = "PDTransitionStartDate", Value = new DateTime(2014, 01, 01)});
        }
    }
}
