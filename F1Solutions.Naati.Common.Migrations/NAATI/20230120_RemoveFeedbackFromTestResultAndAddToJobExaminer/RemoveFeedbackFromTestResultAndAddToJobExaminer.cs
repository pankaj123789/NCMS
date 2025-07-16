using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230120_RemoveFeedbackFromTestResultAndAddToJobExaminer
{
    [NaatiMigration(202301201305)]
    public class RemoveFeedbackFromTestResultAndAddToJobExaminer : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.RemoveFeedbackFromTestResultAndAddToJobExaminer);
        }
    }
}
