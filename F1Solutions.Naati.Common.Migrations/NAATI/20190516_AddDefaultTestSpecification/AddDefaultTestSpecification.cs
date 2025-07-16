
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190516_AddDefaultTestSpecification
{
    [NaatiMigration(201905161006)]
    public class AddDefaultTestSpecification : NaatiMigration
    {
        public override void Up()
        {
            Create.Column("DefaultTestSpecificationId").OnTable("tblTestSession").AsInt32().Nullable();

             Execute.Sql(@"UPDATE tss set tss.DefaultTestSpecificationId =
                             (Select Top 1 ts.testSpecificationId from tblTestSitting ts   where ts.TestSessionId = tss.TestSessionId and ts.Rejected = 0 and ts.Supplementary = 0 )
                             from tblTestSession tss
                             where
                            (Select count(*) from tblTestSitting ts2 where ts2.TestSessionId = tss.TestSessionId and ts2.Rejected = 0 and ts2.Supplementary = 0) > 0");

            Execute.Sql(@"UPDATE tss set tss.DefaultTestSpecificationId =
                        (Select Top 1 ts.testSpecificationId from tblTestSpecification ts   where ts.CredentialTypeId = tss.CredentialTypeId and ts.Active = 1 )
                        from tblTestSession tss
                        where
                        (Select count(*) from tblTestSitting ts2 where ts2.TestSessionId = tss.TestSessionId and ts2.Rejected = 0 and ts2.Supplementary = 0) = 0");


            Alter.Column("DefaultTestSpecificationId").OnTable("tblTestSession").AsInt32().NotNullable();

            Create.ForeignKey("FK_TestSession_TestSpecification")
                .FromTable("tblTestSession")
                .ForeignColumn("DefaultTestSpecificationId")
                .ToTable("tblTestSpecification")
                .PrimaryColumn("TestSpecificationId");

        }
    }
}
