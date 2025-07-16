using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161004_AddExpressionOfInterest
{
    [NaatiMigration(201610040808)]
    public class AddExpressionOfInterest : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("ExpressionOfInterestHistory")
                .WithColumn("ExpressionOfInterestId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("NAATINumber").AsInt32().Nullable()
                .WithColumn("PersonId").AsInt32().Nullable()
                .WithColumn("LanguageId").AsInt32().Nullable()
                .WithColumn("LanguageName").AsAnsiString(50).Nullable()
                .WithColumn("LanguageIndigenous").AsBoolean().Nullable()
                .WithColumn("ToLanguageId").AsInt32().Nullable()
                .WithColumn("ToLanguageName").AsAnsiString(50).Nullable()
                .WithColumn("ToEnglish").AsBoolean().Nullable()
                .WithColumn("EnteredDate").AsDateTime().Nullable()
                .WithColumn("AccreditationLevel").AsAnsiString(100).Nullable()
                .WithColumn("AccreditationCategoryDescription").AsAnsiString(100).Nullable()
                .WithColumn("LanguageToBoth").AsBoolean().Nullable()
                .WithColumn("Direction").AsString(20).Nullable()
                .WithColumn("PreferredTestCentreName").AsString(100).Nullable();
        }

        public override void Down()
        {            
            throw new NotImplementedException();
        }
    }
}
