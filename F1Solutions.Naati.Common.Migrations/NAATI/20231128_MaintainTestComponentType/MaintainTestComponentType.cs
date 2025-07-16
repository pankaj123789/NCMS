using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20231128_MaintainTestComponentType
{
    [NaatiMigration(202311280853)]
    public class MaintainTestComponentType : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql("update tblTestComponentType set CandidateBriefRequired = 1, CandidateBriefAvailabilityDays = 3 where TestComponentTypeId = 31");
        }
    }
}
