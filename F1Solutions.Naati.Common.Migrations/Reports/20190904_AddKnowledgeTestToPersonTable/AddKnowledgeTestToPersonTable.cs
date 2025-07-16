using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190904_AddKnowledgeTestToPersonTable
{
    [NaatiMigration(201909041530)]
    public class AddKnowledgeTestToPersonTable: NaatiMigration
    {
        public override void Up()
        {
            Create.Column("KnowledgeTest").OnTable("PersonHistory").AsBoolean().Nullable();
            Execute.Sql("update PersonHistory set KnowledgeTest = 0");
            Alter.Column("KnowledgeTest").OnTable("PersonHistory").AsBoolean().NotNullable();
        }
    }
}
