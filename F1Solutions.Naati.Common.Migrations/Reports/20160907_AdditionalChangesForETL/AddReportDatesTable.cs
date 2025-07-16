using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160907_AdditionalChangesForETL
{
    [NaatiMigration(201609071520)]
    public class AddReportDatesTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("ReportDates").InSchema("Internal").WithColumn("Date").AsDateTime().NotNullable().PrimaryKey();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
