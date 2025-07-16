
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180409_AddCredentialSelectionControlUpgradesAndSameLevel
{
    [NaatiMigration(201804091600)]
    public class AddCredentialSelectionControlUpgradesAndSameLevel : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialApplicationFormQuestionLogic").AddColumn("CredentialRequestPathTypeId").AsInt32().Nullable().ForeignKey("FK_CredentialApplicationFormQuestionLogic_CredentialRequestPathType", "tblCredentialRequestPathType", "CredentialRequestPathTypeId");
        }
    }
}
