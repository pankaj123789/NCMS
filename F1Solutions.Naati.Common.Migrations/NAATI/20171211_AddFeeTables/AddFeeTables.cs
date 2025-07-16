
namespace F1Solutions.Naati.Common.Migrations.NAATI._20171211_AddFeeTables
{
    [NaatiMigration(201712111500)]
    public class AddFeeTables : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblFeeType")
                .WithColumn("FeeTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50).NotNullable()
                .WithColumn("DisplayName").AsString(50).NotNullable();

            Create.Table("tblCredentialFeeProduct")
                .WithColumn("CredentialFeeProductId").AsInt32().Identity().PrimaryKey()
                .WithColumn("CredentialApplicationTypeId").AsInt32().NotNullable()
                .WithColumn("CredentialTypeId").AsInt32().Nullable()
                .WithColumn("FeeTypeId").AsInt32().NotNullable()
                .WithColumn("ProductSpecificationId").AsInt32().NotNullable();

            Create.ForeignKey("FK_CredentialFeeProduct_CredentialApplicationType")
                .FromTable("tblCredentialFeeProduct")
                .ForeignColumn("CredentialApplicationTypeId")
                .ToTable("tblCredentialApplicationType")
                .PrimaryColumn("CredentialApplicationTypeId");

            Create.ForeignKey("FK_CredentialFeeProduct_CredentialType")
                .FromTable("tblCredentialFeeProduct")
                .ForeignColumn("CredentialTypeId")
                .ToTable("tblCredentialType")
                .PrimaryColumn("CredentialTypeId");

            Create.ForeignKey("FK_CredentialFeeProduct_ProductSpecification")
                .FromTable("tblCredentialFeeProduct")
                .ForeignColumn("ProductSpecificationId")
                .ToTable("tblProductSpecification")
                .PrimaryColumn("ProductSpecificationId");
        }
    }
}
