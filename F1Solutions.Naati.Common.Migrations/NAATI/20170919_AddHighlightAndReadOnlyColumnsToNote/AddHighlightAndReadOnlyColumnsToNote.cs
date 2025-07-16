
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170919_AddHighlightAndReadOnlyColumnsToNote
{
    [NaatiMigration(201709191500)]
    public class AddHighlightAndReadOnlyColumnsToNote : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Highlight").OnTable("tblNote").AsBoolean().NotNullable().WithDefaultValue(0);
            Create.Column("ReadOnly").OnTable("tblNote").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
