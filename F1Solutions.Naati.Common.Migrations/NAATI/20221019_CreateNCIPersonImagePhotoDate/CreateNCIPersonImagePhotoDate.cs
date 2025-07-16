using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221019_CreateNCIPersonImagePhotoDate
{
    [NaatiMigration(202210191140)]
    public class CreateNCIPersonImagePhotoDate : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CreateNCItblPersonImagePhotoDate);
        }
    }
}
