using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20191201_AddSystemValueTable
{
    [NaatiMigration(201912011800)]
    public class AddSystemValueTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblSystemValue")
                .WithColumn("SystemValueId").AsInt32().PrimaryKey().Identity()
                .WithColumn("ValueKey").AsString(80)
                .WithColumn("Value").AsString(int.MaxValue)
                .WithColumn("ModifiedByNaati").AsBoolean()
                .WithColumn("ModifiedDate").AsDateTime()
                .WithColumn("ModifiedUser").AsInt32();

            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "BuildVersion", Value = "", 
                ModifiedByNaati = 0, 
                ModifiedDate = DateTime.Now,
                ModifiedUser = 40
            });
            
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "DatabaseUpdaterLastRun", Value = "", 
                ModifiedByNaati = 0, 
                ModifiedDate = DateTime.Now,
                ModifiedUser = 40
            });

            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "DisableInfoLog",
                Value = "0",
                ModifiedByNaati = 0,
                ModifiedDate = DateTime.Now,
                ModifiedUser = 40
            });
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "DisableReportingExecution",
                Value = "0",
                ModifiedByNaati = 0,
                ModifiedDate = DateTime.Now,
                ModifiedUser = 40
            });
            Insert.IntoTable("tblSystemValue").Row(new
            {
                ValueKey = "LogHistoryDays",
                Value = "180",
                ModifiedByNaati = 0,
                ModifiedDate = DateTime.Now,
                ModifiedUser = 40
            });
        }
    }
}
