
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180621_ExtendTestComponentNamLength
{

    [NaatiMigration(201806210901)]
    public class ExtendTestComponentNamLength : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblTestComponent").AlterColumn("Name").AsString(100).NotNullable();
        }
    }
}
