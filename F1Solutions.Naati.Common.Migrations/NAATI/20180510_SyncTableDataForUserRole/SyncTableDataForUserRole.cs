
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180510_SyncTableDataForUserRole
{
    [NaatiMigration(201805101800)]
    public class SyncTableDataForUserRole : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Update tblTableData set NextKey = (select max(UserRoleId) from tblUserRole) where TableName = 'UserRole'");
        }
    }
}
