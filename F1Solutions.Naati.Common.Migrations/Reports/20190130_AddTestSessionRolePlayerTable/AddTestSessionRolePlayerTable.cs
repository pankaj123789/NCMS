using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190130_AddTestSessionRolePlayerTable
{
    [NaatiMigration(201901301701)]
    public class AddTestSessionRolePlayerTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestSessionRolePlayerHistory")
                .WithColumn("TestSessionRolePlayerId").AsInt32().PrimaryKey()
                .WithColumn("TestSessionId").AsInt32().NotNullable()
                .WithColumn("TestSessionName").AsString(200).Nullable()
                .WithColumn("TestLocationName").AsString(1000).Nullable()
                .WithColumn("TestLocationState").AsString(50).Nullable()
                .WithColumn("TestLocationCountry").AsString(50).Nullable()
                .WithColumn("TestDate").AsDateTime().Nullable()
                .WithColumn("CredentialTypeInternalName").AsString(100).Nullable()
                .WithColumn("CredentialTypeExternalName").AsString(100).Nullable()
                .WithColumn("PersonId").AsInt32().PrimaryKey()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("Status").AsString(100).Nullable()
                .WithColumn("Rehearsed").AsBoolean().WithDefaultValue(0)
                .WithColumn("Attended").AsBoolean().WithDefaultValue(0)
                .WithColumn("Rejected").AsBoolean().WithDefaultValue(0)

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
