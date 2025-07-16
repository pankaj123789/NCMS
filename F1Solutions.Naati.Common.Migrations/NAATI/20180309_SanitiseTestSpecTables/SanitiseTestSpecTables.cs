
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180309_SanitiseTestSpecTables
{
    [NaatiMigration(201803091200)]
    public class SanitiseTestSpecTables : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"delete tblTestSpecificationMapping
                         delete tblTestComponent
                         delete tblTestSpecification
                         delete tluTestComponentType");
        }
    }
}
