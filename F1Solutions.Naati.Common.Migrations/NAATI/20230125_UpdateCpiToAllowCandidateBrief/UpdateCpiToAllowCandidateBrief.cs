
namespace F1Solutions.Naati.Common.Migrations.NAATI._20230125_UpdateCpiToAllowCandidateBrief
{
    [NaatiMigration(202301251232)]
    public class UpdateCpiToAllowCandidateBrief : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("update tblTestComponentType set CandidateBriefRequired = 1, CandidateBriefAvailabilityDays = 3 where TestComponentTypeId in (10, 11) and TestSpecificationId = 6");
        }
    }
}
