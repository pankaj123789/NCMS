using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221206_AddDefaultValueToStoredFileDeletePolicy
{
    [NaatiMigration(202212061132)]
    public class AddDefaultValueToStoredFileDeletePolicy : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddDefaultValueToStoredFileDeltePolicy);
        }
    }
}
