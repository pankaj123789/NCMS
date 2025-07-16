using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221128_ChangeStoredFileStatus4To3
{
    [NaatiMigration(202211281605)]
    public class ChangeStoredFileStatus4To3 : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.ChangeStoredFileStatus4To3);
        }
    }
}
