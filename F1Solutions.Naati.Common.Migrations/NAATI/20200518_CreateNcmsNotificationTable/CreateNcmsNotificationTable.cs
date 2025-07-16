namespace F1Solutions.Naati.Common.Migrations.NAATI._20200518_CreateNcmsNotificationTable
{
    [NaatiMigration(202005191505)]
    public class CreateNcmsNotificationTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblNotificationType")
                .WithColumn("NotificationTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);

            Create.Table("tblNotification")
                .WithColumn("NotificationId").AsInt32().Identity().PrimaryKey()
                .WithColumn("NotificationTypeId").AsInt32()
                .ForeignKey("FK_Notification_NotificationType", "tblNotificationType", "NotificationTypeId")
                .WithColumn("Parameter").AsString()
                .WithColumn("CreatedDate").AsDateTime()
                .WithColumn("ExpiryDate").AsDateTime()
                .WithColumn("FromUserId").AsInt32()
                .ForeignKey("FK_NotificationFrom_User", "tblUser", "UserId")
                .WithColumn("ToUserId").AsInt32()
                .ForeignKey("FK_NotificationTo_User", "tblUser", "UserId");
        }
    }
}
