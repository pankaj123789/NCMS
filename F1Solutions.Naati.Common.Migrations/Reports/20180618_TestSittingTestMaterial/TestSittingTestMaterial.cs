using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180618_TestSittingTestMaterial
{
    [NaatiMigration(201806181100)]
    public class RubricHistory : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("TestSittingTestMaterialHistory")
                .WithColumn("TestSittingTestMaterialId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("TestSittingId").AsInt32().Nullable()
                .WithColumn("TestSessionId").AsInt32().Nullable()
                .WithColumn("TestMaterialId").AsInt32().Nullable()
                .WithColumn("SatDate").AsDateTime().Nullable()
                .WithColumn("TestLocationName").AsString(1000).Nullable()
                .WithColumn("TestLocationState").AsString(50).Nullable()
                .WithColumn("CredentialRequestId").AsInt32().Nullable()
                .WithColumn("CredentialApplicationId").AsInt32().Nullable()
                .WithColumn("PersonId").AsInt32().Nullable()
                .WithColumn("CustomerNo").AsInt32().Nullable()
                .WithColumn("CandidateName").AsString(252).Nullable()
                .WithColumn("Language1").AsString(50).Nullable()
                .WithColumn("Language1Code").AsString(10).Nullable()
                .WithColumn("Language1Group").AsString(100).Nullable()
                .WithColumn("Language2").AsString(50).Nullable()
                .WithColumn("Language2Code").AsString(10).Nullable()
                .WithColumn("Language2Group").AsString(100).Nullable()
                .WithColumn("Skill").AsString(100).Nullable()
                .WithColumn("TestMaterialTitle").AsString(510).Nullable()
                .WithColumn("TestTaskTypeLabel").AsString(100).Nullable()
                .WithColumn("TestTaskTypeName").AsString(50).Nullable()
                .WithColumn("TestTaskLabel").AsString(100).Nullable()
                .WithColumn("TestTaskName").AsString(100).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
