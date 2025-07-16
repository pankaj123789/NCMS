
namespace F1Solutions.Naati.Common.Migrations.NAATI._20210624_AddNewCandidatesOnlyToTestSitting
{

        [NaatiMigration(202106241152)]
        public class AddNewCandidatesOnlyToTestSitting : NaatiMigration
        {
            public override void Up()
            {
            Execute.Sql("Update tblTestSpecification SET ModifiedByNaati = 0 WHERE TestSpecificationId = 15");
            }
        }

}
