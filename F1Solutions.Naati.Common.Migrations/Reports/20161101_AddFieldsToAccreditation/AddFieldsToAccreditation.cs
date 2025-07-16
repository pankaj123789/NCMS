using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161101_AddFieldsToAccreditation
{
    [NaatiMigration(201611011340)]
    public class AddFieldsToAccreditation : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Language").OnTable("AccreditationHistory").AsString(50).Nullable();
            Create.Column("AccreditationMethodDescription").OnTable("AccreditationHistory").AsString(500).Nullable();
            Create.Column("AccreditationCategoryDescription").OnTable("AccreditationHistory").AsString(100).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
