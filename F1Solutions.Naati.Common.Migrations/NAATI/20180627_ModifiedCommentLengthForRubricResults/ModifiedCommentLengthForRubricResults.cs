
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180627_ModifiedCommentLengthForRubricResults
{
    [NaatiMigration(201806271301)]
    public class ModifiedCommentLengthForRubricResults:NaatiMigration
    {
        public override void Up()
        {
          Execute.Sql("ALTER TABLE tblRubricAssessementCriterionResult ALTER COLUMN Comments nvarchar(4000) not null");
        }
    }
}
