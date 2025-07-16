ALTER PROCEDURE [dbo].[DeletedApplications]
@Days int
AS

Declare @DeletedTypeId int, @DraftTypeId int
	
	BEGIN TRANSACTION
	
		IF OBJECT_ID('tempdb..#TempCredentialApplicationId') IS NOT NULL DROP TABLE #TempCredentialApplicationId
		IF OBJECT_ID('tempdb..#TempEmailMessageId') IS NOT NULL DROP TABLE #TempEmailMessageId

		set @DeletedTypeId = (select CredentialApplicationStatusTypeId from tblCredentialApplicationStatusType where [Name] = 'Deleted')
		set @DraftTypeId = (select CredentialApplicationStatusTypeId from tblCredentialApplicationStatusType where [Name] = 'Draft')

		
		select CredentialApplicationId  
			into #TempCredentialApplicationId 
		from tblCredentialApplication WHERE (CredentialApplicationStatusTypeId = @DeletedTypeId) 
			Or
			(CredentialApplicationStatusTypeId = @DraftTypeId AND ABS( DATEDIFF( day, StatusChangeDate, GETDATE() ) ) >= @Days)

		SELECT EmailMessageId 
		INTO #TempEmailMessageId
		FROM tblCredentialApplicationEmailMessage
		WHERE CredentialApplicationId IN (SELECT CredentialApplicationId FROM #TempCredentialApplicationId)
					
		delete from tblCredentialApplicationAttachment where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)
		delete from tblCredentialApplicationFieldData where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)
		delete from tblCredentialApplicationNote where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)

		delete from tblCredentialCredentialRequest where CredentialRequestId in (select CredentialRequestId from tblCredentialRequest where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId))
		delete from tblCredentialRequestFieldData where CredentialRequestId in (select CredentialRequestId from tblCredentialRequest where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId))
		delete from tblCredentialRequest where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)

		delete from tblEmailMessageAttachment where EmailMessageId in (SELECT EmailMessageId FROM #TempEmailMessageId)
		
		DELETE from tblCredentialApplicationEmailMessage where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)
		DELETE from tblEmailMessage where EmailMessageId in (select EmailMessageId from #TempEmailMessageId)

		delete from tblCredentialApplication where CredentialApplicationId in (select CredentialApplicationId from #TempCredentialApplicationId)
				
		drop table #TempCredentialApplicationId
		drop table #TempEmailMessageId

	COMMIT TRAN -- Transaction Success!
	

