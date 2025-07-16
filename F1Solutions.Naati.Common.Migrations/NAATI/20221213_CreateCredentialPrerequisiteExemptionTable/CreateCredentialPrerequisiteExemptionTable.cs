namespace F1Solutions.Naati.Common.Migrations.NAATI._20221213_CreateCredentialPrerequisiteExemptionTable
{
    [NaatiMigration(202212131545)]
    public class CreateCredentialPrerequisiteExemptionTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CreateCredentialPrerequisiteExemptionTable);
        }
    }
}
