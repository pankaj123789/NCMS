using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170216_AddPersonSuburb
{
    [NaatiMigration(201702161655)]
    public class AddPersonSuburb : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(@"
ALTER TABLE [PersonHistory]
ADD [Suburb] VARCHAR(50) NULL");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
