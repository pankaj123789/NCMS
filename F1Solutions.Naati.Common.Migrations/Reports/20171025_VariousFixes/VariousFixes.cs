using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171025_VariousFixes
{
    [NaatiMigration(201710260900)]
    public class VariousFixes : NaatiMigration
    {
        public override void Up()
        {
            Alter.Column("PractitionerNumber").OnTable("ApplicationHistory").AsAnsiString(50).Nullable();
            Alter.Column("PractitionerNumber").OnTable("CredentialRequestsHistory").AsAnsiString(50).Nullable();
            Alter.Column("PractitionerNumber").OnTable("CredentialsHistory").AsAnsiString(50).Nullable();
            Alter.Column("PractitionerNumber").OnTable("PersonHistory").AsAnsiString(50).Nullable();
            Alter.Column("PractitionerNumber").OnTable("PersonHistory").AsAnsiString(50).Nullable();
            Alter.Column("DirectionDisplayName").OnTable("CredentialRequestsHistory").AsString(105);
            Alter.Column("DirectionDisplayName").OnTable("CredentialsHistory").AsString(105);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
