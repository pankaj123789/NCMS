using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161021_AddPanelMembersTable
{
    [NaatiMigration(201610211502)]
    public class AddPanelMembersTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("PanelMembersHistory")
                .WithColumn("PanelMembersId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("PanelId").AsInt32().NotNullable()
                .WithColumn("PersonName").AsString(252).Nullable()
                .WithColumn("NAATINumber").AsString(50).NotNullable()
                .WithColumn("Role").AsString(50).Nullable()
                .WithColumn("StartDate").AsDateTime().NotNullable()
                .WithColumn("EndDate").AsDateTime().Nullable();

            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_PanelMembers_ObsoletedDate] ON [dbo].[PanelMembersHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[PanelId],
    [PersonName],
    [NAATINumber],
    [Role],
    [StartDate],
    [EndDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
