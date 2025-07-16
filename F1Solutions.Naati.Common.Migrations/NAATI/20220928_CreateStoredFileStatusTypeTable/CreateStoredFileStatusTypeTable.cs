using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220928_CreateStoredFileStatusTypeTable
{
    [NaatiMigration(202209280900)]
    public class CreateStoredFileStatusTypeTable : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CreateStoredFileStatusTypeTable);
        }
    }
}
