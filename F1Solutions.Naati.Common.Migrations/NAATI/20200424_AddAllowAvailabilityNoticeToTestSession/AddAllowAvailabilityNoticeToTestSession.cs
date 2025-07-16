using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20200424_AddAllowAvailabilityNoticeToTestSession
{
    [NaatiMigration(202004241020)]
    public class AddAllowAvailabilityNoticeToTestSession : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblTestSession").AddColumn("AllowAvailabilityNotice").AsBoolean().Nullable();

            Update.Table("tblTestSession").Set(new { AllowAvailabilityNotice = false}).AllRows();

            Alter.Table("tblTestSession").AlterColumn("AllowAvailabilityNotice").AsBoolean().NotNullable();
        }
    }
}
