using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190813_AddTestMaterialRequestTable
{
    [NaatiMigration(201908131617)]
    public class AddTestMaterialRequestTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("MaterialRequestHistory")
                .WithColumn("MaterialRequestId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("PanelId").AsInt32().NotNullable()
                .WithColumn("Panel").AsString(100).Nullable()
                .WithColumn("OutputMaterialId").AsInt32().NotNullable()
                .WithColumn("SourceMaterialId").AsInt32().Nullable()
                .WithColumn("SourceMaterialName").AsString(255).Nullable()
                .WithColumn("OutputMaterialName").AsString(255).Nullable()
                .WithColumn("CredentialType").AsString(255).Nullable()
                .WithColumn("Skill").AsString(4000).Nullable()
                .WithColumn("Status").AsString(255).Nullable()
                .WithColumn("NumberOfRounds").AsInt32().NotNullable()

                .WithColumn("MaxBillableHours").AsDouble().Nullable()
                .WithColumn("ProductSpecificationName").AsString()
                .WithColumn("GLCode").AsString()
                .WithColumn("CostPerHour").AsCurrency()

                .WithColumn("CreatedDate").AsDateTime().Nullable()
                .WithColumn("CreatedBy ").AsString(255).Nullable()
                .WithColumn("OwnedBy ").AsString(225).Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("MaterialRequestRoundHistory")
                .WithColumn("MaterialRequestRoundId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32().NotNullable()
                .WithColumn("OutputMaterialName").AsString(255).Nullable()
                .WithColumn("DueDate").AsDateTime().Nullable()
                .WithColumn("RequestedDate").AsDateTime().Nullable()
                .WithColumn("SubmittedDate").AsDateTime().Nullable()
                .WithColumn("Status").AsString(255).Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("MaterialRequestPanelMemberHistory")
                .WithColumn("MaterialRequestPanelMembershipId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32().NotNullable()
                .WithColumn("OutputMaterialName").AsString(255).Nullable()
                .WithColumn("PanelMemberShipId").AsInt32().NotNullable()
                .WithColumn("MemberType").AsString(255).Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("MaterialRequestTaskHoursHistory")
                .WithColumn("MaterialRequestPanelMembershipTaskId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("MaterialRequestId").AsInt32().NotNullable()
                .WithColumn("OutputMaterialName").AsString(255).Nullable()
                .WithColumn("PanelMemberShipId").AsInt32().NotNullable()
                .WithColumn("Task").AsString(255).Nullable()
                .WithColumn("Hours").AsDouble()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("MaterialRequestPayrollHistory")
                .WithColumn("MaterialRequestPayrollId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("PanelMembershipId").AsInt32().NotNullable()
                .WithColumn("MaterialRequestId").AsInt32().NotNullable()
                .WithColumn("ApprovedDate").AsDateTime().Nullable()
                .WithColumn("ApprovedByUserId").AsInt32().Nullable()
                .WithColumn("PaidByUserId").AsInt32().Nullable()
                .WithColumn("PaymentDate").AsDateTime().Nullable()
                .WithColumn("Amount").AsCurrency().Nullable()
                .WithColumn("PaymentReference").AsString()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("TestMaterialHistory")
                .WithColumn("TestMaterialId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("Title").AsString(255).NotNullable()

                .WithColumn("LanguageId").AsInt32().Nullable()
                .WithColumn("Language").AsString(50).Nullable()
                .WithColumn("SkillId").AsInt32().Nullable()
                .WithColumn("Skill").AsString(int.MaxValue).Nullable()

                .WithColumn("Available").AsBoolean().NotNullable()

                .WithColumn("MaterialRequestId").AsInt32().Nullable()

                .WithColumn("TestMaterialTypeId").AsInt32().NotNullable()
                .WithColumn("TestMaterialType").AsString(255).NotNullable()
                .WithColumn("TestMaterialDomainId").AsInt32().NotNullable()
                .WithColumn("TestMaterialDomain").AsString(255).NotNullable()

                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();


        }
    }
}
