
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171221_ModifyTblEventVenueAddTblTestSession
{
    [NaatiMigration(201712211400)]
    public class ModifyTblEventVenueAddTblTestSession : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Name").OnTable("tblVenue").AsString(100).NotNullable();
            Create.Column("PublicNotes").OnTable("tblVenue").AsString(1000).Nullable();
            Create.Column("Inactive").OnTable("tblVenue").AsBoolean().NotNullable().WithDefaultValue(0);
            
            Create.Table("tblTestSession")
                .WithColumn("TestSessionId").AsInt32().Identity().PrimaryKey()
                .WithColumn("VenueId").AsInt32().NotNullable()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("TestDateTime").AsDateTime().NotNullable()
                .WithColumn("ArrivalTime").AsInt32().Nullable()
                .WithColumn("Duration").AsInt32().Nullable()
                .WithColumn("CredentialApplicationTypeId").AsInt32().NotNullable()
                .WithColumn("CredentialTypeId").AsInt32().NotNullable()
                .WithColumn("Completed").AsBoolean().NotNullable();

            Create.ForeignKey("FK_TestSession_Venue")
                .FromTable("tblTestSession")
                .ForeignColumn("VenueId")
                .ToTable("tblVenue")
                .PrimaryColumn("VenueId");

            Create.ForeignKey("FK_TestSession_CredentialApplicationType")
                .FromTable("tblTestSession")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_TestSession_CredentialType")
                .FromTable("tblTestSession")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.Table("tblTestSessionCredentialRequest")
                .WithColumn("TestSessionCredentialRequestId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSessionId").AsInt32().NotNullable()
                .WithColumn("CredentialRequestId").AsInt32().NotNullable();

            Create.ForeignKey("FK_TestSessionCredentialRequest_TestSession")
                .FromTable("tblTestSessionCredentialRequest")
                .ForeignColumn("TestSessionId")
                .ToTable("tblTestSession")
                .PrimaryColumn("TestSessionId");

            Create.ForeignKey("FK_TestSessionCredentialRequest_CredentialRequest")
                .FromTable("tblTestSessionCredentialRequest")
                .ForeignColumn("CredentialRequestId")
                .ToTable("tblCredentialRequest")
                .PrimaryColumn("CredentialRequestId");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblTestSessionCredentialRequest] 
                ADD  CONSTRAINT [UC_TestSessionId_CredentialRequestId]
                UNIQUE NONCLUSTERED ([TestSessionId] , [CredentialRequestId])");
        }
    }
}
