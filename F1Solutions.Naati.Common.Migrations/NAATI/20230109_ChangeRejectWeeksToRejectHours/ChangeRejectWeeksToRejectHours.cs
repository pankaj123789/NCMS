namespace F1Solutions.Naati.Common.Migrations.NAATI._20230109_ChangeRejectWeeksToRejectHours
{
    [NaatiMigration(202301091730)]
    public class ChangeRejectWeeksToRejectHours : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.ChangeRejectWeeksToRejectHours);
        }
    }
}
