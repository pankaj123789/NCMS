using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170324_NullableAccreditationResult
{
    [NaatiMigration(201703241058)]
    public class NullableAccreditationResult : NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("Result").OnTable("AccreditationHistory").AsAnsiString(50).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
