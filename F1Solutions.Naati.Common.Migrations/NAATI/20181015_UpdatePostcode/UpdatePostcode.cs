
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181015_UpdatePostcode
{
    [NaatiMigration(201810151601)]
    public class UpdatePostcodeTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("update tblPostcode set Postcode = 2557 where SuburbId in (SELECT SuburbId FROM tblSuburb where Suburb = 'Catherine Field')");
        }
    }
}
