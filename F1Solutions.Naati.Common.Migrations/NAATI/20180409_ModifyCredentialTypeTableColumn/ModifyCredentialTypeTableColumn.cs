
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180409_ModifyCredentialTypeTableColumn
{
    [NaatiMigration(201804091500)]
    public class ModifyCredentialTypeTableColumn : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"EXEC sp_rename 'tblCredentialType.UpgradePath', 'DisplayOrder', 'COLUMN'");
            // Makes the colums values unique, Final values will be set in the post migration script
            Execute.Sql(@"UPDATE tblCredentialType SET DisplayOrder = CREDENTIALTYPEID +1000");

            Execute.Sql(@"
                ALTER TABLE [dbo].[tblCredentialType] 
                ADD  CONSTRAINT [U_tblCredentialType]
                UNIQUE NONCLUSTERED ([DisplayOrder])");
        }
    }
}
