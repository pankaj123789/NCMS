using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20230110_AddModifiedDateToQrCode
{
    [NaatiMigration(202301101152)]
    public class AddModifiedDateToQrCode : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.AddModifiedDateToQrCode);
        }
    }
}
