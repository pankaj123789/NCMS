using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170807_FixBadGLcodeReferences
{
    [NaatiMigration(201708071430)]
    public class FixBadGLcodeReferences : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.FixBadGLcodeReferences);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
