
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180711_AddProfessionalDevelopmentPointCalculation
{
    [NaatiMigration(201807110801)]
    public class AddProfessionalDevelopmentPointCalculation : NaatiMigration
    {
        public override void Up()
        {
            CreatePdPointsLimitType();
            CreateProfessionalDevelopmentSectionCategory();
            UpdateProfessionalDevelopmentCategory();
            UpdateProfessionalDevelopmentCategoryGroup();
            UpdateProfessionalDevelopmentSection();
            AddRequiredPointsToSystemValueTable();
        }

        void AddRequiredPointsToSystemValueTable()
        {
            Execute.Sql("INSERT INTO tblSystemValue (ValueKey, Value) values  ('RecertificationTotalPointsPerYear', '40')");
        }

        void CreatePdPointsLimitType()
        {
            Create.Table("tblPdPointsLimitType")
                .WithColumn("PdPointsLimitTypeId")
                .AsInt32()
                .PrimaryKey()
                .Identity()
                .WithColumn("Name")
                .AsString(50)
                .WithColumn("DisplayName")
                .AsString(50);

            Insert.IntoTable("tblPdPointsLimitType").Row(new { Name = "InitialData", DisplayName = "InitialData" });
        }


        void CreateProfessionalDevelopmentSectionCategory()
        {

            Create.Table("tblProfessionalDevelopmentSectionCategory")
                .WithColumn("ProfessionalDevelopmentSectionCategoryId").AsInt32().PrimaryKey().Identity()
                .WithColumn("ProfessionalDevelopmentSectionId").AsInt32()
                .WithColumn("ProfessionalDevelopmentCategoryId").AsInt32()
                .WithColumn("PdPointsLimitTypeId").AsInt32().Nullable()
                .WithColumn("PointsLimit").AsInt32().Nullable();

            Create.ForeignKey("FK_ProfessionalDevelopmentSectionCategory_ProfessionalDevelopmentSection")
                .FromTable("tblProfessionalDevelopmentSectionCategory")
                .ForeignColumn("ProfessionalDevelopmentSectionId")
                .ToTable("tblProfessionalDevelopmentSection")
                .PrimaryColumn("ProfessionalDevelopmentSectionId");

            Create.ForeignKey("FK_ProfessionalDevelopmentSectionCategory_ProfessionalDevelopmentnCategory")
                .FromTable("tblProfessionalDevelopmentSectionCategory")
                .ForeignColumn("ProfessionalDevelopmentCategoryId")
                .ToTable("tblProfessionalDevelopmentCategory")
                .PrimaryColumn("ProfessionalDevelopmentCategoryId");

            Create.ForeignKey("FK_ProfessionalDevelopmentSectionCategory_PdPointsLimitType")
                .FromTable("tblProfessionalDevelopmentSectionCategory")
                .ForeignColumn("PdPointsLimitTypeId")
                .ToTable("tblPdPointsLimitType")
                .PrimaryColumn("PdPointsLimitTypeId");

        }

        void UpdateProfessionalDevelopmentSection()
        {
            Create.Column("RequiredPointsPerYear").OnTable("tblProfessionalDevelopmentSection").AsDouble().Nullable();
            Update.Table("tblProfessionalDevelopmentSection").Set(new { RequiredPointsPerYear = 0}).AllRows();
            Alter.Column("RequiredPointsPerYear").OnTable("tblProfessionalDevelopmentSection").AsDouble().NotNullable();
        }

        void UpdateProfessionalDevelopmentCategoryGroup()
        {
            Create.Column("RequiredPointsPerYear").OnTable("tblProfessionalDevelopmentCategoryGroup").AsDouble().Nullable();
            Update.Table("tblProfessionalDevelopmentCategoryGroup").Set(new { RequiredPointsPerYear = 0}).AllRows();
            Alter.Column("RequiredPointsPerYear").OnTable("tblProfessionalDevelopmentCategoryGroup").AsDouble().NotNullable();
        }
      

        void UpdateProfessionalDevelopmentCategory()
        {
            Delete.ForeignKey("FK_ProfessionalDevelopmentCategory_ProfessionalDevelopmentSection")
                .OnTable("tblProfessionalDevelopmentCategory");
            Delete.Column("ProfessionalDevelopmentSectionId").FromTable("tblProfessionalDevelopmentCategory");
        }

   
       
    }
}
