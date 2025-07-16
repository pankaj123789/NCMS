
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171218_LogbookStoredFileType
{
    [NaatiMigration(201712181000)]
    public class LogbookStoredFileType : NaatiMigration
    {
        public override void Up()
        {
            Insert.IntoTable("tluDocumentType").Row(new
            {
                Name = "WorkPractice",
                DisplayName = "Work Practice",
                ExaminerToolsDownload = 0,
                ExaminerToolsUpload = 0,
                DocumentTypeCategoryId = 2
            });
            Insert.IntoTable("tluDocumentType").Row(new
            {
                Name = "ProfessionalDevelopmentActivity",
                DisplayName = "Professional Development Activity",
                ExaminerToolsDownload = 0,
                ExaminerToolsUpload = 0,
                DocumentTypeCategoryId = 2
            });
        }
    }
}
