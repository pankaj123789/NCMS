using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171026_AddColumnsToPerson
{
    [NaatiMigration(201710261727)]
    public class AddColumnsToPerson : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("EthicalCompetency").OnTable("PersonHistory").AsBoolean().WithDefaultValue(0);
            Create.Column("InterculturalCompetency").OnTable("PersonHistory").AsBoolean().WithDefaultValue(0);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
