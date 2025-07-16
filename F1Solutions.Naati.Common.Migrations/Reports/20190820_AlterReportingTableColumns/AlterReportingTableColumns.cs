using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190820_AlterReportingTableColumns
{
   // [Migration(201908201727)]
    public class AlterReportingTableColumns : NaatiMigration
    {
        public override void Up()
        {
            //Execute.Sql(@"ALTER TABLE ApplicationHistory
            //    ALTER COLUMN PreferredTestLocationState char(3)

            //    ALTER TABLE ApplicationHistory
            //    ALTER COLUMN SponsoredContactName nvarchar(500)

            //    ALTER TABLE CredentialRequestsHistory
            //    ALTER COLUMN DirectionDisplayName nvarchar(max)

            //    ALTER TABLE TestSessionsHistory
            //    ALTER COLUMN Skill nvarchar(max)

            //    ALTER TABLE TestSittingTestMaterialHistory
            //    ALTER COLUMN Skill nvarchar(max)

            //    ALTER TABLE TestHistory
            //    ALTER COLUMN Skill nvarchar(max)");
        }
    }
}
