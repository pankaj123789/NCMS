
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180213_AddApiAccessTable
{
    [NaatiMigration(201802131400)]
    public class AddApiAccessTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblApiAccess")
                .WithColumn("ApiAccessId").AsInt32().Identity().PrimaryKey()
                .WithColumn("InstitutionId").AsInt32().NotNullable()
                .WithColumn("PublicKey").AsAnsiString(100).NotNullable()
                .WithColumn("PrivateKey").AsAnsiString(100).NotNullable()
                .WithColumn("CreatedDate").AsDateTime().NotNullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable()
                .WithColumn("ModifiedUserId").AsInt32().NotNullable()
                .WithColumn("Inactive").AsBoolean().NotNullable();

            Create.ForeignKey("FK_ApiAccess_Institution")
                .FromTable("tblApiAccess")
                .ForeignColumn("InstitutionId")
                .ToTable("tblInstitution")
                .PrimaryColumn("InstitutionId");

            Create.ForeignKey("FK_ApiAccess_User")
              .FromTable("tblApiAccess")
              .ForeignColumn("ModifiedUserId")
              .ToTable("tblUser")
              .PrimaryColumn("UserId");
        }
    }
}
