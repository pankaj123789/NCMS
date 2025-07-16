using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220928_AddContraintStoredFileAndStatus
{
    [NaatiMigration(202209280936)]
    public class AddConstraintStoredFileAndStatus : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddConstraintStoredFileAndStatus);
        }
    }
}
