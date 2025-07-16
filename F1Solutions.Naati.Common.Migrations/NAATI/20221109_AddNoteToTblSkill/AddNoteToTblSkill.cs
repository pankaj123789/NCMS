namespace F1Solutions.Naati.Common.Migrations.NAATI._20221109_AddNoteToTblSkill
{
    [NaatiMigration(202211091235)]
    public class AddNoteToTblSkill : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddNoteToTblSkill);
        }
    }
}
