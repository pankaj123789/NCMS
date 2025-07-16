
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171218_CredentialCategoryWorkPractice
{
    [NaatiMigration(201712181245)]
    public class CredentialCategoryWorkPractice : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialCategory")
                .AddColumn("WorkPracticePoints")
                .AsInt32()
                .Nullable()
                .AddColumn("WorkPracticeUnits")
                .AsAnsiString(20)
                .Nullable();
        }
    }
}
