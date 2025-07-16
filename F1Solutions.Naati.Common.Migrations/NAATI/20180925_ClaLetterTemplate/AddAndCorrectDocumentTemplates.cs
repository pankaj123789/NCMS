
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180925_ClaLetterTemplate
{
    [NaatiMigration(201809251100)]
    public class AddAndCorrectDocumentTemplates : NaatiMigration
    {
        public override void Up()
        {
            // add the CLA letter template
            Execute.Sql(Sql.ClaLetterTemplate);

            // just happened to notice that the document type was wrong on this row
            Execute.Sql("UPDATE tblStoredFile SET DocumentTypeId = 19 WHERE StoredFileId = 101995");
        }
    }
}
