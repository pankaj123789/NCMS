
namespace F1Solutions.Naati.Common.Migrations.NAATI._20220808_TestMaterialReminderDays
{

    [NaatiMigration(202208081601)]
    public class AddMaterialReminderAndReminderDays : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"IF COL_LENGTH('tblTestSpecification','TestMaterialReminder') IS NULL
                            BEGIN
                                ALTER TABLE tblTestSpecification ADD TestMaterialReminder BIT  NOT NULL default 0
                            END");

            Execute.Sql(@"IF COL_LENGTH('tblTestSpecification','TestMaterialReminderDays') IS NULL
                            BEGIN
                                ALTER TABLE tblTestSpecification ADD TestMaterialReminderDays INT  NOT NULL default 0
                            END");
        }
    }
}
