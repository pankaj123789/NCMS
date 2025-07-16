using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161004_AddExpressionOfInterest
{
    [NaatiMigration(201610211052)]
    public class AddObsoletedIndex : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_ExpressionOfInterest_ObsoletedDate] ON [dbo].[ExpressionOfInterestHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[ExpressionOfInterestId],
    [NAATINumber],
    [PersonId],
    [LanguageId],
    [LanguageName],
    [LanguageIndigenous],
    [ToLanguageId],
    [ToLanguageName],
    [ToEnglish],
    [EnteredDate],
    [AccreditationLevel],
    [AccreditationCategoryDescription],
    [LanguageToBoth],
    [Direction],
    [PreferredTestCentreName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
