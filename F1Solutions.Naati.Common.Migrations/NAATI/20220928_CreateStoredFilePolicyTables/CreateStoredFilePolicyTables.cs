using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220928_CreateStoredFilePolicyTables
{
    [NaatiMigration(202209280830)]
    public class CreateStoredFilePolicyTables : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CreateStoredFilePolicyTables);
        }
    }
}
