ALTER FUNCTION [dbo].[GetPhoneNumber]
(
	@entityId int
	,@PrimaryContact bit
)
RETURNS TABLE
AS
RETURN
(
	SELECT TOP 1
		[Number]
		,[Invalid]
	FROM [naati_db]..[tblPhone] phone
	WHERE phone.[EntityId] = @entityId AND phone.[PrimaryContact] = @PrimaryContact AND phone.[Invalid] = 0
)
