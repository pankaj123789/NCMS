using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221209_AddCommentDateToRACR
{
    [NaatiMigration(202212091000)]
    public class AddCommentDateToRACR : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddCommentDateToRACR);
        }
    }
}
