
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180907_Recertification
{
    [NaatiMigration(201809070000)]
    public class Recertification : NaatiMigration
    {
        public override void Up()
        {
            if (!Schema.Table("tblProfessionalDevelopmentCredentialApplication").Exists())
            {
                Create.Table("tblProfessionalDevelopmentCredentialApplication")
                    .WithColumn("ProfessionalDevelopmentCredentialApplicationId").AsInt32().Identity().PrimaryKey()
                    .WithColumn("CredentialApplicationId").AsInt32()
                    .WithColumn("ProfessionalDevelopmentActivityId").AsInt32();

                Create.ForeignKey("FK_ProfessionalDevelopmentCredentialApplication_ProfessionalDevelopment")
                    .FromTable("tblProfessionalDevelopmentCredentialApplication")
                    .ForeignColumn("ProfessionalDevelopmentActivityId")
                    .ToTable("tblProfessionalDevelopmentActivity")
                    .PrimaryColumn("ProfessionalDevelopmentActivityId");

                Create.ForeignKey("FK_ProfessionalDevelopmentCredentialApplication_CredentialApplication")
                    .FromTable("tblProfessionalDevelopmentCredentialApplication")
                    .ForeignColumn("CredentialApplicationId")
                    .ToTable("tblCredentialApplication")
                    .PrimaryColumn("CredentialApplicationId");


                Create.Table("tblWorkPracticeCredentialRequest")
                    .WithColumn("WorkPracticeCredentialRequestId").AsInt32().Identity().PrimaryKey()
                    .WithColumn("WorkPracticeId").AsInt32()
                    .WithColumn("CredentialRequestId").AsInt32();

                Create.ForeignKey("FK_WorkPracticeCredentialRequest_WorkPractice")
                    .FromTable("tblWorkPracticeCredentialRequest")
                    .ForeignColumn("WorkPracticeId")
                    .ToTable("tblWorkPractice")
                    .PrimaryColumn("WorkPracticeId");

                Create.ForeignKey("FK_WorkPracticeCredentialRequest_CredentialRequest")
                    .FromTable("tblWorkPracticeCredentialRequest")
                    .ForeignColumn("CredentialRequestId")
                    .ToTable("tblCredentialRequest")
                    .PrimaryColumn("CredentialRequestId");


                Create.Table("tblRecertification")
                    .WithColumn("RecertificationId").AsInt32().Identity().PrimaryKey()
                    .WithColumn("CredentialApplicationId").AsInt32()
                    .WithColumn("CertificationPeriodId").AsInt32();

                Create.ForeignKey("FK_Recertification_CredentialApplication")
                    .FromTable("tblRecertification")
                    .ForeignColumn("CredentialApplicationId")
                    .ToTable("tblCredentialApplication")
                    .PrimaryColumn("CredentialApplicationId");

                Create.ForeignKey("FK_Recertification_CertificationPeriod")
                    .FromTable("tblRecertification")
                    .ForeignColumn("CertificationPeriodId")
                    .ToTable("tblCertificationPeriod")
                    .PrimaryColumn("CertificationPeriodId");

                Insert.IntoTable("tblSystemValue")
                    .Row(new { ValueKey = "CertificationPeriodRecertifyExpiry", Value = "3" });
            }
        }
    }
}
