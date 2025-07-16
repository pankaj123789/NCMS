Declare @dependencies Table(Id Int NOT NULL IDENTITY(1,1) PRIMARY KEY,
						tableName varchar(50) not null, 						
						dependantTableName varchar(50) not null )

declare @tablesToClear Table(Id Int NOT NULL IDENTITY(1,1) PRIMARY KEY,
						tableName varchar(50) not null, 						
						eraseLevel int null,
						needsToBeEvaluated bit null )

declare @dependantTables Table(Id Int NOT NULL IDENTITY(1,1) PRIMARY KEY,
						tableName varchar(50) not null)
Declare @totalTables int;
Declare @tableName varchar(50);
Declare @tableObjectId int;
Declare @totalDependantTables int=0;
Declare @dependantTableName varchar(50);
Declare @excuteWithConflicts bit = 0; --Flag to enable or disable erasing when conflicts are found

-- Add the tables need to be erased
Insert Into @tablesToClear (tableName) 
Values
('Results'),
('tblAccreditationProduct'),
('tblAccreditationResult'),
('tblAddress'),
('tblApplication'),
('tblApplicationRevalidation'),
('tblApprovalCondition'),
('tblAuditLog'),
('tblBankDeposit'),
('tblChangeRequest'),
('tblChangeRequestCourseApproval'),
('tblCorrespondence'),
('tblCourse'),
('tblCourseApproval'),
('tblCourseAttendance'),
('tblCourseCoordinator'),
('tblCourseLocation'),
('tblCredentialRecordLog'),
('tblDashboardReport'),
('tblEmail'),
('tblEmailBatch'),
('tblEmailBatchAttachment'),
('tblEmailBatchLetterTemplate'),
('tblEmailBatchRecipient'),
('tblEmailTemplate'),
('tblEntityCorrespondence'),
('tblEoiLanguage'),
('tblEvent'),
('tblEventDate'),
('tblEventFacilitator'),
('tblEventLog'),
('tblEventVenue'),
('tblExaminerMarking'),
('tblExaminerTestComponentResult'),
('tblExaminerUnavailable'),
('tblExpressionOfInterest'),
('tblExternalAccountingOperation'),
('tblInspection'),
('tblInstitutionContactPerson'),
('tblInstitutionMergeLog'),
('tblInvoice'),
('tblInvoiceBatch'),
('tblInvoiceBatchInvoices'),
('tblInvoiceLine'),
('tblJob'),
('tblJobCorrespondence'),
('tblJobExaminer'),
('tblJobExaminerPayroll'),
('tblLanguageExperience'),
('tblLetterBatch'),
('tblLetterBatchExtraFields'),
('tblMailingList'),
('tblMailingListAddress'),
('tblMaterial'),
('tblMergeData'),
('tblMergeData_OLD'),
('tblMergeDataAccreditationProduct'),
('tblMergeDataApplication'),
('tblMergeDataInvoice'),
('tblMergeDataJob'),
('tblMergeDataTestAttendance'),
('tblNonStandardData'),
('tblNote'),
('tblOrder'),
('tblPanelMembership'),
('tblPanelNote'),
('tblPayment'),
('tblPaymentAllocation'),
('tblPaymentCash'),
('tblPaymentCC'),
('tblPaymentCheque'),
('tblPaymentDirectDeposit'),
('tblPaymentOther'),
('tblPayroll'),
('tblPDInclusion'),
('tblPerson'),
('tblPersonArchiveHistory'),
('tblPersonExpertise'),
('tblPersonImage'),
('tblPersonMergeLog'),
('tblPersonName'),
('tblPersonToInstitutionMergeLog'),
('tblPhone'),
('tblProductSpecification'),
('tblProject'),
('tblReminder'),
('tblRevalidation'),
('tblSidebar'),
('tblSmsReceived'),
('tblSmsReminderType'),
('tblSmsSend'),
('tblSmsTemplate'),
('tblStoredFile'),
('tblStoredSearches'),
('tblTestAttendance'),
('tblTestAttendanceDocument'),
('tblTestAvailability'),
('tblTestAvailabilityRollover'),
('tblTestComponentResult'),
('tblTestEvent'),
('tblTestInvitation'),
('tblTestNote'),
('tblTestResult'),
('tblTestResultAttachment'),
('tblTransaction'),
('tblWorkshopAttendance'),
('tblWorkshopAttendanceLanguage'),
('tblWorkshopEvent'),
('tblWorkshopMaterial'),
('tluAccreditationCategory'),
('tluAccreditationDescription'),
('tluAccreditationLevel'),
('tluAccreditationMethod'),
('tluAccreditationType'),
('tluApplicationProductMatchup'),
('tluContactType'),
('tluDepositType'),
('tluMailCategory'),
('tluPaymentOtherType'),
('tluSmsStatus'),
('tmpApplicationStatus'),
('vwAccreditationTypeFixed_2'),
('tblAddressMailCategory'),
('tblGadget'),
('tblTestResultFailureReason')

Select @totalTables  = count(*) from @tablesToClear
--Print(CONCAT('Total tables to process: ',@totalTables))
Declare @i int; 
Declare @j int;
Declare @tableIsInList bit;
Declare @currentTableEraseLevel int;
Declare @tableLevel int;
Declare @tablesPendingToEvaluate int;
Declare @evaluateTable bit;

