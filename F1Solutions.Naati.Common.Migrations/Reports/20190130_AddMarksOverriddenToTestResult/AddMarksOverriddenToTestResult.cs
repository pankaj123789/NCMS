using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190130_AddMarksOverriddenToTestResult
{
    [NaatiMigration(201901301430)]
    public class AddMarksOverriddenToTestResult : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("MarksOverridden")
                .OnTable("TestResultHistory")
                .AsBoolean().WithDefaultValue(0);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
