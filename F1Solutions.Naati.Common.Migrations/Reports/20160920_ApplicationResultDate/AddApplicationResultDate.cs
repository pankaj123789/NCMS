using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160920_ApplicationResultDate
{
    [NaatiMigration(201609201127)]
    public class AddApplicationResultDate : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("ResultDate").OnTable("ApplicationHistory").AsDateTime().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
