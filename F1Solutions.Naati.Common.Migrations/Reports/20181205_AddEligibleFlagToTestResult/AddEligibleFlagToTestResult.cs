using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20181205_AddEligibleFlagToTestResult
{
    [NaatiMigration(201812051400)]
    public class AddEligibleFlagToTestResult : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("EligibleForSupplementary")
                .OnTable("TestResultHistory")
                .AsBoolean().WithDefaultValue(0);

            Create.Column("EligibleForConcededPass")
                .OnTable("TestResultHistory")
                .AsBoolean().WithDefaultValue(0);

            Create.Column("EligibleForSupplementary")
                .OnTable("TestResultRubricHistory")
                .AsBoolean().WithDefaultValue(0);

            Create.Column("EligibleForConcededPass")
                .OnTable("TestResultRubricHistory")
                .AsBoolean().WithDefaultValue(0);

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
