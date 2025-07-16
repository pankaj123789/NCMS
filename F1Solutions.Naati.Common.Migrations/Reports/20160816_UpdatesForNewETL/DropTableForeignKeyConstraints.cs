using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20160816_UpdatesForNewETL
{
    [NaatiMigration(201608160922)]
    public class DropTableForeignKeyConstraints : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("ALTER TABLE [Internal].[Accreditation] DROP CONSTRAINT FK_Accreditation_Application");
            Execute.Sql("ALTER TABLE [Internal].[Application] DROP CONSTRAINT FK_Application_Person");
            Execute.Sql("ALTER TABLE [Internal].[Revalidation] DROP CONSTRAINT FK_Revalidation_Application");
            Execute.Sql("ALTER TABLE [Internal].[Test] DROP CONSTRAINT FK_Test_Application");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
