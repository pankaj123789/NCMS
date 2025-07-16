ALTER PROCEDURE [dbo].[ReportingSnapshop_ExaminerJob]
	@Date DateTime
AS
BEGIN

		DECLARE @ExaminerJobHistory as table( [JobExaminerId] [int] NOT NULL,[TestSittingId] [int] NOT NULL,[JobId] [int] NULL,[ApplicationTypeId] [int] NULL,[ApplicationType] [nvarchar](100) NULL,[ExaminerCustomerNumber] [int] NULL,[CredentialTypeId] [int] NULL,[CredentialType] [nvarchar](100) NULL,
										  [SkillId] [int] NULL,[Skill] [nvarchar](300) NULL,[DateAllocated] [datetime] NULL,[ReceivedDate] [datetime] NULL,[DueDate] [datetime] NULL,[ExaminerCost] [decimal](19, 5) NULL,[ProductSpecificationId] [int] NULL,[ProductSpecificationCode] [nvarchar](100) NOT NULL,
										  [ProductSpecificationDescription] [nvarchar](500) NOT NULL,[GLCodeId] [int] NULL,[GLCode] [nvarchar](100) NULL,[ExaminerPayRollStatusId] [int] NULL,[ExaminerPayRollStatus] [nvarchar](100) NULL,[PayRollModifiedDate] [datetime] NULL,[PayRollModifiedUser] [nvarchar](100) NULL,
										  [PayRollAccountingReference] [nvarchar](100) NULL,
										  [ModifiedDate] [datetime] NOT NULL,[RowStatus] nvarchar(50))   	
		
	IF(@Date is NULL)
	set @Date = GETDATE()	
	SELECT @Date = DATEADD(dd, DATEDIFF(dd, 0, @Date), 0) 

	INSERT into @ExaminerJobHistory

	SELECT
		CASE WHEN [Source].[JobExaminerId] IS NULL THEN [ExaminerJob].[JobExaminerId] ELSE [Source].[JobExaminerId] END AS [JobExaminerId]
		,CASE WHEN [Source].[TestSittingId] IS NULL THEN [ExaminerJob].[TestSittingId] ELSE [Source].[TestSittingId] END AS [TestSittingId]
		,CASE WHEN [Source].[JobId] IS NULL THEN [ExaminerJob].[JobId] ELSE [Source].[JobId] END AS [JobId]
		,CASE WHEN [Source].[ApplicationTypeId] IS NULL THEN [ExaminerJob].[ApplicationTypeId] ELSE [Source].[ApplicationTypeId] END AS [ApplicationTypeId]
		,CASE WHEN [Source].[ApplicationType] IS NULL THEN [ExaminerJob].[ApplicationType] ELSE [Source].[ApplicationType] END AS [ApplicationType]
		,CASE WHEN [Source].[ExaminerCustomerNumber] IS NULL THEN [ExaminerJob].[ExaminerCustomerNumber] ELSE [Source].[ExaminerCustomerNumber] END AS [ExaminerCustomerNumber]
		,CASE WHEN [Source].[CredentialTypeId] IS NULL THEN [ExaminerJob].[CredentialTypeId] ELSE [Source].[CredentialTypeId] END AS [CredentialTypeId]
		,CASE WHEN [Source].[CredentialType] IS NULL THEN [ExaminerJob].[CredentialType] ELSE [Source].[CredentialType] END AS [CredentialType]
		,CASE WHEN [Source].[SkillId] IS NULL THEN [ExaminerJob].[SkillId] ELSE [Source].[SkillId] END AS [SkillId]
		,CASE WHEN [Source].[Skill] IS NULL THEN [ExaminerJob].[Skill] ELSE [Source].[Skill] END AS [Skill]
		,CASE WHEN [Source].[DateAllocated] IS NULL THEN [ExaminerJob].[DateAllocated] ELSE [Source].[DateAllocated] END AS [DateAllocated]
		,CASE WHEN [Source].[ReceivedDate] IS NULL THEN [ExaminerJob].[ReceivedDate] ELSE [Source].[ReceivedDate] END AS [ReceivedDate]
		,CASE WHEN [Source].[DueDate] IS NULL THEN [ExaminerJob].[DueDate] ELSE [Source].[DueDate] END AS [DueDate]
		,CASE WHEN [Source].[ExaminerCost] IS NULL THEN [ExaminerJob].[ExaminerCost] ELSE [Source].[ExaminerCost] END AS [ExaminerCost]
		,CASE WHEN [Source].[ProductSpecificationId] IS NULL THEN [ExaminerJob].[ProductSpecificationId] ELSE [Source].[ProductSpecificationId] END AS [ProductSpecificationId]
		,CASE WHEN [Source].[ProductSpecificationCode] IS NULL THEN [ExaminerJob].[ProductSpecificationCode] ELSE [Source].[ProductSpecificationCode] END AS [ProductSpecificationCode]
		,CASE WHEN [Source].[ProductSpecificationDescription] IS NULL THEN [ExaminerJob].[ProductSpecificationDescription] ELSE [Source].[ProductSpecificationDescription] END AS [ProductSpecificationDescription]
		,CASE WHEN [Source].[GLCodeId] IS NULL THEN [ExaminerJob].[GLCodeId] ELSE [Source].[GLCodeId] END AS [GLCodeId]
		,CASE WHEN [Source].[GLCode] IS NULL THEN [ExaminerJob].[GLCode] ELSE [Source].[GLCode] END AS [GLCode]
		,CASE WHEN [Source].[ExaminerPayRollStatusId] IS NULL THEN [ExaminerJob].[ExaminerPayRollStatusId] ELSE [Source].[ExaminerPayRollStatusId] END AS [ExaminerPayRollStatusId]
		,CASE WHEN [Source].[ExaminerPayRollStatus] IS NULL THEN [ExaminerJob].[ExaminerPayRollStatus] ELSE [Source].[ExaminerPayRollStatus] END AS [ExaminerPayRollStatus]
		,CASE WHEN [Source].[PayRollModifiedDate] IS NULL THEN [ExaminerJob].[PayRollModifiedDate] ELSE [Source].[PayRollModifiedDate] END AS [PayRollModifiedDate]
		,CASE WHEN [Source].[PayRollModifiedUser] IS NULL THEN [ExaminerJob].[PayRollModifiedUser] ELSE [Source].[PayRollModifiedUser] END AS [PayRollModifiedUser]
		,CASE WHEN [Source].[PayRollAccountingReference] IS NULL THEN [ExaminerJob].[PayRollAccountingReference] ELSE [Source].[PayRollAccountingReference] END AS [PayRollAccountingReference]

		,@Date AS [ModifiedDate]		
		,CASE WHEN [Source].[JobExaminerId] IS NULL THEN 'Deleted' ELSE 'NewOrModified' END AS [RowStatus]	
	    
	FROM
	(
	SELECT je.jobExaminerId AS [JobExaminerId],
		   ts.TestSittingId AS [TestSittingId],
		   j.jobId AS [JobId],
		   cat.CredentialApplicationTypeId AS [ApplicationTypeId],
		   cat.DisplayName AS [ApplicationType],
		   e.NaatiNumber AS [ExaminerCustomerNumber],
		   ct.CredentialTypeId AS [CredentialTypeId],
		   ct.InternalName AS [CredentialType],
		   s.SkillId AS [SkillId],
		   REPLACE(REPLACE(dt.DisplayName, '[Language 1]', l1.[Name]), '[Language 2]', l2.Name) AS [Skill],
		   je.DateAllocated AS [DateAllocated],
		   je.ExaminerReceivedDate AS [ReceivedDate],
		   j.DueDate AS [DueDate],
		   je.ExaminerCost  AS [ExaminerCost],
		   ps.ProductSpecificationId AS [ProductSpecificationId],
		   ps.Code AS [ProductSpecificationCode],
		   ps.Description AS [ProductSpecificationDescription],
		   g.GLCodeId AS [GLCodeId],
		   g.Code AS [GLCode],
		   jeps.PayRollStatusId AS [ExaminerPayRollStatusId],
		   prs.DisplayName AS [ExaminerPayRollStatus],
		   pr.ModifiedDate AS [PayRollModifiedDate],
		   u.FullName AS [PayRollModifiedUser],
		   epr.AccountingReference AS [PayRollAccountingReference]
		   
		FROM [naati_db]..tblJobExaminer je 
		LEFT JOIN [naati_db]..tblJob j  ON j.jobID = je.jobID
		LEFT JOIN [naati_db]..tblTestResult tr ON tr.CurrentJobId = j.JobId
		LEFT JOIN [naati_db]..tblTestSitting ts ON tr.TestSittingId = ts.TestSittingId
		LEFT JOIN [naati_db]..tblCredentialRequest cr on ts.CredentialRequestId = cr.CredentialRequestId
		LEFT JOIN [naati_db]..tblCredentialApplication ca on cr.CredentialApplicationId = ca.CredentialApplicationId
		LEFT JOIN [naati_db]..tblCredentialApplicationType cat on ca.CredentialApplicationTypeId = cat.CredentialApplicationTypeId
		LEFT JOIN [naati_db]..tblCredentialType ct on cr.CredentialTypeId = ct.CredentialTypeId
		LEFT JOIN [naati_db]..tblSkill s on cr.SkillId = s.SkillId
		LEFT JOIN [naati_db]..tblDirectionType dt on s.DirectionTypeId = dt.DirectionTypeId
		LEFT JOIN [naati_db]..tblExaminerMarking em ON je.JobExaminerId = em.JobExaminerId
		LEFT JOIN [naati_db]..tblPanelMembership pm ON pm.PanelMembershipId = je.PanelMembershipId
		LEFT JOIN [naati_db]..tblPerson p ON p.PersonId = pm.PersonId
		LEFT JOIN [naati_db]..tblEntity e ON e.EntityId = p.EntityId
		LEFT JOIN [naati_db]..vwJobExaminerPayrollStatus jeps ON je.JobExaminerId = jeps.JobExaminerId
		LEFT JOIN [naati_db]..tblProductSpecification ps ON je.ProductSpecificationId = ps.ProductSpecificationId
		LEFT JOIN [naati_db]..tblGLCode g on g.GLCodeId = ps.GLCodeId
		LEFT JOIN [naati_db]..tblJobExaminerPayroll epr ON epr.JobExaminerId = jeps.JobExaminerId	
		LEFT JOIN [naati_db]..tblPayRoll pr ON pr.PayRollId = epr.PayRollId
		LEFT JOIN [naati_db]..tluPayrollStatus prs ON prs.payrollStatusId = jeps.payrollStatusId	 
		LEFT JOIN [naati_db]..tblUser u ON u.UserId = pr.ModifiedUserId
		INNER JOIN [naati_db]..tblLanguage l1 ON s.Language1Id = l1.LanguageId
		INNER JOIN [naati_db]..tblLanguage l2 ON s.Language2Id = l2.LanguageId

	) [Source]
	FULL OUTER JOIN [ExaminerJob] ON [Source].[JobExaminerId] = [ExaminerJob].[JobExaminerId]
	WHERE (([Source].[TestSittingId] IS NOT NULL AND [ExaminerJob].[TestSittingId] IS NULL) OR ([Source].[TestSittingId] IS NULL AND [ExaminerJob].[TestSittingId] IS NOT NULL) OR (([Source].[TestSittingId] IS NOT NULL AND [ExaminerJob].[TestSittingId] IS NOT NULL) AND [Source].[TestSittingId] != [ExaminerJob].[TestSittingId]))
	OR (([Source].[JobId] IS NOT NULL AND [ExaminerJob].[JobId] IS NULL) OR ([Source].[JobId] IS NULL AND [ExaminerJob].[JobId] IS NOT NULL) OR (([Source].[JobId] IS NOT NULL AND [ExaminerJob].[JobId] IS NOT NULL) AND [Source].[JobId] != [ExaminerJob].[JobId]))
	OR (([Source].[ApplicationTypeId] IS NOT NULL AND [ExaminerJob].[ApplicationTypeId] IS NULL) OR ([Source].[ApplicationTypeId] IS NULL AND [ExaminerJob].[ApplicationTypeId] IS NOT NULL) OR (([Source].[ApplicationTypeId] IS NOT NULL AND [ExaminerJob].[ApplicationTypeId] IS NOT NULL) AND [Source].[ApplicationTypeId] != [ExaminerJob].[ApplicationTypeId]))
	OR (([Source].[ApplicationType] IS NOT NULL AND [ExaminerJob].[ApplicationType] IS NULL) OR ([Source].[ApplicationType] IS NULL AND [ExaminerJob].[ApplicationType] IS NOT NULL) OR (([Source].[ApplicationType] IS NOT NULL AND [ExaminerJob].[ApplicationType] IS NOT NULL) AND [Source].[ApplicationType] != [ExaminerJob].[ApplicationType]))
	OR (([Source].[ExaminerCustomerNumber] IS NOT NULL AND [ExaminerJob].[ExaminerCustomerNumber] IS NULL) OR ([Source].[ExaminerCustomerNumber] IS NULL AND [ExaminerJob].[ExaminerCustomerNumber] IS NOT NULL) OR (([Source].[ExaminerCustomerNumber] IS NOT NULL AND [ExaminerJob].[ExaminerCustomerNumber] IS NOT NULL) AND [Source].[ExaminerCustomerNumber] != [ExaminerJob].[ExaminerCustomerNumber]))
	OR (([Source].[CredentialTypeId] IS NOT NULL AND [ExaminerJob].[CredentialTypeId] IS NULL) OR ([Source].[CredentialTypeId] IS NULL AND [ExaminerJob].[CredentialTypeId] IS NOT NULL) OR (([Source].[CredentialTypeId] IS NOT NULL AND [ExaminerJob].[CredentialTypeId] IS NOT NULL) AND [Source].[CredentialTypeId] != [ExaminerJob].[CredentialTypeId]))
	OR (([Source].[CredentialType] IS NOT NULL AND [ExaminerJob].[CredentialType] IS NULL) OR ([Source].[CredentialType] IS NULL AND [ExaminerJob].[CredentialType] IS NOT NULL) OR (([Source].[CredentialType] IS NOT NULL AND [ExaminerJob].[CredentialType] IS NOT NULL) AND [Source].[CredentialType] != [ExaminerJob].[CredentialType]))
	OR (([Source].[SkillId] IS NOT NULL AND [ExaminerJob].[SkillId] IS NULL) OR ([Source].[SkillId] IS NULL AND [ExaminerJob].[SkillId] IS NOT NULL) OR (([Source].[SkillId] IS NOT NULL AND [ExaminerJob].[SkillId] IS NOT NULL) AND [Source].[SkillId] != [ExaminerJob].[SkillId]))
	OR (([Source].[Skill] IS NOT NULL AND [ExaminerJob].[Skill] IS NULL) OR ([Source].[Skill] IS NULL AND [ExaminerJob].[Skill] IS NOT NULL) OR (([Source].[Skill] IS NOT NULL AND [ExaminerJob].[Skill] IS NOT NULL) AND [Source].[Skill] != [ExaminerJob].[Skill]))
	OR (([Source].[DateAllocated] IS NOT NULL AND [ExaminerJob].[DateAllocated] IS NULL) OR ([Source].[DateAllocated] IS NULL AND [ExaminerJob].[DateAllocated] IS NOT NULL) OR (([Source].[DateAllocated] IS NOT NULL AND [ExaminerJob].[DateAllocated] IS NOT NULL) AND [Source].[DateAllocated] != [ExaminerJob].[DateAllocated]))
	OR (([Source].[ReceivedDate] IS NOT NULL AND [ExaminerJob].[ReceivedDate] IS NULL) OR ([Source].[ReceivedDate] IS NULL AND [ExaminerJob].[ReceivedDate] IS NOT NULL) OR (([Source].[ReceivedDate] IS NOT NULL AND [ExaminerJob].[ReceivedDate] IS NOT NULL) AND [Source].[ReceivedDate] != [ExaminerJob].[ReceivedDate]))
	OR (([Source].[DueDate] IS NOT NULL AND [ExaminerJob].[DueDate] IS NULL) OR ([Source].[DueDate] IS NULL AND [ExaminerJob].[DueDate] IS NOT NULL) OR (([Source].[DueDate] IS NOT NULL AND [ExaminerJob].[DueDate] IS NOT NULL) AND [Source].[DueDate] != [ExaminerJob].[DueDate]))
	OR (([Source].[ExaminerCost] IS NOT NULL AND [ExaminerJob].[ExaminerCost] IS NULL) OR ([Source].[ExaminerCost] IS NULL AND [ExaminerJob].[ExaminerCost] IS NOT NULL) OR (([Source].[ExaminerCost] IS NOT NULL AND [ExaminerJob].[ExaminerCost] IS NOT NULL) AND [Source].[ExaminerCost] != [ExaminerJob].[ExaminerCost]))
	OR (([Source].[ProductSpecificationId] IS NOT NULL AND [ExaminerJob].[ProductSpecificationId] IS NULL) OR ([Source].[ProductSpecificationId] IS NULL AND [ExaminerJob].[ProductSpecificationId] IS NOT NULL) OR (([Source].[ProductSpecificationId] IS NOT NULL AND [ExaminerJob].[ProductSpecificationId] IS NOT NULL) AND [Source].[ProductSpecificationId] != [ExaminerJob].[ProductSpecificationId]))
	OR (([Source].[ProductSpecificationCode] IS NOT NULL AND [ExaminerJob].[ProductSpecificationCode] IS NULL) OR ([Source].[ProductSpecificationCode] IS NULL AND [ExaminerJob].[ProductSpecificationCode] IS NOT NULL) OR (([Source].[ProductSpecificationCode] IS NOT NULL AND [ExaminerJob].[ProductSpecificationCode] IS NOT NULL) AND [Source].[ProductSpecificationCode] != [ExaminerJob].[ProductSpecificationCode]))
	OR (([Source].[ProductSpecificationDescription] IS NOT NULL AND [ExaminerJob].[ProductSpecificationDescription] IS NULL) OR ([Source].[ProductSpecificationDescription] IS NULL AND [ExaminerJob].[ProductSpecificationDescription] IS NOT NULL) OR (([Source].[ProductSpecificationDescription] IS NOT NULL AND [ExaminerJob].[ProductSpecificationDescription] IS NOT NULL) AND [Source].[ProductSpecificationDescription] != [ExaminerJob].[ProductSpecificationDescription]))
	OR (([Source].[GLCodeId] IS NOT NULL AND [ExaminerJob].[GLCodeId] IS NULL) OR ([Source].[GLCodeId] IS NULL AND [ExaminerJob].[GLCodeId] IS NOT NULL) OR (([Source].[GLCodeId] IS NOT NULL AND [ExaminerJob].[GLCodeId] IS NOT NULL) AND [Source].[GLCodeId] != [ExaminerJob].[GLCodeId]))
	OR (([Source].[GLCode] IS NOT NULL AND [ExaminerJob].[GLCode] IS NULL) OR ([Source].[GLCode] IS NULL AND [ExaminerJob].[GLCode] IS NOT NULL) OR (([Source].[GLCode] IS NOT NULL AND [ExaminerJob].[GLCode] IS NOT NULL) AND [Source].[GLCode] != [ExaminerJob].[GLCode]))
	OR (([Source].[ExaminerPayRollStatusId] IS NOT NULL AND [ExaminerJob].[ExaminerPayRollStatusId] IS NULL) OR ([Source].[ExaminerPayRollStatusId] IS NULL AND [ExaminerJob].[ExaminerPayRollStatusId] IS NOT NULL) OR (([Source].[ExaminerPayRollStatusId] IS NOT NULL AND [ExaminerJob].[ExaminerPayRollStatusId] IS NOT NULL) AND [Source].[ExaminerPayRollStatusId] != [ExaminerJob].[ExaminerPayRollStatusId]))
	OR (([Source].[ExaminerPayRollStatus] IS NOT NULL AND [ExaminerJob].[ExaminerPayRollStatus] IS NULL) OR ([Source].[ExaminerPayRollStatus] IS NULL AND [ExaminerJob].[ExaminerPayRollStatus] IS NOT NULL) OR (([Source].[ExaminerPayRollStatus] IS NOT NULL AND [ExaminerJob].[ExaminerPayRollStatus] IS NOT NULL) AND [Source].[ExaminerPayRollStatus] != [ExaminerJob].[ExaminerPayRollStatus]))
	OR (([Source].[PayRollModifiedDate] IS NOT NULL AND [ExaminerJob].[PayRollModifiedDate] IS NULL) OR ([Source].[PayRollModifiedDate] IS NULL AND [ExaminerJob].[PayRollModifiedDate] IS NOT NULL) OR (([Source].[PayRollModifiedDate] IS NOT NULL AND [ExaminerJob].[PayRollModifiedDate] IS NOT NULL) AND [Source].[PayRollModifiedDate] != [ExaminerJob].[PayRollModifiedDate]))
	OR (([Source].[PayRollModifiedUser] IS NOT NULL AND [ExaminerJob].[PayRollModifiedUser] IS NULL) OR ([Source].[PayRollModifiedUser] IS NULL AND [ExaminerJob].[PayRollModifiedUser] IS NOT NULL) OR (([Source].[PayRollModifiedUser] IS NOT NULL AND [ExaminerJob].[PayRollModifiedUser] IS NOT NULL) AND [Source].[PayRollModifiedUser] != [ExaminerJob].[PayRollModifiedUser]))
	OR (([Source].[PayRollAccountingReference] IS NOT NULL AND [ExaminerJob].[PayRollAccountingReference] IS NULL) OR ([Source].[PayRollAccountingReference] IS NULL AND [ExaminerJob].[PayRollAccountingReference] IS NOT NULL) OR (([Source].[PayRollAccountingReference] IS NOT NULL AND [ExaminerJob].[PayRollAccountingReference] IS NOT NULL) AND [Source].[PayRollAccountingReference] != [ExaminerJob].[PayRollAccountingReference]))
	
	--select * from @ExaminerJobHistory
		
	BEGIN TRANSACTION 
	
	   --Merge operation delete
		MERGE ExaminerJobHistory AS Target USING(select * from @ExaminerJobHistory where [RowStatus] = 'Deleted' ) AS Source ON Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.RowStatus = 'Latest'
		WHEN MATCHED THEN 
		  UPDATE Set     
		 Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Deleted';

		--Merge operation Obsolete
		MERGE ExaminerJobHistory AS Target USING(	select * from @ExaminerJobHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate <> @Date
		WHEN MATCHED THEN 
		  UPDATE Set     		  
		  Target.[ObsoletedDate] =  @Date,
		  Target.[RowStatus] = 'Obsolete';

		--Merge operation Latest
		MERGE ExaminerJobHistory AS Target USING(	select * from @ExaminerJobHistory where [RowStatus] = 'NewOrModified' ) AS Source ON Target.[JobExaminerId] = Source.[JobExaminerId] AND Target.RowStatus = 'Latest' AND  Target.ModifiedDate = @Date
		WHEN MATCHED THEN 
		  UPDATE Set     
		    Target.JobExaminerId = Source.JobExaminerId
		  ,Target.TestSittingId = Source.TestSittingId
		  ,Target.JobId = Source.JobId
		  ,Target.ApplicationTypeId = Source.ApplicationTypeId
		  ,Target.ApplicationType = Source.ApplicationType
		  ,Target.ExaminerCustomerNumber = Source.ExaminerCustomerNumber      
		  ,Target.CredentialTypeId = Source.CredentialTypeId
		  ,Target.CredentialType = Source.CredentialType
		  ,Target.SkillId = Source.SkillId
		  ,Target.Skill = Source.Skill

		  ,Target.DateAllocated = Source.DateAllocated
		  ,Target.ReceivedDate = Source.ReceivedDate
		  ,Target.DueDate = Source.DueDate
		  ,Target.ExaminerCost = Source.ExaminerCost
		  ,Target.ProductSpecificationId = Source.ProductSpecificationId
		  ,Target.ProductSpecificationCode = Source.ProductSpecificationCode
		  ,Target.ProductSpecificationDescription = Source.ProductSpecificationDescription
		  ,Target.GLCodeId = Source.GLCodeId
		  ,Target.GLCode = Source.GLCode
		  ,Target.ExaminerPayRollStatusId = Source.ExaminerPayRollStatusId
		  ,Target.ExaminerPayRollStatus = Source.ExaminerPayRollStatus
		  ,Target.PayRollModifiedDate = Source.PayRollModifiedDate

		  ,Target.PayRollModifiedUser = Source.PayRollModifiedUser
		  ,Target.PayRollAccountingReference = Source.PayRollAccountingReference
		  ,Target.ModifiedDate = @Date

		  ,Target.[RowStatus] = 'Latest'
		WHEN NOT MATCHED THEN 
		  INSERT([JobExaminerId],[TestSittingId],[JobId],[ApplicationTypeId],[ApplicationType],[ExaminerCustomerNumber],[CredentialTypeId],[CredentialType],[SkillId],[Skill],[DateAllocated],[ReceivedDate],[DueDate],[ExaminerCost],
			 [ProductSpecificationId],[ProductSpecificationCode],[ProductSpecificationDescription],[GLCodeId],[GLCode],[ExaminerPayRollStatusId],[ExaminerPayRollStatus],[PayRollModifiedDate],[PayRollModifiedUser],
			 [PayRollAccountingReference],[ModifiedDate],[RowStatus])

		  VALUES (Source.[JobExaminerId],Source.[TestSittingId],Source.[JobId],Source.[ApplicationTypeId],Source.[ApplicationType],Source.[ExaminerCustomerNumber],Source.[CredentialTypeId],Source.[CredentialType],Source.[SkillId],Source.[Skill],
			  Source.[DateAllocated],Source.[ReceivedDate],Source.[DueDate],Source.[ExaminerCost],Source.[ProductSpecificationId],Source.[ProductSpecificationCode],Source.[ProductSpecificationDescription],Source.[GLCodeId],Source.[GLCode],
			  Source.[ExaminerPayRollStatusId],Source.[ExaminerPayRollStatus],Source.[PayRollModifiedDate],Source.[PayRollModifiedUser],Source.[PayRollAccountingReference],
			  @Date, 'Latest');		  
		  	
		  
	    COMMIT TRANSACTION;	
				
END
