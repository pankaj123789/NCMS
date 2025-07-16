
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190326_RubricRules
{
    [NaatiMigration(201903261700)]
    public class RubricRules : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblTestResultEligibilityType").WithDescription("Types of test result that a person may be eligible for, depending on their scores")
                .WithColumn("TestResultEligibilityTypeId").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(50).NotNullable().WithColumnDescription("e.g. Pass, ConcededPass, SupplementaryTest")
                .WithColumn("DisplayName").AsString(50).NotNullable();

            Create.Table("tblRubricQuestionPassRule").WithDescription("Describes rules for passing individual questions")
                .WithColumn("RubricQuestionPassRuleId").AsInt32().PrimaryKey().Identity()
                .WithColumn("TestSpecificationId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricQuestionPassRule_TestSpecification", "tblTestSpecification", "TestSpecificationId")
                .WithColumn("TestComponentTypeId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricQuestionPassRule_TestComponentType", "tblTestComponentType", "TestComponentTypeId")
                .WithColumn("RubricMarkingAssessmentCriterionId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricQuestionPassRule_RubricMarkingAssessmentCriterion", "tblRubricMarkingAssessmentCriterion", "RubricMarkingAssessmentCriterionId")
                .WithColumn("MaximumBandLevel").AsInt32().NotNullable()
                    .WithColumnDescription("The minimum band level required to pass the q uestion (see Level column of tblRubricMarkingBand)")
                .WithColumn("RuleGroup").AsAnsiString(10).Nullable()
                    .WithColumnDescription("Groups rows and describes what logical operation to apply to them. All ungrouped rows are logically ANDed");

            Create.Table("tblRubricTestQuestionRule").WithDescription("Describes rules for determining test result eligibility based on questions attempted or passed")
                .WithColumn("RubricTestQuestionRuleId").AsInt32().PrimaryKey().Identity()
                .WithColumn("TestSpecificationId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestQuestionRule_TestSpecification", "tblTestSpecification", "TestSpecificationId")
                .WithColumn("TestResultEligibilityTypeId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestQuestionRule_TestResultEligibilityType", "tblTestResultEligibilityType", "TestResultEligibilityTypeId")
                    .WithColumnDescription("Specifies what kind of result eligibility this rule applies to (Pass, Conceded Pass, Supplementary")
                .WithColumn("TestComponentTypeId").AsInt32().Nullable()
                    .ForeignKey("FK_RubricTestQuestionRule_TestComponentType", "tblTestComponentType", "TestComponentTypeId")
                .WithColumn("MinimumQuestionsAttempted").AsInt32().NotNullable()
                .WithColumn("MinimumQuestionsPassed").AsInt32().NotNullable()
                .WithColumn("RuleGroup").AsAnsiString(10).Nullable()
                    .WithColumnDescription("Groups rows and describes what logical operation to apply to them. All ungrouped rows are logically ANDed");

            Create.Table("tblRubricTestBandRule")
                .WithDescription("Describes rules for determining test result eligibility based on bands acheived in a minimum number of questions of a certain Task Type")
                .WithColumn("RubricTestBandRuleId").AsInt32().PrimaryKey().Identity()
                .WithColumn("TestSpecificationId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestBandRule_TestSpecification", "tblTestSpecification", "TestSpecificationId")
                .WithColumn("TestResultEligibilityTypeId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestBandRule_TestResultEligibilityType", "tblTestResultEligibilityType", "TestResultEligibilityTypeId")
                .WithColumnDescription("Specifies what kind of result eligibility this rule applies to (Pass, Conceded Pass, Supplementary")
                // not required with current rules but could be useful for a rule like: "At least band 2 in all Task Type A questions"
                .WithColumn("TestComponentTypeId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestBandRule_TestComponentType", "tblTestComponentType", "TestComponentTypeId")
                .WithColumn("RubricMarkingAssessmentCriterionId").AsInt32().NotNullable()
                    .ForeignKey("FK_RubricTestBandRule_RubricMarkingAssessmentCriterion", "tblRubricMarkingAssessmentCriterion", "RubricMarkingAssessmentCriterionId")
                .WithColumn("MaximumBandLevel").AsInt32().NotNullable()
                    .WithColumnDescription("The (numerically) highest band level required to be eligible for the result (see Level column of tblRubricMarkingBand)")
                .WithColumn("NumberOfQuestions").AsInt32().NotNullable().WithColumnDescription("The minimum number of questions in which the minimum band must be met")
                .WithColumn("RuleGroup").AsAnsiString(10).Nullable()
                    .WithColumnDescription("Groups rows and describes what logical operation to apply to them. All ungrouped rows are logically ANDed");

            Create.Column("ResultAutoCalculation").OnTable("tblTestSpecification").AsBoolean().WithDefaultValue(0);

            Alter.Column("Comments").OnTable("tblRubricAssessementCriterionResult").AsString(4000).Nullable();
        }
    }
}
