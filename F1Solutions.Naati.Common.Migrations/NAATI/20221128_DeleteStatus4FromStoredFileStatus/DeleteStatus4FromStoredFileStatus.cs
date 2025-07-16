using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221128_DeleteStatus4FromStoredFileStatus
{
    [NaatiMigration(202211281615)]
    public class DeleteStatus4FromStoredFileStatus : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.DeleteStatus4FromStoredFileStatus);
        }
    }
}
