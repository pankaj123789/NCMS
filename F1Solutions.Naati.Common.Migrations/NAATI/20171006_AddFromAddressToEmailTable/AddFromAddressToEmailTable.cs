using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20171006_AddFromAddressToEmailTable
{
   [NaatiMigration(201710060946)]
    public class AddFromAddressToEmailTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("FromAddress").OnTable("tblEmailTemplate").AsString(500).Nullable();
            Create.Column("FromAddress").OnTable("tblEmailMessage").AsString(500).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }

    
    }
}
