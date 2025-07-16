
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180523_AddDisplayBillToApplicationType
{
    [NaatiMigration(201805231204)]
    public class AddDisplayBillToApplicationType : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("DisplayBills")
                .OnTable("tblCredentialApplicationType")
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(0);

        }
    }
}
