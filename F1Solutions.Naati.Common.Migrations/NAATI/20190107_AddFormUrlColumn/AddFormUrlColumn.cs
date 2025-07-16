
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190107_AddFormUrlColumn
{
    [NaatiMigration(201901071404)]
    public class AddFormUrlColumn : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("Url").OnTable("tblCredentialApplicationForm").AsString(50).Nullable();
            Execute.Sql("Update tblCredentialApplicationForm set Url = cast(CredentialApplicationFormId as nvarchar) ");
     
            Alter.Column("Url").OnTable("tblCredentialApplicationForm").AsString(50).NotNullable();

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialApplicationForm] 
                ADD  CONSTRAINT [U_tblCredentialApplicationForm]
                UNIQUE NONCLUSTERED ([Url])");
        }
    }
}
