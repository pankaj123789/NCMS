
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180213_AddInactiveColumnToCredentialApplicationForm
{
    [NaatiMigration(201802131200)]
    public class AddInactiveColumnToCredentialApplicationForm : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Inactive ").OnTable("tblCredentialApplicationForm").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}
