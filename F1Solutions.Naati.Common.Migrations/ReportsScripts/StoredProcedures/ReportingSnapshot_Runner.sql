ALTER PROCEDURE [dbo].[ReportingSnapshot_Runner]	
 @Date datetime, @StoreProcedureName varchar(200)
AS
BEGIN
	
	BEGIN TRY
		
		Declare @DisableReportingExecution varchar(max)
		set @DisableReportingExecution = dbo.GetSystemValue('DisableReportingExecution')

		IF @DisableReportingExecution <> '1' 
		BEGIN
			EXEC LogExecutionInfo @StoreProcedureName,'Running...'
			EXEC @StoreProcedureName @Date;	
			EXEC LogExecutionInfo @StoreProcedureName,'Finished.'
		END
		ELSE
		BEGIN
			EXEC LogExecutionWarning @StoreProcedureName,'Report Execution is disabled.'
		END		

	END TRY
	BEGIN CATCH   
		
		DECLARE @ErrorMessage NVARCHAR(max);
	    DECLARE @ActiveTransactionState int;
		set @ActiveTransactionState = XACT_STATE();

		if @ActiveTransactionState<>0
		BEGIN
			ROLLBACK TRAN
	    END

		SET @ErrorMessage = ERROR_MESSAGE() + ', Severity: ' + CAST(ERROR_SEVERITY() as varchar(200)) + ', Error State: ' +  CAST(ERROR_STATE() as varchar(200));

		EXEC LogExecutionError @StoreProcedureName, @ErrorMessage
		
	END CATCH

END