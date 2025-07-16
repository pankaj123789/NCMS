namespace F1Solutions.Naati.Common.Migrations.NAATI._20171016_AddColumnsForTestLocation
{
   [NaatiMigration(201710161302)]
    public class AddColumnsForTestLocation : NaatiMigration
    {
        public override void Up()
        {
            Create.Table("tblTestLocation")
                .WithColumn("TestLocationId").AsInt32().Identity().PrimaryKey()
                .WithColumn("OfficeId").AsInt32().NotNullable()
                .WithColumn("CountryId").AsInt32().Nullable()
                .WithColumn("Name").AsString(500).Nullable();
                

            Create.ForeignKey("FK_TestLocation_Office")
                .FromTable("tblTestLocation")
                .ForeignColumn("OfficeId")
                .ToTable("tblOffice")
                .PrimaryColumn("OfficeId");
            Create.ForeignKey("FK_TestLocation_Country")
                .FromTable("tblTestLocation")
                .ForeignColumn("CountryId")
                .ToTable("tblCountry")
                .PrimaryColumn("CountryId");


            Create.Column("PreferredTestLocationId").OnTable("tblCredentialApplication").AsInt32().Nullable();

            var query =
               @"declare @countryId int = (select countryID from tblCountry where name = 'Australia')
                 IF (select count(1) from tblTestLocation) <= 0 BEGIN 
                    insert into tblTestLocation (OfficeId,CountryId, Name)
                    values(3, @countryId, 'Sydney')
                    ,(1,@countryId,'Canberra')
                    ,(9,@countryId,'Melbourne')
                    ,(6,@countryId,'Brisbane')
                    ,(8,@countryId,'Hobart')
                    ,(7,@countryId,'Adelaide')
                    ,(10,@countryId,'Perth')
                    ,(4,@countryId,'Darwin')
                END";
            Execute.Sql(query);
        }
    }
}
