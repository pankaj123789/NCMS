using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180228_UpdateTests
{
    [NaatiMigration(201802281138)]
    public class UpdateTests : NaatiMigration
    {
        public override void Up()
        {
            //TestSessionHistory
            Rename.Column("TestSessionCredentialRequestId").OnTable("TestSessionsHistory").To("TestSittingId");

            //TestHistory
            Rename.Column("TestInvitationId").OnTable("TestHistory").To("TestSittingId");
            Rename.Column("TestAttendanceId").OnTable("TestHistory").To("CredentialRequestId");
            Rename.Column("ApplicationId").OnTable("TestHistory").To("CredentialApplicationId");
            Rename.Column("LanguageName").OnTable("TestHistory").To("LanguageName1");
            Rename.Column("Accepted").OnTable("TestHistory").To("Rejected");

            Delete.PrimaryKey("PK_Test").FromTable("TestHistory");
            Delete.Column("Direction").FromTable("TestHistory");
            Delete.Column("AccreditationCategoryDescription").FromTable("TestHistory");
            Delete.Column("AccreditationLevel").FromTable("TestHistory");
            Delete.Column("WithdrawnDate").FromTable("TestHistory");
            Delete.Column("ResponseDate").FromTable("TestHistory");
            Delete.Column("ConfirmedDate").FromTable("TestHistory");
            Delete.Column("TestEventId").FromTable("TestHistory");
            Delete.Column("TestKey").FromTable("TestHistory");

            Alter.Table("TestHistory").AddColumn("LanguageName2").AsFixedLengthAnsiString(50);
            Alter.Table("TestHistory").AlterColumn("Venue").AsFixedLengthAnsiString(200);
            Alter.Table("TestHistory").AlterColumn("TestSittingId").AsInt32().NotNullable().PrimaryKey();

            //TestResultHistory
            Rename.Column("TestId").OnTable("TestResultHistory").To("TestSittingId");
            Rename.Column("LanguageName").OnTable("TestResultHistory").To("LanguageName1");

            Delete.Column("Direction").FromTable("TestResultHistory");
            Delete.Column("AccreditationCategoryDescription").FromTable("TestResultHistory");
            Delete.Column("AccreditationLevel").FromTable("TestResultHistory");

            Alter.Table("TestResultHistory").AddColumn("LanguageName2").AsFixedLengthAnsiString(50);

            //MarkHistory
            Rename.Column("TestId").OnTable("MarkHistory").To("TestSittingId");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
