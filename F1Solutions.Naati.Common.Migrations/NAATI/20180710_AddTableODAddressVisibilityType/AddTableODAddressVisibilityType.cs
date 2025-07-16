
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180710_AddTableODAddressVisibilityType
{
    [NaatiMigration(201807101900)]
    public class AddTableODAddressVisibilityType : NaatiMigration
    {
        public override void Up()
        {
            CreateTableODAddressVisibilityTypeTable();
            CreateColumnfortblAddress();
        }

        void CreateColumnfortblAddress()
        {
            Create.Column("ODAddressVisibilityTypeId").OnTable("tblAddress").AsInt32().Nullable();
            Update.Table("tblAddress").Set(new { ODAddressVisibilityTypeId = 1 }).AllRows();
            Alter.Column("ODAddressVisibilityTypeId").OnTable("tblAddress").AsInt32().NotNullable();

            Create.ForeignKey("FK_Address_ODAddressVisibilityType")
                    .FromTable("tblAddress")
                    .ForeignColumn("ODAddressVisibilityTypeId")
                    .ToTable("tblODAddressVisibilityType")
                    .PrimaryColumn("ODAddressVisibilityTypeId");
        }       

        void CreateTableODAddressVisibilityTypeTable()
        {
            Create.Table("tblODAddressVisibilityType")
                .WithColumn("ODAddressVisibilityTypeId").AsInt32().Identity().PrimaryKey()
                .WithColumn("Name").AsString(50)
                .WithColumn("DisplayName").AsString(50);

            //insert data            
            Execute.Sql("INSERT INTO  tblODAddressVisibilityType(Name,DisplayName) VALUES('DoNotShow','Do Not Show')");
            Execute.Sql("INSERT INTO  tblODAddressVisibilityType(Name,DisplayName) VALUES('StateOnly','State Only')");
            Execute.Sql("INSERT INTO  tblODAddressVisibilityType(Name,DisplayName) VALUES('StateSuburb','State Suburb')");
            Execute.Sql("INSERT INTO  tblODAddressVisibilityType(Name,DisplayName) VALUES('FullAddress','Full Address')");

        }
    }
}
