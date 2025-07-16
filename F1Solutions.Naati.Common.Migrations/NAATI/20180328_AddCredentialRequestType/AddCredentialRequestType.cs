namespace F1Solutions.Naati.Common.Migrations.NAATI._20180328_AddCredentialRequestType
{
    [NaatiMigration(201803281500)]
    public class AddCredentialRequestType : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblCredentialRequestPathType")
                .WithColumn("CredentialRequestPathTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsAnsiString(50)
                .WithColumn("DisplayName").AsString(50);
            // Adding default value, this will be complted  in post migration scritp

            Execute.Sql("SET IDENTITY_INSERT[dbo].[tblCredentialRequestPathType] ON ");
            Insert.IntoTable("tblCredentialRequestPathType").Row(new {CredentialRequestPathTypeId = 1, Name = "New", DisplayName = "New"});
            Execute.Sql("SET IDENTITY_INSERT[dbo].[tblCredentialRequestPathType] OFF ");

            Create.Column("CredentialRequestPathTypeId").OnTable("tblCredentialRequest").AsInt32().Nullable();
            Execute.Sql("UPDATE TBLCREDENTIALREQUEST  SET CredentialRequestPathTypeId = 1");
            Alter.Column("CredentialRequestPathTypeId").OnTable("tblCredentialRequest").AsInt32().NotNullable();

            Create.ForeignKey("FK_CredentialRequest_CredentialRequestPathType")
                .FromTable("tblCredentialRequest")
                .ForeignColumn("CredentialRequestPathTypeId")
                .ToTable("tblCredentialRequestPathType")
                .PrimaryColumn("CredentialRequestPathTypeId");

        }
    }
}
