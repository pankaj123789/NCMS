namespace F1Solutions.Naati.Common.Migrations.NAATI._20210408_AlterTluState
{
    [NaatiMigration(202104081714)]
    public class AlterTluState : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tluState").AlterColumn("State").AsString(6).NotNullable();
            Execute.Sql("UPDATE tluState SET State = 'ONLINE' WHERE StateId = 118");
        }
    }
}
