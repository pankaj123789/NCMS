using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20170222_AdditionalColumns
{
    [NaatiMigration(201702221349)]
    public class AdditionalColumns : NaatiMigration
    {
        public override void Up()
        {
            using (new ReportMigrationContainer(this, "Accreditation"))
            {
                Create.Column("CandidateName").OnTable("AccreditationHistory").AsString(252).Nullable();
            }

            using (new ReportMigrationContainer(this, "Application"))
            {
                Create.Column("CandidateName").OnTable("ApplicationHistory").AsString(252).Nullable();
            }

            using (new ReportMigrationContainer(this, "Test"))
            {
                Create.Column("CandidateName").OnTable("TestHistory").AsString(252).Nullable();
                Create.Column("LanguageName").OnTable("TestHistory").AsString(50).Nullable();
                Create.Column("AccreditationLevel").OnTable("TestHistory").AsString(100).Nullable();
                Create.Column("AccreditationCategoryDescription").OnTable("TestHistory").AsString(100).Nullable();
                Create.Column("Direction").OnTable("TestHistory").AsString(20).Nullable();
            }
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
