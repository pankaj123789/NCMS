
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180423_UpdateProductSpecification
{
    [NaatiMigration(201804231400)]
    public class UpdateProductSpecification:NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("Name").OnTable("tblProductSpecification").AsString(100).NotNullable();
        }
    }
}
