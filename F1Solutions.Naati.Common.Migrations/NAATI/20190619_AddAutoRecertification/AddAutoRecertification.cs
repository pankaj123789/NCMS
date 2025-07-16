
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190619_AddAutoRecertification
{
    [NaatiMigration(201906191645)]
    public class AddAutoRecertification : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("AllowAutoRecertification").OnTable("tblPerson").AsBoolean().Nullable();

            Update.Table("tblPerson").Set(new { AllowAutoRecertification = 1 }).AllRows();

            Alter.Column("AllowAutoRecertification").OnTable("tblPerson").AsBoolean().NotNullable();
        }
    }
}
