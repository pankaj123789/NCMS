
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180509_AddRubricConfigurationSchema
{
    [NaatiMigration(201806091801)]
    public class AddRubricConfigurationSchema : NaatiMigration
    {
        public override void Up()
        {
            CreateRubricMarkingCompetency();
            CreateRubricMarkingAssessmentCriterion();
            CreateRubricMarkingBand();
            CreateRubricTestComponentResult();
            CreateJobExaminerRubricTestComponentResult();
            CreateRubricAssessementCriterionResult();
            CreateTestResultRubricTestComponentResult();
        }


        void CreateRubricMarkingCompetency()
        {
            Create.Table("tblRubricMarkingCompetency")
                .WithColumn("RubricMarkingCompetencyId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestComponentTypeId").AsInt32()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("Label").AsString(50)
                .WithColumn("DisplayOrder").AsInt32();

            Create.ForeignKey("FK_RubricMarkingCompetency_TestComponentType")
                .FromTable("tblRubricMarkingCompetency")
                .ForeignColumn("TestComponentTypeId")
                .ToTable("tblTestComponentType")
                .PrimaryColumn("TestComponentTypeId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblRubricMarkingCompetency] 
                ADD  CONSTRAINT [U_tblRubricMarkingCompetency]
                UNIQUE NONCLUSTERED ([TestComponentTypeId] , [DisplayOrder])");
        }

        void CreateRubricMarkingAssessmentCriterion()
        {
            Create.Table("tblRubricMarkingAssessmentCriterion")
                .WithColumn("RubricMarkingAssessmentCriterionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("RubricMarkingCompetencyId").AsInt32()
                .WithColumn("Name").AsAnsiString(100)
                .WithColumn("Label").AsString(50)
                .WithColumn("DisplayOrder").AsInt32();

            Create.ForeignKey("FK_RubricMarkingAssessmentCriterion_RubricMarkingCompetency")
                .FromTable("tblRubricMarkingAssessmentCriterion")
                .ForeignColumn("RubricMarkingCompetencyId")
                .ToTable("tblRubricMarkingCompetency")
                .PrimaryColumn("RubricMarkingCompetencyId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblRubricMarkingAssessmentCriterion] 
                ADD  CONSTRAINT [U_tblRubricMarkingAssessmentCriterion]
                UNIQUE NONCLUSTERED ([RubricMarkingCompetencyId] , [DisplayOrder])");

        }

        void CreateRubricMarkingBand()
        {
            Create.Table("tblRubricMarkingBand")
                .WithColumn("RubricMarkingBandId").AsInt32().Identity().PrimaryKey()
                .WithColumn("RubricMarkingAssessmentCriterionId").AsInt32()
                .WithColumn("Label").AsString(50)
                .WithColumn("Description").AsString(500)
                .WithColumn("Level").AsInt32();

            Create.ForeignKey("FK_RubricMarkingBand_RubricMarkingAssessmentCriterion")
                .FromTable("tblRubricMarkingBand")
                .ForeignColumn("RubricMarkingAssessmentCriterionId")
                .ToTable("tblRubricMarkingAssessmentCriterion")
                .PrimaryColumn("RubricMarkingAssessmentCriterionId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblRubricMarkingBand] 
                ADD  CONSTRAINT [U_tblRubricMarkingBand]
                UNIQUE NONCLUSTERED ([RubricMarkingAssessmentCriterionId] , [Level])");
        }

        void CreateRubricTestComponentResult()
        {
            Create.Table("tblRubricTestComponentResult")
                .WithColumn("RubricTestComponentResultId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestComponentId").AsInt32()
                .WithColumn("WasAttempted").AsBoolean()
                .WithColumn("Successful").AsBoolean().Nullable();

            Create.ForeignKey("FK_RubricTestComponentResult_TestComponent")
                .FromTable("tblRubricTestComponentResult")
                .ForeignColumn("TestComponentId")
                .ToTable("tblTestComponent")
                .PrimaryColumn("TestComponentId");
        }
        void CreateRubricAssessementCriterionResult()
        {
            Create.Table("tblRubricAssessementCriterionResult")
                .WithColumn("RubricAssessementCriterionResultId").AsInt32().Identity().PrimaryKey()
                .WithColumn("RubricTestComponentResultId").AsInt32()
                .WithColumn("RubricMarkingAssessmentCriterionId").AsInt32()
                .WithColumn("RubricMarkingBandId").AsInt32().Nullable()
                .WithColumn("Comments").AsString(500).Nullable();
            
            Create.ForeignKey("FK_RubricAssessementCriterionResult_RubricTestComponentResult")
                .FromTable("tblRubricAssessementCriterionResult")
                .ForeignColumn("RubricTestComponentResultId")
                .ToTable("tblRubricTestComponentResult")
                .PrimaryColumn("RubricTestComponentResultId");

            Create.ForeignKey("FK_RubricAssessementCriterionResult_RubricMarkingAssessmentCriterion")
                .FromTable("tblRubricAssessementCriterionResult")
                .ForeignColumn("RubricMarkingAssessmentCriterionId")
                .ToTable("tblRubricMarkingAssessmentCriterion")
                .PrimaryColumn("RubricMarkingAssessmentCriterionId");

            Create.ForeignKey("FK_RubricAssessementCriterionResult_RubricMarkingBand")
                .FromTable("tblRubricAssessementCriterionResult")
                .ForeignColumn("RubricMarkingBandId")
                .ToTable("tblRubricMarkingBand")
                .PrimaryColumn("RubricMarkingBandId");
        
        }

        void CreateJobExaminerRubricTestComponentResult()
        {
            Create.Table("tblJobExaminerRubricTestComponentResult")
                .WithColumn("JobExaminerRubricTestComponentResultId").AsInt32().Identity().PrimaryKey()
                .WithColumn("JobExaminerId").AsInt32()
                .WithColumn("RubricTestComponentResultId").AsInt32();


            Create.ForeignKey("FK_JobExaminerRubricTestComponentResult_JobExaminer")
                .FromTable("tblJobExaminerRubricTestComponentResult")
                .ForeignColumn("JobExaminerId")
                .ToTable("tblJobExaminer")
                .PrimaryColumn("JobExaminerId");

            Create.ForeignKey("FK_JobExaminerRubricTestComponentResult_RubricTestComponentResult")
                .FromTable("tblJobExaminerRubricTestComponentResult")
                .ForeignColumn("RubricTestComponentResultId")
                .ToTable("tblRubricTestComponentResult")
                .PrimaryColumn("RubricTestComponentResultId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblJobExaminerRubricTestComponentResult] 
                ADD  CONSTRAINT [U_tblJobExaminerRubricTestComponentResult]
                UNIQUE NONCLUSTERED ([JobExaminerId] , [RubricTestComponentResultId])");
        }

        void CreateTestResultRubricTestComponentResult()
        {
            Create.Table("tblTestResultRubricTestComponentResult")
                .WithColumn("TestResultRubricTestComponentResultId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestResultId").AsInt32()
                .WithColumn("RubricTestComponentResultId").AsInt32()
                .WithColumn("ModifiedUserId").AsInt32()
                .WithColumn("ModifiedDate").AsDateTime();

            Create.ForeignKey("FK_TestResultRubricTestComponentResult_TestResult")
                .FromTable("tblTestResultRubricTestComponentResult")
                .ForeignColumn("TestResultId")
                .ToTable("tblTestResult")
                .PrimaryColumn("TestResultId");

            Create.ForeignKey("FK_TestResultRubricTestComponentResult_RubricTestComponentResult")
                .FromTable("tblTestResultRubricTestComponentResult")
                .ForeignColumn("RubricTestComponentResultId")
                .ToTable("tblRubricTestComponentResult")
                .PrimaryColumn("RubricTestComponentResultId");

            Create.ForeignKey("FK_TestResultRubricTestComponentResult_User")
                .FromTable("tblTestResultRubricTestComponentResult")
                .ForeignColumn("ModifiedUserId")
                .ToTable("tblUser")
                .PrimaryColumn("UserId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblTestResultRubricTestComponentResult] 
                ADD  CONSTRAINT [U_tblTestResultRubricTestComponentResult]
                UNIQUE NONCLUSTERED ([TestResultId] , [RubricTestComponentResultId])");
        }


    }
}
