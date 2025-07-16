ALTER Procedure LogExecutionInfo(@EntityName varchar(200), @Message varchar(Max))
AS
BEGIN

	Declare @DisableInfoLog varchar(max)
	set @DisableInfoLog = dbo.GetSystemValue('DisableInfoLog')

	IF @DisableInfoLog <> '1' 
	BEGIN 
		Insert into tblJobExecutionLog(LogDate,EntityName,Message,JobExecutionLogTypeId) values(GETDATE(),@EntityName,@Message,1)		
	END
	
END