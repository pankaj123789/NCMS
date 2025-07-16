
namespace F1Solutions.Naati.Common.Migrations.NAATI._20180910_ChangeLengthOfAttachment
{
    [NaatiMigration(201809101400)]
    public class ChangeLengthOfAttachment : NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"ALTER TABLE tblCredentialApplicationAttachment
                         ALTER   COLUMN Description nvarchar(255)

                         ALTER TABLE tblWorkPracticeAttachment
                         ALTER   COLUMN Description nvarchar(255)

                         ALTER TABLE tblProfessionalDevelopmentActivityAttachment
                         ALTER COLUMN Description nvarchar(255)");
        }
    }
}
