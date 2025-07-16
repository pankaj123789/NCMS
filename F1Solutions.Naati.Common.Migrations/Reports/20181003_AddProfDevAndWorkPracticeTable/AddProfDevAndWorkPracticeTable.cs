using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20181003_AddProfDevAndWorkPracticeTable
{
    [NaatiMigration(201810081707)]
    public class AddProfDevAndWorkPracticeTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("ProfessionalDevelopmentHistory")
               .WithColumn("ProfessionalDevelopmentActivityId").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("PersonId").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("CustomerNumber").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("PractitionerNumber").AsString(50).NotNullable().PrimaryKey()
               .WithColumn("ApplicationID").AsInt32().NotNullable().PrimaryKey()

               .WithColumn("ApplicationStatus").AsString(100).Nullable()

               .WithColumn("CertificationPeriodID").AsInt32().NotNullable().PrimaryKey()

               .WithColumn("CertificationPeriodStartDate").AsDateTime().Nullable()
               .WithColumn("CertificationPeriodOriginalEndDate").AsDateTime().Nullable()
               .WithColumn("CertificationPeriodEndDate").AsDateTime().Nullable()

               .WithColumn("DateCompleted").AsDateTime().Nullable()
               .WithColumn("Description").AsString(500).Nullable()

                .WithColumn("SectionID").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("SectionName").AsString(50).Nullable()
                .WithColumn("CategoryID").AsInt32().NotNullable().PrimaryKey()

               .WithColumn("CategoryName").AsString(50).Nullable()
               .WithColumn("CategoryGroup").AsString(50).Nullable()
               .WithColumn("RequirementID").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("RequirementName").AsString(255).Nullable()
               .WithColumn("Points").AsInt32().Nullable()
               .WithColumn("NumberOfAttachments").AsInt32().Nullable()

               .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
               .WithColumn("ObsoletedDate").AsDateTime().Nullable()
               .WithColumn("DeletedDate").AsDateTime().Nullable();

            Create.Table("WorkPracticeHistory")
               .WithColumn("WorkPracticeId").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("PersonID").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("CustomerNumber").AsInt32().NotNullable().PrimaryKey()
               .WithColumn("PractitionerNumber").AsString(50).NotNullable().PrimaryKey()
               .WithColumn("ApplicationID").AsInt32().NotNullable().PrimaryKey()

               .WithColumn("ApplicationStatus").AsString(100).Nullable()
               .WithColumn("CredentialTypeInternalName").AsString(50).Nullable()
               .WithColumn("CredentialTypeExternalName").AsString(50).Nullable()
               .WithColumn("Skill").AsString(200).Nullable()
               .WithColumn("CertificationPeriodID").AsInt32().Nullable()

               .WithColumn("CertificationPeriodStartDate").AsDateTime().Nullable()
               .WithColumn("CertificationPeriodOriginalEndDate").AsDateTime().Nullable()
               .WithColumn("CertificationPeriodEndDate").AsDateTime().Nullable()

               .WithColumn("DateCompleted").AsDateTime().Nullable()
               .WithColumn("Description").AsString(200).Nullable()
               .WithColumn("Points").AsDecimal(9,1).Nullable()
               .WithColumn("WorkPracticeUnits").AsString(20).Nullable()
               .WithColumn("NumberOfAttachments").AsInt32().Nullable()

               .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
               .WithColumn("ObsoletedDate").AsDateTime().Nullable()
               .WithColumn("DeletedDate").AsDateTime().Nullable();
        }
    }
}
