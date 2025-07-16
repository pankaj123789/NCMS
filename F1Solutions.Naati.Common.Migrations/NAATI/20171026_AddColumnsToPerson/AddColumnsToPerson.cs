
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171026_AddColumnsToPerson
{
    [NaatiMigration(201710261727)]
    public class AddColumnsToPerson : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("EthicalCompetency").OnTable("tblPerson").AsBoolean().WithDefaultValue(0);
            Create.Column("InterculturalCompetency").OnTable("tblPerson").AsBoolean().WithDefaultValue(0);
        }
    }
}
