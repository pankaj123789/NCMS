ALTER Procedure LogExecutionError(@EntityName varchar(200), @Message varchar(Max))
AS
BEGIN
	
	Insert into tblJobExecutionLog(LogDate,EntityName,Message,JobExecutionLogTypeId) values(GETDATE(),@EntityName,@Message,3)	
	PRINT CONVERT(NVARCHAR(MAX), GETDATE(), 121) + ' ' + 'Error in Entity: ' + @EntityName + '. Message: ' + @Message
END