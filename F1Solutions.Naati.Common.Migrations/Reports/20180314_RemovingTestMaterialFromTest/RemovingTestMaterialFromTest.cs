using System;
using F1Solutions.Naati.Common.Migrations.NAATI;

namespace F1Solutions.Naati.Common.Migrations.Reports._20180314_RemovingTestMaterialFromTest
{
    [NaatiMigration(201803141634)]
    public class RemovingTestMaterialFromTest : NaatiMigration
    {
        public override void Up()
        {
            Delete.Column("TestMaterialId").FromTable("TestHistory");
            Delete.Column("TestMaterialDescription").FromTable("TestHistory");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
