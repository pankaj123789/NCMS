using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230130_AddNewSkillsForPracrticeTestCertifiedInterpreter
{
    [NaatiMigration(202301301617)]
    public class AddNewSkillsForPracticeTest : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddNewSkillsForPracticeTests);
        }
    }
}
