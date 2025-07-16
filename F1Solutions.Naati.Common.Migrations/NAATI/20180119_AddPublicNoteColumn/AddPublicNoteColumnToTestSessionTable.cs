
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180119_AddPublicNoteColumn
{
    [NaatiMigration(201801191700)]
    public class AddPublicNoteColumnToTestSessionTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("PublicNote").OnTable("tblTestSession").AsString(1000).Nullable();
        }
    }
}
