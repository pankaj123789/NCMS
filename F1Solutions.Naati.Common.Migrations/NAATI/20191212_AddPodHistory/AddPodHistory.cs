using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20191212_AddPodHistory
{
    [NaatiMigration(201912121400)]
    public class AddPodHistory :NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblPodHistory")
                .WithColumn("PodHistoryId").AsInt32().Identity().PrimaryKey()
                .WithColumn("PodName").AsString(500).NotNullable()
                .WithColumn("StartedDate").AsDateTime().NotNullable()
                .WithColumn("TerminationDate").AsDateTime().Nullable()
                .WithColumn("FolderPath").AsString(1500).NotNullable()
                .WithColumn("DeletionError").AsString(int.MaxValue).Nullable();
        }
    }
}
