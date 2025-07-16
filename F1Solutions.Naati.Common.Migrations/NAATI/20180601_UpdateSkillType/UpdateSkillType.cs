
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180601_UpdateSkillType
{
    [NaatiMigration(201806011831)]
    public class UpdateSkillType : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.SkillType);
        }
    }
}
