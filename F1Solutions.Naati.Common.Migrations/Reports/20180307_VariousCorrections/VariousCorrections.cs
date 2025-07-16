using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180307_VariousCorrections
{
    [NaatiMigration(201803071700)]
    public class VariousCorrections : NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("LanguageName2").OnTable("TestResultHistory").AsString(50);
            Alter.Column("LanguageName2").OnTable("TestHistory").AsString(50);
            Alter.Column("Venue").OnTable("TestHistory").AsString(100);
            Delete.PrimaryKey("PK_TestSessionsHistory").FromTable("TestSessionsHistory");
            Create.PrimaryKey("PK_TestSessionsHistory").OnTable("TestSessionsHistory").Columns("TestSessionId", "TestSittingId", "ModifiedDate");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
