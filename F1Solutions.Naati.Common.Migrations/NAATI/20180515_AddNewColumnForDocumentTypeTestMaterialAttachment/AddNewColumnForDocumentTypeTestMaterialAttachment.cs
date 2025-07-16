
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180515_AddNewColumnForDocumentTypeTestMaterialAttachment
{
    [NaatiMigration(201805151000)]
    public class AddNewColumnForDocumentTypeTestMaterialAttachment : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("MergeDocument")
                .OnTable("tblTestMaterialAttachment")
                .AsBoolean()
                .WithDefaultValue(0)
                .NotNullable();

            Create.Column("MergeDocument")
                .OnTable("tluDocumentType")
                .AsBoolean()
                .WithDefaultValue(0)
                .NotNullable();
        }
    }
}
