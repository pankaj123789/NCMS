namespace F1Solutions.Naati.Common.Migrations.NAATI._20200717_AddOnlineState
{
    [NaatiMigration(202007171405)]
    public class AddOnlineState : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("Set IDENTITY_INSERT tluState ON ");
            Execute.Sql("INSERT INTO [dbo].[tluState]([StateId],[State],[Name],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])VALUES(118,'ONL','Online', '0', GETDATE(), 40)");
            Execute.Sql("Set IDENTITY_INSERT tluState OFF ");
            Execute.Sql("Update tblOffice set StateId =118 where OfficeId =316");
        }
    }
}
