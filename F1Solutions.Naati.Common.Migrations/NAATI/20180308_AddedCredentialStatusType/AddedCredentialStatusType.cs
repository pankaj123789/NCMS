
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180308_AddedCredentialStatusType
{
    [NaatiMigration(201803081100)]
    public class AddedCredentialStatusType : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialStatusType")
                .WithColumn("CredentialStatusTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);
        }
    }
}
