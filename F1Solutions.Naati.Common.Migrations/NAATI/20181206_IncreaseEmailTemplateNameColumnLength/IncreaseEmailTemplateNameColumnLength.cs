
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181206_IncreaseEmailTemplateNameColumnLength
{
    [NaatiMigration(201812061400)]
    public class IncreaseEmailTemplateNameColumnLength : NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("Name").OnTable("tblEmailTemplate").AsString(100).NotNullable();
        }
    }
}
