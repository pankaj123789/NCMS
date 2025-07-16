namespace F1Solutions.Naati.Common.Migrations.NAATI._20171123_TrimValueInInstitutionName
{
   [NaatiMigration(201711231600)]
    public class TrimValueInInstitutionName : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"Update tblInstitutionName set Name = RTRIM(Name)");
        }
    }
}
