
namespace F1Solutions.Naati.Common.Migrations.NAATI._20181022_AddAllowPaidReviewFlag
{
    [NaatiMigration(201810221603)]
    public class AddAllowPaidReviewFlag : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblCredentialApplicationTypeCredentialType ")
                .AddColumn("AllowPaidReview").AsBoolean().WithDefaultValue(0);
            Alter.Table("tblCredentialType")
                .AddColumn("AllowBackdating").AsBoolean().WithDefaultValue(0);
            Alter.Table("tblCredentialType")
                .AddColumn("Level").AsInt32().WithDefaultValue(0);

        }
    }
}
