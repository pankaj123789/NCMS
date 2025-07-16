
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180329_AddDisplayOrderColumn
{
    [NaatiMigration(201803021201)]
    public class AddDisplayOrderColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("DisplayOrder").OnTable("tblCredentialRequestStatusType").AsInt32().NotNullable().WithDefaultValue(0);
            Create.Column("DisplayOrder").OnTable("tblCredentialApplicationStatusType").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
