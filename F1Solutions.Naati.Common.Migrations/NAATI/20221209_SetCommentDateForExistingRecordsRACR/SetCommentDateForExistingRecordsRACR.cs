using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221209_SetCommentDateForExistingRecordsRACR
{
    [NaatiMigration(202212091015)]
    public class SetCommentDateForExistingRecordsRACR : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.SetCommentDateForExistingRecordsRACR);
        }
    }
}
