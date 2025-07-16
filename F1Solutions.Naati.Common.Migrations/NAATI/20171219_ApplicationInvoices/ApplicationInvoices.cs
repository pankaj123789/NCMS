
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171219_ApplicationInvoices
{
    [NaatiMigration(201712191200)]
    public class ApplicationInvoices : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Applicant").OnTable("tblEmailTemplate").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Column("Sponsor").OnTable("tblEmailTemplate").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Column("Invoice").OnTable("tblEmailTemplate").AsBoolean().NotNullable().WithDefaultValue(false);

            Insert.IntoTable("tluDocumentType").Row(new
            {
                Name = "Invoice",
                DisplayName = "Invoice",
                ExaminerToolsDownload = "0",
                ExaminerToolsUpload = "0",
                DocumentTypeCategoryId = "3"
            });
        }
    }
}
