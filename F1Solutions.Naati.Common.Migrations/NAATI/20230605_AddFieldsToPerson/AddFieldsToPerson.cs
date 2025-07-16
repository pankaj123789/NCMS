using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230605_AddFieldsToPerson
{
    [NaatiMigration(202306051515)]
    public class AddFieldsToPerson : NaatiMigration
    {
        public override void Up()
        {
            {

                Execute.Sql("ALTER TABLE dbo.tblPerson ADD EmailCodeExpireStartDate datetime NULL,AccessDisabledByNcms bit NULL");

                Execute.Sql("ALTER TABLE dbo.tblPerson ADD CONSTRAINT DF_tblPerson_AccessDisabledByNcms DEFAULT 0 FOR AccessDisabledByNcms");

            }
        }
    }
}
