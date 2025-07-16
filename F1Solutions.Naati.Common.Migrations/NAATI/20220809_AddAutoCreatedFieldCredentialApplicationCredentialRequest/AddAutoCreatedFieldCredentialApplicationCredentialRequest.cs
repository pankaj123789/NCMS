using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20220809_AddAutoCreatedFieldCredentialApplicationCredentialRequest
{
    [NaatiMigration(202208091024)]
    public class AddAutoCreatedFieldToTblCredentialApplicationAndTblCredentialRequest : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(Sql.AddAutoCreatedFieldCredentialApplicationCredentialRequest);
        }
    }
}
