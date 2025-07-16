
namespace F1Solutions.Naati.Common.Migrations.NAATI._20230608_AddEmailCodeToPerson
{
    [NaatiMigration(202306080726)]
    public class AddEmailCodeToPerson : NaatiMigration
    {
        public override void Up()
        {
            {

                Execute.Sql("ALTER TABLE dbo.tblPerson ADD LastEmailCode int NOT NULL CONSTRAINT DF_tblPerson_LastEmailCode DEFAULT 0");

            }
        }
    }
}
