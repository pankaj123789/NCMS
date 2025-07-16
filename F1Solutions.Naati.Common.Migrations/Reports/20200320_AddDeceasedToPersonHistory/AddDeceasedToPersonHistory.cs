using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20203020_AddDeceasedToPersonHistory
{
    [NaatiMigration(202003201415)]
    public class AddDeceasedToPersonHistory : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("PersonHistory").AddColumn("Deceased").AsBoolean().Nullable();

            Update.Table("PersonHistory").Set(new { Deceased = false}).AllRows();

            Alter.Table("PersonHistory").AlterColumn("Deceased").AsBoolean().NotNullable();

            //This column is 100 characters in NCMS but was smaller in Reporting. This was leading to may be truncated errors.
            Alter.Table("MarkHistory").AlterColumn("ComponentName").AsString(100);
        }
    }
}
