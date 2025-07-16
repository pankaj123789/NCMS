using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20190130_AddRolePlayerFields
{
    [NaatiMigration(201901311706)]
    public class AddRolePlayerFields : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("RolePlayerRating")
                .OnTable("PersonHistory")
                .AsDecimal(3, 1).Nullable();

            Create.Column("RolePlayerSenior")
                .OnTable("PersonHistory")
                .AsBoolean().Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
