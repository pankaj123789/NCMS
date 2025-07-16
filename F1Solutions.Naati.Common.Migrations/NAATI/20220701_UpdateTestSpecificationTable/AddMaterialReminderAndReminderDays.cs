
namespace F1Solutions.Naati.Common.Migrations.NAATI._20220701_UpdateTestSpecificationTable
{

    [NaatiMigration(202207011201)]
    public class AddMaterialReminderAndReminderDays : NaatiMigration
    {
        public override void Up()
        {
            //Execute.Sql("ALTER TABLE tblTestSpecification ADD TestMaterialReminder BIT  NOT NULL default 'FALSE'");
            //Execute.Sql("ALTER TABLE tblTestSpecification ADD TestMaterialReminderDays INT  NOT NULL default 'FALSE'");
        }
    }

}
