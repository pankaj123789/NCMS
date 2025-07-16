using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20221021_CreatIndexStoredFileDocumentTypeId
{
    [NaatiMigration(202210211555)]
    public class CreateIndexStoredFileDocumentTypeId : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(sql.CreateIndexStoredFileDocumentTypeId);
        }
    }
}
