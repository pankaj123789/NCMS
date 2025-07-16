using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221102_CredentialQrCodes
{
    [NaatiMigration(202211020900)]
    public class CredentialQrCodes : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CredentialQrCodes);
        }
    }
}
