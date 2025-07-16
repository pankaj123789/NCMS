
IF NOT EXISTS (
    SELECT *
    FROM sys.indexes 
	WHERE [NAME] = 'EmailDateIndex'
	)
CREATE NONCLUSTERED INDEX [EmailDateIndex]
ON [tblEmailMessage] ([CreatedDate])
INCLUDE ([RecipientEntityId],[RecipientEmail],[Subject],[LastSendResult],[LastSendAttemptDate],[CreatedUserId],[EmailSendStatusTypeId])

GO