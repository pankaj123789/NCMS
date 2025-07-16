using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161020_RemoveAccreditationExpiry
{
    [NaatiMigration(201610201744)]
    public class RemoveAccreditationExpiry : NaatiMigration
    {
        public override void Up()
        {
            this.ExecuteSql("DROP INDEX IX_Accreditation_ObsoletedDate ON AccreditationHistory");

            Delete.Column("Expired").FromTable("AccreditationHistory");

            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_Accreditation_ObsoletedDate] ON [dbo].[AccreditationHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[AccreditationResultId],
    [ApplicationId],
    [PersonId],
    [ResultDate],
    [FailureReason],
    [Days],
    [CertificateNumber],
    [IncludeInPD],
    [AccreditationLevel],
    [ExpiryDate],
    [Notes],
    [Result]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
