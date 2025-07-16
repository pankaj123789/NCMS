using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170821_ChangesToContacts
{
    [NaatiMigration(201708221636)]
    public class ChangesToContacts : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddPrimaryContact);
            Execute.Sql(Sql.DropAddressColumn);
            Execute.Sql(Sql.DropEmailColumn);
            Execute.Sql(Sql.DropPhoneColumn);
            Execute.Sql(Sql.DropContactTypeTable);
            Execute.Sql(Sql.DropProcedureContactTypeSelect);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
