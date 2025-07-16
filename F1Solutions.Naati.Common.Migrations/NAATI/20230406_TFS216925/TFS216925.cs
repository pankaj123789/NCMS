using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230406_TFS216925
{
    [NaatiMigration(202304061430)]
    public class TFS216925 : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.TFS216925);
        }
    }
}
