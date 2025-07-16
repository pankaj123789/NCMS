
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180426_RemoveApplicationTypeFromTestSession
{
    [NaatiMigration(201804262000)]
    public class RemoveApplicationTypeFromTestSession : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE [dbo].[tblTestSession] DROP CONSTRAINT [FK_TestSession_CredentialApplicationType]
                          ALTER TABLE [dbo].[tblTestSession] DROP COLUMN [CredentialApplicationTypeId]");
        }
    }
}
