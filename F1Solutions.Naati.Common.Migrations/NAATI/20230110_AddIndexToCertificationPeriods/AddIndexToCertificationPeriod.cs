using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230110_AddIndexToCertificationPeriods
{
    [NaatiMigration(202301100919)]
    public class AddIndexToCertificationPeriod : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddIndexToCertificationPeriod);
        }
    }
}
