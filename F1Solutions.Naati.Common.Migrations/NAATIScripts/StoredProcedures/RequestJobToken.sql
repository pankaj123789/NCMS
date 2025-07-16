ALTER PROCEDURE [dbo].[RequestJobToken]
(@JobName varchar(100),
 @Success bit OUTPUT)
AS
	DECLARE @MaximunRunningTime int = 61;
	DECLARE @table table ([Value] nvarchar(max))
	UPDATE tblSystemValue SET [Value] =  CONVERT(nvarchar(max), GETDATE(), 126)
	OUTPUT inserted.[Value] 
	INTO @table
	--If the time is greater than running time then token is assigned.
	WHERE ValueKey = @JobName AND  ([Value] = '0' OR (TRY_CONVERT(datetime, [VALUE])  IS NOT NULL AND  DATEADD(MINUTE, -@MaximunRunningTime, GETDATE())> TRY_CONVERT(datetime, [VALUE]) ))

	select @Success = CAST(COUNT(*) AS BIT) from @table
