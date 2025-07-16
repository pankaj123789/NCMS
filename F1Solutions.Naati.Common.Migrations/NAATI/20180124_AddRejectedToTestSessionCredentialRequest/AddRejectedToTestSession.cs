
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180124_AddRejectedToTestSessionCredentialRequest
{
    [NaatiMigration(201801241100)]
    public class AddRejectedToTestSessionCredentialRequest : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Rejected").OnTable("tbltestsessionCredentialRequest").AsBoolean().NotNullable().WithDefaultValue(0);
            Execute.Sql(@"ALTER TABLE tblTestSessionCredentialRequest DROP CONSTRAINT UC_TestSessionId_CredentialRequestId");
        }
    }
}
