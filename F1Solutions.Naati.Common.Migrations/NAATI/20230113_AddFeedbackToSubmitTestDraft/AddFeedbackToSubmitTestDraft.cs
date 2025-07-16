using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230113_AddFeedbackToSubmitTestDraft
{
    [NaatiMigration(202301131515)]
    public class AddFeedbackToSubmitTestDraft : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddFeedbackToSubmitTestDraft);
        }
    }
}
