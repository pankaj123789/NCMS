using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20171013_AddOrganisationTable
{
    [NaatiMigration(201710131700)]
    public class AddOrganisationTable : NaatiMigration
    {
        public override void Up()
        {
            // Organisation Contacts
            Create.Table("OrganisationContactsHistory")
                .WithColumn("ContactPersonId").AsInt32().NotNullable().PrimaryKey()
                .WithColumn("OrganisationId").AsInt32()
                .WithColumn("Name").AsString(50000).Nullable()
                .WithColumn("Email").AsString(500).Nullable()
                .WithColumn("Phone").AsString(500).Nullable()
                .WithColumn("Address").AsString(int.MaxValue).Nullable()
                .WithColumn("Description").AsString(int.MaxValue).Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            // Organisation
            Create.Table("OrganisationHistory")
                .WithColumn("OrganisationId").AsInt32().PrimaryKey()
                .WithColumn("NAATINumber").AsInt32().NotNullable()
                .WithColumn("Name").AsString(100).Nullable()
                .WithColumn("PrimaryAddress").AsString(500).Nullable()
                .WithColumn("Suburb").AsString(50).Nullable()
                .WithColumn("Country").AsString(50).Nullable()
                .WithColumn("PrimaryPhone").AsString(60).Nullable()
                .WithColumn("PrimaryEmail").AsString(200).Nullable()
                .WithColumn("TrustedPayer").AsBoolean().Nullable()
                .WithColumn("ModifiedDate").AsDateTime().NotNullable().PrimaryKey()
                .WithColumn("ObsoletedDate").AsDateTime().Nullable()
                .WithColumn("DeletedDate").AsDateTime().Nullable();

            // Add the stored procedures
            Execute.Sql(Sql.AddOrganisationContactsProcedure);
            Execute.Sql(Sql.AddOrganisationProcedure);

            // Add the views
            Execute.Sql(Sql.AddOrganisationContactsView);
            Execute.Sql(Sql.AddOrganisationView);

            // Applications
            Create.Column("SponsoredOrganisationId").OnTable("ApplicationHistory").AsInt32().Nullable();
            Create.Column("SponsoredOrganisationName").OnTable("ApplicationHistory").AsString(100).Nullable();
            Create.Column("SponsoredContactId").OnTable("ApplicationHistory").AsInt32().Nullable();
            Create.Column("SponsoredContactName").OnTable("ApplicationHistory").AsString(201).Nullable();
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
