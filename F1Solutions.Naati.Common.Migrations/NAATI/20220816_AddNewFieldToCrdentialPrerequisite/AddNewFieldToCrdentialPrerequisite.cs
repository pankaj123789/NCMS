using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220816_AddNewFieldToCrdentialPrerequisite
{
    [NaatiMigration(202208161541)]
    public class AddNewFieldToCrdentialPrerequisite : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddNewFieldToCredentialPrerequisite);
        }
    }
}
