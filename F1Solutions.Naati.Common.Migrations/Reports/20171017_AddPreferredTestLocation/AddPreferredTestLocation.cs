using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171017_AddPreferredTestLocation
{
    [NaatiMigration(201710171046)]
    public class AddPreferredTestLocation : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("PreferredTestLocationState").OnTable("ApplicationHistory").AsString(3).Nullable();
            Create.Column("PreferredTestLocationCity").OnTable("ApplicationHistory").AsString(500).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}