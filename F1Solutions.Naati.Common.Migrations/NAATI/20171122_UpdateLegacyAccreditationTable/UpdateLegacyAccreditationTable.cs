
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171122_UpdateLegacyAccreditationTable
{
   [NaatiMigration(201711221415)]
    public class UpdateLegacyAccreditationTable : NaatiMigration
    {
        public override void Up()
        {
            Delete.ForeignKey("FK_LegacyAccreditation_Person").OnTable("tblLegacyAccreditation");
            Delete.Column("PersonId").FromTable("tblLegacyAccreditation");

            Create.Column("NAATINumber").OnTable("tblLegacyAccreditation").AsInt32().Nullable();
        }
    }
}
