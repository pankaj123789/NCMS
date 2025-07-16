namespace F1Solutions.Naati.Common.Migrations.NAATI._20221111_AddNoteToTblLanguage
{
    [NaatiMigration(202211110745)]
    public class AddNoteToTblLanguage : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddNoteToTblLanguage);
        }
    }
}
