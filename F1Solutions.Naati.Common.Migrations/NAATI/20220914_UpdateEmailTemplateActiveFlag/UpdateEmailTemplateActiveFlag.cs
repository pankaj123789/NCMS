using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220914_UpdateEmailTemplateActiveFlag
{
    [NaatiMigration(202209141104)]
    public class UpdateEmailTemplateActiveFlag : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.UpdateEmailTemplateActiveFlag);
        }
    }
}
