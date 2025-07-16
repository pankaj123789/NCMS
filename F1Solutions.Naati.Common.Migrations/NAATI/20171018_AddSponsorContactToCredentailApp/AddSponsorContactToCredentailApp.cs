namespace F1Solutions.Naati.Common.Migrations.NAATI._20171018_AddSponsorContactToCredentailApp
{
   [NaatiMigration(201710180929)]
    public class AddSponsorContactToCredentailApp : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("SponsorInstitutionContactPersonId").OnTable("tblCredentialApplication").AsInt32().Nullable();
        }
    }
}
