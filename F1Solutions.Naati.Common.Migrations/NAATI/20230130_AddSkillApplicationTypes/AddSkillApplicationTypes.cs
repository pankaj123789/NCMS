using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230130_AddSkillApplicationTypes
{
    [NaatiMigration(202301301048)]
    public class AddSkillApplicationTypes : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddSkillApplicationTypes);
        }
    }
}
