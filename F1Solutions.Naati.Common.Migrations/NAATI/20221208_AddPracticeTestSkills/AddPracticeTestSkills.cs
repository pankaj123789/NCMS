using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221208_AddPracticeTestSkills
{
    [NaatiMigration(202212080836)]
    public class AddPracticeTestSkills : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddpracticeTestSkills);
        }
    }
}
