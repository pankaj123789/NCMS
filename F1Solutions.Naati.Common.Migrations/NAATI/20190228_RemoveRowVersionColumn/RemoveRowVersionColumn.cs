using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20190228_RemoveRowVersionColumn
{
    [NaatiMigration(201902281235)]
    public class RemoveRowVersionColumn: NaatiMigration
    {
        public override void Up()
        {
            var sql = @"
            ALTER TABLE [dbo].[tblAddress]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblAuditLog]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblCountry]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblEmail]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblEmailBatch]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblEmailBatchRecipient]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblEmailTemplate]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblEntity]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblGLCode]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblInstitution]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblInstitutionName]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblLanguage]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblOffice]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPanel]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPanelMembership]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPerson]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPersonImage]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPersonName]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPhone]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblPostcode]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblProductSpecification]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblRoleScreen]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblSuburb]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblSystemValue]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblTestComponentType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblUser]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tblUserRole]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluAuditType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluEFTMachine]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluJobType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluPanelType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluPermission]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluProductCategory]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluProductType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluRegion]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluResultType]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluRole]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluState]
                    DROP COLUMN[RowVersion]

            ALTER TABLE[dbo].[tluTitle]
                    DROP COLUMN[RowVersion]";

            Execute.Sql(sql);
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
