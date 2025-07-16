

namespace F1Solutions.Naati.Common.Migrations.NAATI._20190326_AdjustRubricData
{
    [NaatiMigration(201903261111)]
    public class AdjustRubricData:NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"UPDATE criterionResult
                        set criterionResult.RubricMarkingBandId = band.RubricMarkingBandId
                        FROM tblRubricAssessementCriterionResult criterionResult
                        inner join tblRubricMarkingBand band on band.RubricMarkingAssessmentCriterionId = criterionResult.RubricMarkingAssessmentCriterionId 
                        AND criterionResult.RubricMarkingBandId = band.Level
                        where criterionResult.RubricMarkingBandId is not null");
        }
    }
}
