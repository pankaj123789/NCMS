using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221012_UpdateStoredFileColumns
{
    [NaatiMigration(202210120300)]
    class UpdateTblStoredFileColumns : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.UpdateTblStoredFileColumns);
        }
    }
}
