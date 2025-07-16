
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190402_AddAutomaticIssuing
{
    [NaatiMigration(201904021004)]
    public class AddAutomaticIssuing :NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblTestSpecification").AddColumn("AutomaticIssuing").AsBoolean().Nullable();
            Alter.Table("tblTestSpecification").AddColumn("MaxScoreDifference").AsDouble().Nullable();
            Update.Table("tblTestSpecification").Set(new { AutomaticIssuing = false} ).AllRows();
            Alter.Column("AutomaticIssuing").OnTable("tblTestSpecification").AsBoolean().NotNullable();
        }
    }
}
