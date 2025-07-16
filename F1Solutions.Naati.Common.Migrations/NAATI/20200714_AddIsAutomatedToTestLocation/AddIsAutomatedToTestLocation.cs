namespace F1Solutions.Naati.Common.Migrations.NAATI._20200714_AddIsAutomatedToTestLocation
{
    //[NaatiMigration(202007141141)]
    public class AddIsAutomatedToTestLocation : NaatiMigration
    {
        public override void Up()
        {
            //Alter.Table("tblTestLocation").AddColumn("IsAutomated").AsBoolean().Nullable();
            //Update.Table("tblTestLocation").Set(new { IsAutomated = false }).AllRows();
            //Alter.Table("tblTestLocation").AlterColumn("IsAutomated").AsBoolean().NotNullable();

            //Alter.Table("tblTestSession").AddColumn("Synced").AsBoolean().Nullable();
            //Update.Table("tblTestSession").Set(new { Synced = false }).AllRows();
            //Alter.Table("tblTestSession").AlterColumn("Synced").AsBoolean().NotNullable();
        }
    }
}
