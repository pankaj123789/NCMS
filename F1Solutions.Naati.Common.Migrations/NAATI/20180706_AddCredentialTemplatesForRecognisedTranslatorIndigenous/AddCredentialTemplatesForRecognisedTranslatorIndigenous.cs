
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180706_AddCredentialTemplatesForRecognisedTranslatorIndigenous
{
    [NaatiMigration(201807061301)]
    public class AddCredentialTemplatesForRecognisedTranslatorIndigenous : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("INSERT INTO tblCredentialTypeTemplate(CredentialTypeId, StoredFileId, DocumentNameTemplate) SELECT 28,  StoredFileId, DocumentNameTemplate FROM tblCredentialTypeTemplate WHERE CredentialTypeId = 27");
        }
    }
}
