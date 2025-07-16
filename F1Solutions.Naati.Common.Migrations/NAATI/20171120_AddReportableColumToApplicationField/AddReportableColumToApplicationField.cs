
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171120_AddReportableColumToApplicationField
{
    [NaatiMigration(201711201400)]
    public class AddReportableColumToApplicationField : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Reportable").OnTable("tblCredentialApplicationField").AsBoolean().WithDefaultValue(0);

            Execute.Sql("Update tblCredentialApplicationField Set Reportable = 1 where CredentialApplicationTypeId = 1 and CredentialApplicationFieldId in(1,2,3,4)");
            Execute.Sql("Update tblCredentialApplicationField Set Reportable = 1 where CredentialApplicationTypeId = 2 and CredentialApplicationFieldId in(13,14,15,16,17,18,19,20,21,22,24,25,30,31,37)");
            Execute.Sql("Update tblCredentialApplicationField Set Reportable = 1 where CredentialApplicationTypeId = 3 and CredentialApplicationFieldId in(33)");

        }
    }
}
