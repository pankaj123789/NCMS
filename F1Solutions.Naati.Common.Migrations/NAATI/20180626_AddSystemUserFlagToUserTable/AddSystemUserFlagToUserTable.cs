
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180626_AddSystemUserFlagToUserTable
{
    [NaatiMigration(201806261200)]
    public class AddSystemUserFlagToUserTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("SystemUser").OnTable("tblUser").AsBoolean().NotNullable().WithDefaultValue(0);
            Execute.Sql("update tblUser set SystemUser = 1 where username like '%_svc%' or username like '%eportal%'");
        }
    }
}
