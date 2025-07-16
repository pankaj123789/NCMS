ALTER PROCEDURE ReportingSnapshot_Test_Latest
AS
BEGIN
		IF OBJECT_ID(N'dbo.TestLatest') IS NOT NULL DROP TABLE TestLatest		
        SELECT * INTO TestLatest FROM TestHistory WHERE RowStatus = 'Latest'
END