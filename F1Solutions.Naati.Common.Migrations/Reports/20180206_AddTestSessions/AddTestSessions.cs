using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180206_AddTestSessions
{
    [NaatiMigration(201802061200)]
    public class AddTestSessions : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestSessionsHistory")
                .WithColumn("TestSessionCredentialRequestId").AsInt32().PrimaryKey()
                .WithColumn("TestSessionId").AsInt32().NotNullable()
                .WithColumn("CredentialRequestId").AsInt32().NotNullable()
                .WithColumn("TestSessionName").AsString(200).Nullable()
                .WithColumn("TestLocationState").AsString(50).Nullable()
                .WithColumn("TestLocationCountry").AsString(50).Nullable()
                .WithColumn("TestLocationName").AsString(1000).Nullable()
                .WithColumn("VenueName").AsString(200).Nullable()
                .WithColumn("VenueAddress").AsString(510).Nullable()
                .WithColumn("TestDate").AsDateTime().Nullable()
                .WithColumn("TestStartTime").AsDateTime().Nullable()
                .WithColumn("TestArrivalTime").AsDateTime().Nullable()
                .WithColumn("TestEndTime").AsDateTime().Nullable()
                .WithColumn("ApplicationType").AsString(100).Nullable()
                .WithColumn("CredentialTypeInternalName").AsString(100).Nullable()
                .WithColumn("CredentialTypeExternalName").AsString(100).Nullable()
                .WithColumn("TestSessionCompleted").AsBoolean().Nullable()
                .WithColumn("PersonId").AsInt32().NotNullable()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("Title").AsString(50).Nullable()
                .WithColumn("GivenName").AsString(100).Nullable()
                .WithColumn("OtherNames").AsString(100).Nullable()
                .WithColumn("FamilyName").AsString(100).Nullable()
                .WithColumn("PrimaryAddress").AsString(500).Nullable()
                .WithColumn("State").AsFixedLengthAnsiString(50).Nullable()
                .WithColumn("Postcode").AsFixedLengthAnsiString(4).Nullable()
                .WithColumn("Country").AsString(50).Nullable()
                .WithColumn("PrimaryPhone").AsString(60).Nullable()
                .WithColumn("PrimaryEmail").AsString(200).Nullable()
                .WithColumn("ApplicationId").AsInt32().Nullable()
                .WithColumn("ApplicationReference").AsString(10).Nullable()
                .WithColumn("Certification").AsString(100).Nullable()
                .WithColumn("Language1").AsString(50).Nullable()
                .WithColumn("Language1Code").AsString(10).Nullable()
                .WithColumn("Language1Group").AsString(100).Nullable()
                .WithColumn("Language2").AsString(50).Nullable()
                .WithColumn("Language2Code").AsString(50).Nullable()
                .WithColumn("Language2Group").AsString(100).Nullable()
                .WithColumn("Skill").AsString(100).Nullable()
                .WithColumn("Status").AsString(100).Nullable()
                .WithColumn("StatusDateModified").AsDateTime().NotNullable()
                .WithColumn("StatusModifiedUser").AsString(100).Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            // Create CredentialRequests view
            Execute.Sql(Sql.CreateTestSessionsView);

            // Create CredentialRequests procedure
            Execute.Sql(Sql.CreateTestSessionsProcedure);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
