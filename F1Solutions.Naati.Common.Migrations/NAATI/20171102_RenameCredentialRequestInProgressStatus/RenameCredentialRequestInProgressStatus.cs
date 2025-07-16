
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171102_RenameCredentialRequestInProgressStatus
{
    [NaatiMigration(201711021600)]
    public class RenameCredentialRequestInProgressStatus : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("UPDATE tblCredentialRequestStatusType SET [Name] = 'EligibleForTesting', [DisplayName] = 'Eligible for Testing' WHERE [Name] = 'InProgress'");
        }
    }
}
