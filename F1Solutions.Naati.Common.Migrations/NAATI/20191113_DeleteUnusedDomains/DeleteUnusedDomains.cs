namespace F1Solutions.Naati.Common.Migrations.NAATI._20191113_DeleteUnusedDomains
{
    [NaatiMigration(201911130955)]
    public class DeleteUnusedDomains : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"UPDATE tblTestMaterial
                          SET TestMaterialDomainId = 54
                          WHERE TestMaterialDomainId IN
                          (55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68)
                        ");

            Execute.Sql(@"DELETE FROM tblCredentialTypeTestMaterialDomain
                          WHERE CredentialTypeTestMaterialDomainId IN
                          (162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175)
                        ");

            Execute.Sql(@"DELETE FROM tblTestMaterialDomain
                          WHERE TestMaterialDomainId IN
                          (55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68)
                        ");
        }
    }
}