
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190904_AddKnowledgeTestToPersonTable
{
    [NaatiMigration(201909041400)]
    public class AddKnowledgeTestToPersonTable: NaatiMigration
    {
        public override void Up()
        {
            Create.Column("KnowledgeTest").OnTable("tblPerson").AsBoolean().Nullable();
            Execute.Sql("update tblPerson set KnowledgeTest = 0");
            Alter.Column("KnowledgeTest").OnTable("tblPerson").AsBoolean().NotNullable();
        }
    }
}
