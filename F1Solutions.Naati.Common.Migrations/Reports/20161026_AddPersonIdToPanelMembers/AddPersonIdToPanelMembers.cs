using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161026_AddPersonIdToPanelMembers
{
    [NaatiMigration(201610261805)]
    public class AddPersonIdToPanelMembers : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("PersonId").OnTable("PanelMembersHistory").AsInt32().NotNullable();

            this.ExecuteSql(@"
                            UPDATE [PanelMembersHistory]
                            SET [PersonId] = ph.[PersonId]
                            FROM [PersonHistory] ph
                            LEFT JOIN  [PanelMembersHistory] pm ON ph.[NAATINumber] = pm.[NAATINumber]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
