ALTER FUNCTION [dbo].[GetSystemValue](@ValueKey varchar(80))
RETURNS varchar(max)
AS 
BEGIN
    Declare @Value varchar(max)
	select @Value = [Value] from tblSystemValue where ValueKey = @ValueKey
    RETURN  @Value
END;