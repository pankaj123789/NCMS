using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20161021_AddPanelTable
{
    [NaatiMigration(201610211444)]
    public class AddPanelTable : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("PanelHistory")
                .WithColumn("PanelId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable()
                .WithColumn("PanelName").AsString(100).Nullable()
                .WithColumn("PanelType").AsString(50).Nullable()
                .WithColumn("Language").AsString(50).Nullable()
                .WithColumn("CommissionedDate").AsDateTime().Nullable();

            this.ExecuteSql(@"
CREATE NONCLUSTERED INDEX [IX_Panel_ObsoletedDate] ON [dbo].[PanelHistory]
(
    [ObsoletedDate] ASC
)
INCLUDE ( 	[PanelId],
    [PanelName],
    [PanelType],
    [Language],
    [CommissionedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
