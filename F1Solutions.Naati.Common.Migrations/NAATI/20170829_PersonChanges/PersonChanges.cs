using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170829_PersonChanges
{
    [NaatiMigration(201708291723)]
    public class PersonChanges : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE dbo.tblPerson
            DROP COLUMN Citizen");
           
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
