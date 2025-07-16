
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180510_AddNewColumnTestSpecificationAttachment
{
    [NaatiMigration(201805101700)]
    public class AddRolAddNewColumnTestSpecificationAttachmenteScreen : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("MergeDocument")
                .OnTable("tblTestSpecificationAttachment")
                .AsBoolean()
                .WithDefaultValue(0)
                .NotNullable();
        }
    }
}
