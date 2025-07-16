using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171119_AddAddressFields
{
    [NaatiMigration(201711191311)]
    public class AddAddressFields : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("State").OnTable("PersonHistory").AsAnsiString(3).Nullable();
            Create.Column("Postcode").OnTable("PersonHistory").AsAnsiString(4).Nullable();
            Create.Column("State").OnTable("ApplicationHistory").AsAnsiString(3).Nullable();
            Create.Column("Postcode").OnTable("ApplicationHistory").AsAnsiString(4).Nullable();
            Create.Column("State").OnTable("CredentialsHistory").AsAnsiString(3).Nullable();
            Create.Column("Postcode").OnTable("CredentialsHistory").AsAnsiString(4).Nullable();
            Create.Column("State").OnTable("CredentialRequestsHistory").AsAnsiString(3).Nullable();
            Create.Column("Postcode").OnTable("CredentialRequestsHistory").AsAnsiString(4).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
