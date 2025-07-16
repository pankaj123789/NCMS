using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170728_AddFinanceCorrespondenceCategory
{
    [NaatiMigration(201707281400)]
    public class AddFinanceCorrespondenceCategory : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"Insert into tluCorrespondenceCategory values(22, 'Finance', 'Third party invoice created', NULL)");
            Execute.Sql(@"Insert into tblsystemvalue values('FinanceCorrespondenceCategoryId', 22, NULL)");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
