ALTER Procedure LogExecutionWarning(@EntityName varchar(200), @Message varchar(Max))
AS
BEGIN
	
	Insert into tblJobExecutionLog(LogDate,EntityName,Message,JobExecutionLogTypeId) values(GETDATE(),@EntityName,@Message,2)	
	PRINT CONVERT(NVARCHAR(MAX), GETDATE(), 121) + ' ' + 'Warning on Entity: ' + @EntityName + '. Message: ' + @Message
END