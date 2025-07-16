using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160809_RenameColumnAccreditationLevel
{
    [NaatiMigration(201608091030)]
    public class RenameColumnAccreditationLevel : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("sp_rename 'Accreditation.AccreditationLevelWebDisplay', 'AccreditationLevel', 'COLUMN'");
            Execute.Sql("sp_rename 'Application.AccreditationLevelWebDisplay', 'AccreditationLevel', 'COLUMN'");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
