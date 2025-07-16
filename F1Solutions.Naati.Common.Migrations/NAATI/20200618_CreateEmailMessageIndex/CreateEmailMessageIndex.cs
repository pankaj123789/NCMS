namespace F1Solutions.Naati.Common.Migrations.NAATI._20200618_CreateEmailMessageIndex
{
    [NaatiMigration(202006181635)]
    public class CreateEmailMessageIndex : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(Sql.CreateEmailMessageIndex);
        }
    }
}
