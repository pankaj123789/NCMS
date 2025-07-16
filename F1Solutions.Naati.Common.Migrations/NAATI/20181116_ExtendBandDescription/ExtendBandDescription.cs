
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181116_ExtendBandDescription
{
    [NaatiMigration(201811161104)]
    public class ExtendBandDescription: NaatiMigration
    {
        
        public override void Up()
        {
            Alter.Column("Description").OnTable("tblRubricMarkingBand").AsString(700);
        }
    }
}
