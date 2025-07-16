using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190508_AddPayRunTables
{
    [NaatiMigration(201905081603)]
    public class AddPayRunTables: NaatiMigration
    {
        public override void Up()
        {
            Create.Table("ExaminerJobHistory")
                .WithColumn("JobExaminerId").AsInt32().PrimaryKey()
                .WithColumn("TestSittingId").AsInt32().NotNullable()
                .WithColumn("JobId").AsInt32().Nullable()
                .WithColumn("ApplicationTypeId").AsInt32().Nullable()
                .WithColumn("ApplicationType").AsString(100).Nullable()
                .WithColumn("ExaminerCustomerNumber").AsInt32().Nullable()
                .WithColumn("CredentialTypeId").AsInt32().Nullable()
                .WithColumn("CredentialType").AsString(100).Nullable()
                .WithColumn("SkillId").AsInt32().Nullable()
                .WithColumn("Skill").AsString(300).Nullable()
                .WithColumn("DateAllocated").AsDateTime().Nullable()
                .WithColumn("ReceivedDate").AsDateTime().Nullable()
                .WithColumn("DueDate").AsDateTime().Nullable()
                .WithColumn("ExaminerCost").AsDecimal().Nullable()
                .WithColumn("ProductSpecificationId").AsInt32().Nullable()
                .WithColumn("ProductSpecificationCode").AsString(100)
                .WithColumn("ProductSpecificationDescription").AsString(500)
                .WithColumn("GLCodeId").AsInt32().Nullable()
                .WithColumn("GLCode").AsString(100).Nullable()
                .WithColumn("ExaminerPayRollStatusId").AsInt32().Nullable()
                .WithColumn("ExaminerPayRollStatus").AsString(100).Nullable()
                .WithColumn("PayRollModifiedDate").AsDateTime().Nullable()
                .WithColumn("PayRollModifiedUser").AsString(100).Nullable()
                .WithColumn("PayRollAccountingReference").AsString(100).Nullable()
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
