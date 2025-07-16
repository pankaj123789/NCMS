using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170411_FixFloatTruncation
{
    [NaatiMigration(201704110942)]
    public class FixFloatTruncation : NaatiMigration
    {
        public override void Up()
        {
            const string alterTableFormat = @"
ALTER TABLE [{0}History]
ALTER COLUMN [{1}] FLOAT NULL;
";

            Execute.Sql(string.Format(alterTableFormat, "TestResult", "TotalMarks"));
            Execute.Sql(string.Format(alterTableFormat, "TestResult", "PassMark"));
            Execute.Sql(string.Format(alterTableFormat, "Mark", "PassMark"));
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
