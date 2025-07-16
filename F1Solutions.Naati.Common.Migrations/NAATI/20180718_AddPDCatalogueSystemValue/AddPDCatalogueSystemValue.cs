
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180718_AddPDCatalogueSystemValue
{
    [NaatiMigration(201807181500)]
    public class AddPdCatalogueSystemValue : NaatiMigration
    {
        public override void Up()
        {
           Execute.Sql(@"insert into tblSystemValue (valueKey,value) values ('PdCatalogue','https://www.naati.com.au/media/2007/recertification-pd-catalogue-finalpdf.pdf')");
        }
    }
}
