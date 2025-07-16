
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171218_ApplicationsOther
{
    [NaatiMigration(201712181400)]
    public class ApplicationsOther : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(
                @"update tluDocumentType set name = 'ApplicationsOther', displayName ='Other Application Related' where Name = 'Other'
                         update tblStoredFile set ExternalFileId=REPLACE(ExternalFileId,'Other\','ApplicationsOther\') where DocumentTypeId =17");
        }
    }
}
