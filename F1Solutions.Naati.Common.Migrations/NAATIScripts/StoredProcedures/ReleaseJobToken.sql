ALTER PROCEDURE [dbo].[ReleaseJobToken]
(@JobName varchar(100),
 @Success bit OUTPUT)
AS	
	DECLARE @table table ([Value] nvarchar(max))
	UPDATE tblSystemValue SET [Value] = '0'
	OUTPUT inserted.[Value] 
	INTO @table
	WHERE ValueKey = @JobName AND TRY_CONVERT(datetime, [VALUE])  IS NOT NULL

	select @Success = CAST(COUNT(*) AS BIT) from @table