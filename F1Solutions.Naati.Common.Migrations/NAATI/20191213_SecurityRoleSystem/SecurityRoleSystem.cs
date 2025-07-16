using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20191213_SecurityRoleSystem
{
    [NaatiMigration(201912131450)]
    public class SecurityRoleSystem : NaatiMigration
    {
        public override void Up()
        {
            Alter.Table("tblSecurityRole")
                .AddColumn("System").AsBoolean().Nullable();

            Update.Table("tblSecurityRole").Set(new { System = false }).AllRows();

            Alter.Table("tblSecurityRole")
                .AlterColumn("System").AsBoolean().NotNullable();
        }
    }
}
