
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180423_AddAllowSupplementaryColumn
{
    [NaatiMigration(201804230800)]
    public class AddAllowSupplementaryColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("AllowSupplementary")
                .OnTable("tblcredentialapplicationtypecredentialtype")
                .AsBoolean()
                .WithDefaultValue(0);
        }
    }
}
