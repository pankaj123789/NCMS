
namespace F1Solutions.Naati.Common.Migrations.NAATI._20170912_LanguageSchemaChanges
{
    [NaatiMigration(201709121700)]
    public class LanguageSchemaChanges : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblLanguageGroup")
                .WithColumn("LanguageGroupId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50).Unique().NotNullable();

            Delete.Column("GroupLanguageId").FromTable("tblLanguage");
            Delete.Column("Indigenous").FromTable("tblLanguage");
            Create.Column("LanguageGroupId").OnTable("tblLanguage").AsInt32().Nullable();

            Create.ForeignKey("FK_Language_LanguageGroup")
                .FromTable("tblLanguage")
                .ForeignColumn("LanguageGroupId")
                .ToTable("tblLanguageGroup")
                .PrimaryColumn("LanguageGroupId");

            Create.Table("tblAlternateLanguageName")
                .WithColumn("AlternateLanguageNameId").AsInt32().Identity().PrimaryKey()
                .WithColumn("LanguageId").AsInt32()
                .WithColumn("AlternateName").AsString(50);

            Create.ForeignKey("FK_AlternateLanguageName_Language")
                .FromTable("tblAlternateLanguageName")
                .ForeignColumn("LanguageId")
                .ToTable("tblLanguage")
                .PrimaryColumn("LanguageId");
        }
    }
}
