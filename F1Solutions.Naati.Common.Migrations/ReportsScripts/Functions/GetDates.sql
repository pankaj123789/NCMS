ALTER FUNCTION [dbo].[GetDates]
(
	@startDate datetime,
	@endDate datetime
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT TOP (DATEDIFF(DAY, @startDate, @endDate) + 1)
        Date = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @startDate)
	FROM    sys.all_objects a
			CROSS JOIN sys.all_objects b
)
