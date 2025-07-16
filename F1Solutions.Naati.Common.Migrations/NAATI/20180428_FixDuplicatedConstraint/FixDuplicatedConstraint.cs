namespace F1Solutions.Naati.Common.Migrations.NAATI._20180428_FixDuplicatedConstraint
{
    [NaatiMigration(201804281600)]
    public class FixDuplicatedConstraint: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"if exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
                        WHERE CONSTRAINT_NAME = 'FK_TestComponent_TestSpecification1')
                        ALTER TABLE tblTestComponent 
                        DROP CONSTRAINT FK_TestComponent_TestSpecification1;");
            

            Execute.Sql(@"if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                                 WHERE CONSTRAINT_NAME = 'FK_TestComponent_TestSpecification')
            ALTER TABLE tblTestComponent
            ADD CONSTRAINT FK_TestComponent_TestSpecification
            FOREIGN KEY (TestSpecificationId)REFERENCES TestSpecification(TestSpecificationId);");
        }
    }
}
