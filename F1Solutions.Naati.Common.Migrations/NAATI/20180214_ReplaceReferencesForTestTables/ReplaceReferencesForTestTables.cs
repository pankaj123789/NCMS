
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180214_ReplaceReferencesForTestTables
{
    [NaatiMigration(201802141000)]
    public class ReplaceReferencesForTestTables:NaatiMigration
    {
        public override void Up()
        {
            RemoveOldReferences();
            CreateNewReferences();
            CreateTestStatusTypeTable();
            AddSatColumnToTestSessionCredentialRequest();
        }

        void RemoveOldReferences()
        {
            Delete.ForeignKey("FK_tblTestResult_tblTestAttendance").OnTable("tblTestResult");
            Delete.ForeignKey("FK_tblTestAttendanceDocument_tblTestAttendance").OnTable("tblTestAttendanceDocument");
            Delete.ForeignKey("FK_tblTestNote_tblTestAttendance").OnTable("tblTestNote");
            Delete.ForeignKey("FK_tblTestAttendance_tblTestMaterial").OnTable("tblTestAttendance");
            Delete.Column("TestMaterialId").FromTable("tblTestAttendance");
        }

        void CreateNewReferences()
        {
            Rename.Column("TestAttendanceId").OnTable("tblTestResult").InSchema("dbo").To("TestSessionCredentialRequestId");
            Create.ForeignKey("FK_TestResult_TestSessionCredentialRequest")
                .FromTable("tblTestResult")
                .ForeignColumn("TestSessionCredentialRequestId")
                .ToTable("tblTestSessionCredentialRequest")
                .PrimaryColumn("TestSessionCredentialRequestId");

            Rename.Column("TestAttendanceId").OnTable("tblTestAttendanceDocument").InSchema("dbo").To("TestSessionCredentialRequestId");
            Create.ForeignKey("FK_TestAttendanceDocument_TestSessionCredentialRequest")
                .FromTable("tblTestAttendanceDocument")
                .ForeignColumn("TestSessionCredentialRequestId")
                .ToTable("tblTestSessionCredentialRequest")
                .PrimaryColumn("TestSessionCredentialRequestId");

            Rename.Column("TestAttendanceId").OnTable("tblTestNote").InSchema("dbo").To("TestSessionCredentialRequestId");
            Create.ForeignKey("FK_TestNote_TestSessionCredentialRequest")
                .FromTable("tblTestNote")
                .ForeignColumn("TestSessionCredentialRequestId")
                .ToTable("tblTestSessionCredentialRequest")
                .PrimaryColumn("TestSessionCredentialRequestId");
        }

        void CreateTestStatusTypeTable()
        {
            Create.Table("tblTestStatusType")
                .WithColumn("TestStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);
        }

        void AddSatColumnToTestSessionCredentialRequest()
        {
            Create.Column("Sat")
                .OnTable("tblTestSessionCredentialRequest")
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(0);
        }

    }
}
