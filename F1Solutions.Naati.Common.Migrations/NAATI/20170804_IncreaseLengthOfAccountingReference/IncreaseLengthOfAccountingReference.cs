using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170804_IncreaseLengthOfAccountingReference
{
    [NaatiMigration(201708041400)]
    public class IncreaseLengthOfAccountingReference : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Alter table tblJobExaminerPayroll alter column AccountingReference varchar(59)");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
