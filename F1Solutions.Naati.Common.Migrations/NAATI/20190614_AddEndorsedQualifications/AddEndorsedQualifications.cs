
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190614_AddEndorsedQualifications
{
    [NaatiMigration(201906141004)]
    public class AddEndorsedQualifications : NaatiMigration
    {
        public override void Up()
        {

            Create.Table("tblEndorsedQualification")
                .WithColumn("EndorsedQualificationId").AsInt32().PrimaryKey().Identity()
                .WithColumn("InstitutionId").AsInt32()
                .WithColumn("Location").AsString(200)
                .WithColumn("Qualification").AsString(200)
                .WithColumn("CredentialTypeId").AsInt32()
                .WithColumn("EndorsementPeriodFrom").AsDate()
                .WithColumn("EndorsementPeriodTo").AsDate()
                .WithColumn("Notes").AsString(1000).Nullable()
                .WithColumn("Active").AsBoolean();

            Create.ForeignKey("FK_EndorsedQualification_Institution")
                .FromTable("tblEndorsedQualification")
                .ForeignColumn("InstitutionId")
                .ToTable("tblInstitution")
                .PrimaryColumn("InstitutionId");

            Create.ForeignKey("FK_EndorsedQualification_CredentialType")
                .FromTable("tblEndorsedQualification")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");
        }
    }
}
