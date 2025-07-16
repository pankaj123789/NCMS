
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190301_CandidateBrief
{
    [NaatiMigration(201903011151)]
    public class CandidateBrief : NaatiMigration
    {
        public override void Up()
        {
            CreateBriefTables();
            AddFileNameToAttachment();
        }

        private void CreateBriefTables()
        {
            Create.Column("CandidateBriefRequired").OnTable("tblTestComponentType").AsBoolean().NotNullable().WithDefaultValue(false);
            Create.Column("CandidateBriefAvailabilityDays").OnTable("tblTestComponentType").AsInt32().Nullable();

            Create.Table("tblCandidateBrief")
                .WithColumn("CandidateBriefId").AsInt32().Identity().PrimaryKey()
                .WithColumn("TestSittingId").AsInt32().NotNullable().ForeignKey("tblTestSitting", "TestSittingId")
                .WithColumn("TestMaterialAttachmentId").AsInt32().NotNullable().ForeignKey("tblTestMaterialAttachment", "TestMaterialAttachmentId")
                .WithColumn("EmailedDate").AsDateTime().Nullable();
        }

        private void AddFileNameToAttachment()
        {
            Alter.Table("tblEmailMessageAttachment").AddColumn("FileName").AsString(255).Nullable();
            Execute.Sql("update a set a.filename = s.FilenAME  FROM tblEmailMessageAttachment a inner join tblStoredFile s on a.storedFileId = s.StoredFileId ");
            Alter.Column("FileName").OnTable("tblEmailMessageAttachment").AsString(255).NotNullable();
        }

       
    }
}
