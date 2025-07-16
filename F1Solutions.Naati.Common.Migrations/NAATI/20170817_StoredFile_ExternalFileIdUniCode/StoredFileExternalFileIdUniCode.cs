using System;

namespace F1Solutions.Naati.Common.Migrations.NAATI._20170817_StoredFile_ExternalFileIdUniCode
{
    [NaatiMigration(201708171000)]
    public class StoredFileExternalFileIdUniCode : NaatiMigration
    {
        public override void Up()
        {
            // change the tblStoredFile.ExternalFileId column to nvarchar to support non-ANSI characters in filenames
            Execute.Sql(@"
                ALTER TABLE tblStoredFile
                ALTER COLUMN ExternalFileId NVARCHAR(MAX) NOT NULL");
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}
