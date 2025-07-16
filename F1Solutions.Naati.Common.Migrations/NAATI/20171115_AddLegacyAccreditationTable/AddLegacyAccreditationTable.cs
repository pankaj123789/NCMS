
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171115_AddLegacyAccreditationTable
{
   [NaatiMigration(201711151400)]
    public class AddLegacyAccreditationTable : NaatiMigration
    {
        public override void Up()
        {
            if (!Schema.Table("tblLegacyAccreditation").Exists())
            {
                Create.Table("tblLegacyAccreditation")
                    .WithColumn("LegacyAccreditationId").AsInt32().Identity().PrimaryKey()
                    .WithColumn("PersonId").AsInt32().NotNullable()
                    .WithColumn("AccreditationId").AsInt32().NotNullable()
                    .WithColumn("Level").AsString(500).NotNullable()
                    .WithColumn("Category").AsString(500).NotNullable()
                    .WithColumn("Direction").AsString(500).NotNullable()
                    .WithColumn("Language1").AsString(500).NotNullable()
                    .WithColumn("Language2").AsString(500).NotNullable()
                    .WithColumn("StartDate").AsDateTime().NotNullable()
                    .WithColumn("ExpiryDate").AsDateTime().Nullable()
                    .WithColumn("IncludeInOD").AsBoolean().WithDefaultValue(0);

                Create.ForeignKey("FK_LegacyAccreditation_Person")
                    .FromTable("tblLegacyAccreditation")
                    .ForeignColumn("PersonId")
                    .ToTable("tblPerson")
                    .PrimaryColumn("PersonId");
            }
        }
    }
}