SELECT @tablesPendingToEvaluate = COUNT(*) FROM @tablesToClear where needsToBeEvaluated = 1 Or needsToBeEvaluated is NULL;
SET @j =1;
WHILE @tablesPendingToEvaluate > 0
BEGIN
	SET @i =1;

	WHILE  @i <=  @totalTables
	BEGIN
		--Checks whether the tables has to be evaluated.
		SELECT @evaluateTable = needsToBeEvaluated from @tablesToClear where Id = @i 
		IF @evaluateTable = 0 
		BEGIN
			set @i = @i +1;
			CONTINUE
		END
	
		UPDATE @tablesToClear SET needsToBeEvaluated = 0 WHERE Id = @i 	

		Select @tableName= tableName from @tablesToClear where Id = @i 		
		Select @tableObjectId = object_id from sys.tables where name = @tableName 	
	
		--Looks for the dependencies
		DELETE FROM @dependantTables	
		Insert Into @dependantTables
		Select pt.name AS dependantTableNames FROM SYS.foreign_keys fk
		inner join sys.tables t on fk.referenced_object_id = t.object_id
		inner join sys.tables pt on pt.object_id = fk.parent_object_id
		where fk.referenced_object_id = @tableObjectId;		
	
		Select @totalDependantTables = (@totalDependantTables + count(*)) from @dependantTables
	
		WHILE(@j <= @totalDependantTables)
		BEGIN		
		
			Select @dependantTableName = tableName from @dependantTables where Id = @j	

			--Checks if the dependency is in the list to clear too.
			Select @tableIsInList = Case When (Select count(*) from @tablesToClear where tableName = @dependantTableName) > 0 THEN 1 ELSE 0 End
		
			IF @tableIsInList =1 
			BEGIN	
				
				--Checks whether table has an assigned level
				Select @currentTableEraseLevel= eraseLevel from @tablesToClear where Id = @i
				IF @currentTableEraseLevel IS NULL
				BEGIN		
					SET @currentTableEraseLevel = 1;
					UPDATE @tablesToClear set eraseLevel = @currentTableEraseLevel WHERE Id = @i
				END			
			
				Select @tableLevel = eraseLevel from @tablesToClear where tableName = @dependantTableName
				IF @tableLevel =-1
				BEGIN
					--Skips table because it has a dependency and the dependency cant be erased		
					Update @tablesToClear set eraseLevel = -1 where Id = @i
					Insert into @dependencies values(@tableName,@dependantTableName)
					print(Concat('Table: ', @tableName,' cant be erased because table:', @dependantTableName,' depends on it and can not be erased'))					
				END
				ELSE
				BEGIN
					--Checks if the level of  the dependency is grater, If not then level is updated and set flag needsToBeEvaluated =1 in order to be reevaluated
					IF @tableLevel <= @currentTableEraseLevel OR @tableLevel IS NULL
					BEGIN
						UPDATE @tablesToClear SET eraseLevel = (@currentTableEraseLevel +1), needsToBeEvaluated = 1 WHERE tableName = @dependantTableName
					END		
				END							

			END
			ELSE
			BEGIN	
				--Skips table because it has a dependency and the dependency cant be erased		
				Update @tablesToClear set eraseLevel = -1 where Id = @i
				Insert into @dependencies values(@tableName,@dependantTableName)
				print(Concat('Table:', @tableName,' cant be erased because table:', @dependantTableName,' depends on it.'))		
			END
			set @j = @j+1;
		END

		set @i = @i +1;
	END

	SELECT @tablesPendingToEvaluate = COUNT(*) FROM @tablesToClear where needsToBeEvaluated = 1 Or needsToBeEvaluated is NULL
END

Declare @totalConflicts int;

--counts conflicts
Select @totalConflicts = count(*) from @dependencies;

IF @totalConflicts = 0 OR @excuteWithConflicts = 1
BEGIN
	--Delete from list tables that cant be erased
	DELETE FROM @tablesToClear WHERE eraseLevel=-1


	declare @orderedTablesToClear Table(Id Int NOT NULL IDENTITY(1,1) PRIMARY KEY,
							tableName varchar(50) not null,
							eraseLevel int null )

	--Create a table with the tables to be erased, ordered by level, Tables with higher order  must be erased first
	Insert into  @orderedTablesToClear
	Select tableName, eraseLevel from @tablesToClear order by eraselevel DESC	

	--Get total of tables to erase
	SELECT @totalTables =  COUNT(*) FROM @orderedTablesToClear

	set @i = 1
	declare @command  nvarchar(1000)
	WHILE  @i <=  @totalTables
	BEGIN
		 --Erase the tables
	
		SELECT @command= 'DELETE FROM ' + tableName  from @orderedTablesToClear where Id = @i
		--print(' Executing command: ' + @Command);
		execute (@Command);
		--print('Delete command executed for table.' )
		SET @i = @i +1;
	END
END
ELSE
BEGIN	 
	Declare @newLine AS CHAR(2) = CHAR(13) + CHAR(10)
	declare @errorMessage  nvarchar(1000)
	SET @errorMessage = 'TABLES WERE NOT DELETED BECAUSE FOLLING CONFLICTS WHERE FOUND:' + @newLine
	set @i = 1
	WHILE  @i <=  @totalConflicts
	BEGIN
		 --Erase the tables		
		SELECT @errorMessage=  @errorMessage +'TableName: ' + tableName + 'DependantTableName: '+ dependantTableName + @newLine  from @dependencies where Id = @i		
		SET @i = @i +1;			
	END;
	
	THROW  51000, @errorMessage, 1;	
END

--NOTE: THIS QUERY IS ERASING SEVERAL TABLES, THEREFORE IT IS NORMAL IF IT TAKE SOME MINUTES TO FINISH
