namespace F1Solutions.Naati.Common.Migrations.NAATI._20220713_AddNewSystemValueRolePlayerAvailable
{
    [NaatiMigration(202207131239)]
    public class AddNewSystemValueRolePlayerAvailableSetToFalse : NaatiMigration
    {
        public override void Up()
        {
            //this.ExecuteSql("INSERT INTO tblSystemValue (ValueKey, Value, ModifiedByNaati, ModifiedDate, ModifiedUser) VALUES ('RolePlayerAvailable', 'FALSE', 0, GETDATE(), 40)");
        }
    }
}
