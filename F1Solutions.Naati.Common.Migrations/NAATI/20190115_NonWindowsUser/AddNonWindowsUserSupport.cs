

namespace F1Solutions.Naati.Common.Migrations.NAATI._20190115_NonWindowsUser
{
    [NaatiMigration(201901151200)]
    public class AddNonWindowsUserSupport : NaatiMigration
    {
        public override void Up()
        {
            Delete.Index("AK_tblUser").OnTable("tblUser");
            Create.Column("NonWindowsUser").OnTable("tblUser").AsBoolean().NotNullable().WithDefaultValue(0);
            Create.Column("Password").OnTable("tblUser").AsString(128).Nullable();
            Create.Column("LastPasswordChangeDate").OnTable("tblUser").AsDateTime().Nullable();
            Create.Column("FailedPasswordAttemptCount").OnTable("tblUser").AsInt32().NotNullable().WithDefaultValue(0);
            Create.Column("IsLockedOut").OnTable("tblUser").AsBoolean().NotNullable().WithDefaultValue(0);
            Create.Column("LastLockoutDate").OnTable("tblUser").AsDateTime().Nullable();
            Alter.Column("UserName").OnTable("tblUser").AsString(50);
            Alter.Column("FullName").OnTable("tblUser").AsString(100);
            Alter.Column("Note").OnTable("tblUser").AsString(int.MaxValue).Nullable();
            Create.UniqueConstraint("U_User_UserName").OnTable("tblUser").Column("UserName");
        }
    }
}
