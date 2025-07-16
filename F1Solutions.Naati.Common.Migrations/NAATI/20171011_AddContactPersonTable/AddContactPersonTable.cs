namespace F1Solutions.Naati.Common.Migrations.NAATI._20171011_AddContactPersonTable
{
   [NaatiMigration(201710111436)]
    public class AddContactPersonTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblContactPerson")
                .WithColumn("ContactPersonId").AsInt32().Identity().PrimaryKey()
                .WithColumn("InstitutionId").AsInt32().NotNullable()
                .WithColumn("Name").AsString(500).NotNullable()
                .WithColumn("Email").AsString(500).NotNullable()
                .WithColumn("Phone").AsString(500).Nullable()
                .WithColumn("PostalAddress").AsString(int.MaxValue).Nullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("Inactive").AsBoolean().WithDefaultValue(0);
                

            Create.ForeignKey("FK_ContactPerson_Institution")
                .FromTable("tblContactPerson")
                .ForeignColumn("InstitutionId")
                .ToTable("tblInstitution")
                .PrimaryColumn("InstitutionId");
        }
    }
}
