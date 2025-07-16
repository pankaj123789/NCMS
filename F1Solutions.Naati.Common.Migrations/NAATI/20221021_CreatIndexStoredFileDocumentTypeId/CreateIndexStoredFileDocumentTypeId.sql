IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_tblStoredFile_DocumentTypeId' AND object_id = OBJECT_ID('tblStoredFile '))
    BEGIN
        CREATE Nonclustered INDEX IX_tblStoredFile_DocumentTypeId ON tblStoredFile (DocumentTypeId)
    END