using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230103_AddFieldsToQrCode
{
    [NaatiMigration(202301031645)]
    public class AddFieldsToQrCode : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddFieldsToQrCodes);
        }
    }
}
