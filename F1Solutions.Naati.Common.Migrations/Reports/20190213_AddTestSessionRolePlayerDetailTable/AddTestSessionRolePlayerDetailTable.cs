using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190213_AddTestSessionRolePlayerDetailTable
{
    [NaatiMigration(201902131601)]
    public class AddTestSessionRolePlayerDetailTable: NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestSessionRolePlayerDetailsHistory")
                .WithColumn("TestSessionRolePlayerDetailId").AsInt32().PrimaryKey()
                .WithColumn("TestSessionId").AsInt32().NotNullable()
                .WithColumn("TestSessionName").AsString(200).Nullable()
                .WithColumn("TestDate").AsDateTime().Nullable()
                .WithColumn("PersonId").AsInt32().PrimaryKey()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("Status").AsString(100).Nullable()
                .WithColumn("TaskName").AsString(100).Nullable()
                .WithColumn("Language").AsString(100).Nullable()
                .WithColumn("Position").AsString(100).Nullable()

                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
