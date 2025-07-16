using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191105_AddEndorsedQualificationTable
{
    [NaatiMigration(201911051347)]
    public class AddEndorsedQualificationTable: NaatiMigration

    {
        public override void Up()
        {
            Create.Table("EndorsedQualificationHistory")

                .WithColumn("EndorsedQualificationId").AsInt32().NotNullable().PrimaryKey()

                .WithColumn("InstitutionId").AsInt32().NotNullable()
                .WithColumn("NAATINumber").AsInt32().NotNullable()
                .WithColumn("InstitutionName").AsString(100).NotNullable()

                .WithColumn("Location").AsString(200).NotNullable()
                .WithColumn("Qualification").AsString(200).NotNullable()

                .WithColumn("CredentialTypeId").AsInt32().NotNullable()
                .WithColumn("CredentialTypeInternalName").AsString(50).NotNullable()
                .WithColumn("CredentialTypeExternalName").AsString(50).NotNullable()

                .WithColumn("EndorsementPeriodFrom").AsDateTime().NotNullable()
                .WithColumn("EndorsementPeriodTo").AsDateTime().NotNullable()
                .WithColumn("Notes").AsString(1000).Nullable()
                .WithColumn("Active").AsBoolean().NotNullable()
                
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();
        }
    }
}
