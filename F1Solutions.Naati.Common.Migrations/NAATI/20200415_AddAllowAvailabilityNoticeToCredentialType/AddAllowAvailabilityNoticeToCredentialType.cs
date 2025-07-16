using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20200415_AddAllowAvailabilityNoticeToCredentialType
{
    [NaatiMigration(202004151145)]
    public class AddAllowAvailabilityNoticeToCredentialType : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialType").AddColumn("AllowAvailabilityNotice").AsBoolean().Nullable();

            Update.Table("tblCredentialType").Set(new { AllowAvailabilityNotice = false}).AllRows();

            Alter.Table("tblCredentialType").AlterColumn("AllowAvailabilityNotice").AsBoolean().NotNullable();
        }
    }
}
