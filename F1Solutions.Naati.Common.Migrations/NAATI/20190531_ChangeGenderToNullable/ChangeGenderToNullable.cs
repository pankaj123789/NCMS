
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190531_ChangeGenderToNullable
{
    [NaatiMigration(201905311004)]
    public class ChangeGenderToNullable: NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("Gender").OnTable("tblPerson").AsFixedLengthString(1).Nullable();

            Execute.Sql("Update tblPerson set Gender = null where TRIM(Gender) = ''");
        }
    }
}
