using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171120_AddApplicationCustomFieldsTable
{
    [NaatiMigration(201711201400)]
    public class AddApplicationCustomFieldsTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("ApplicationCustomFieldsHistory")
                .WithColumn("ApplicationCustomFieldId").AsInt32().PrimaryKey()
                .WithColumn("PersonId").AsInt32().NotNullable().ForeignKey()
                .WithColumn("ApplicationId").AsInt32().NotNullable().ForeignKey()
                .WithColumn("ApplicationType").AsString(50).NotNullable()
                .WithColumn("ApplicationStatus").AsString(50).NotNullable()
                .WithColumn("ApplicationStatusModifiedDate").AsDateTime().NotNullable()
                .WithColumn("ApplicationEnteredDate").AsDateTime().NotNullable()
                .WithColumn("Section").AsString(100).Nullable()
                .WithColumn("FieldName").AsString(50).NotNullable()
                .WithColumn("Type").AsString(50).NotNullable()
                .WithColumn("Value").AsString(int.MaxValue).Nullable()
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
