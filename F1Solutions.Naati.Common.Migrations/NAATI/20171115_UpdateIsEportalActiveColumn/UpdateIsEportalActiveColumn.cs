using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20171115_UpdateIsEportalActiveColumn
{
    [NaatiMigration(201711151300)]
    public class UpdateIsEportalActiveColumn : NaatiMigration
    {
        
        public override void Up()
        {
           Execute.Sql("update tblPerson set IsEportalActive = 0");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
