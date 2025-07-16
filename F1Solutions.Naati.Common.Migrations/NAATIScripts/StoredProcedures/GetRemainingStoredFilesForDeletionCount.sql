ALTER PROCEDURE GetRemainingStoredFilesForDeletionCount
@Log NVARCHAR(MAX) OUTPUT, 
@TotalCount int OUTPUT
AS
declare @Policy1 as int;set @Policy1 = 365;
declare @Policy2 as int;set @Policy2 = 1095;
declare @Policy3 as int;set @Policy3 = 1095;
declare @Policy4 as int;set @Policy4 = 1095;
declare @Policy5 as int;set @Policy5 = 1095;
declare @Policy6 as int;set @Policy6 = 183;
declare @Policy7 as int;set @Policy7 = 1825;
declare @Policy8 as int;set @Policy8 = 1460;
declare @Policy9 as int;set @Policy9 = 365;
declare @Count as int; set @Count = 0;
declare @Delimiter as varchar(2);
set @TotalCount = 0;
set @Delimiter = CHAR(13)+CHAR(10);
BEGIN
	-- Policy 1
	select @Count = (select Count(a.StoredFileId)
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 25 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = 'CredentialApplicationAttachment, AIIC Membership Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 2
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 26 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, AITC Membership Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 3
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 21 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

    select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Application Form:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 4
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 13 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Auslan Proficiency Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 5
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 43 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Blank Template:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 6
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 40 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Candidate Brief:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	--policy 7, 8, 9 = null

	-- Policy 10
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 30 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

    select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Candidate Test Material:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 11
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 18 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Chuchotage Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 12 null

	-- Policy 13
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 4 and
	e.StatusChangeDate < DATEADD(day, -@Policy2, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

    select @Log = @Log + @Delimiter + 'TestSittingDocument, English Marking:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 14 
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 14 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, English Proficiency Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 15
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 15 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Ethical Competency Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 16 null

	-- Policy 17
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 31 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Examiner Test Material:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 18
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblPersonAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	where 
	DocumentTypeId = 39 and
	UploadedDateTime < DATEADD(day, -@Policy7, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

	select @Log = @Log + @Delimiter + 'PersonAttachment, General:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count



	-- Policy 19
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblNoteAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	where 
	DocumentTypeId = 39 and
	UploadedDateTime < DATEADD(day, -@Policy7, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

    select @Log = @Log + @Delimiter + 'NoteAttachment, General:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 20
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 39 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, General:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 21
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblNoteAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	where 
	DocumentTypeId = 8 and
	UploadedDateTime < DATEADD(day, -@Policy3, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

	select @Log = @Log + @Delimiter + 'NoteAttachment, General document:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 22
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 8 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

    select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, General document:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 23
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblNoteAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblNote c on a.NoteId = c.NoteId
	where 
	DocumentTypeId = 8 and
	c.ModifiedDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2))
	select @Log = @Log + @Delimiter + 'NoteAttachment, General document:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 24

	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 7 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, General Test Document:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 25 null

	-- Policy 26, 27
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblNoteAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblNote c on a.NoteId = c.NoteId
	where 
	DocumentTypeId = 9 and
	c.ModifiedDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

	select @Log = @Log + @Delimiter + 'NoteAttachment, Identification:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 28
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 9 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Identification:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 29 same as 26, 27

	-- Policy 30
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 16 and
	c.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Intercultural Competency Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 31
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblTestResult d on c.TestSittingId = d.TestSittingId
	where 
	DocumentTypeId = 3 and
	d.ProcessedDate < DATEADD(day, -@Policy8, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Marked Test Asset:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 32
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 37 and
	e.StatusChangeDate < DATEADD(day, -@Policy6, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Medical Certificate:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 33
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblPersonAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	where 
	DocumentTypeId = 38 and
	UploadedDateTime < DATEADD(day, -@Policy3, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

    select @Log = @Log + @Delimiter + 'PersonAttachment, NAATI Employment:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 34
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 38 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, NAATI Employment:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 35
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 17 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


    select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Other Application Related:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 36 - photo needs to be an insert.

	-- Policy 37
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 36 and
	e.StatusChangeDate < DATEADD(day, -@Policy2, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Problem Sheet:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 38
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblProfessionalDevelopmentActivityAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblProfessionalDevelopmentActivity c on a.ProfessionalDevelopmentActivityId = c.ProfessionalDevelopmentActivityId
	join tblProfessionalDevelopmentCredentialApplication d on c.ProfessionalDevelopmentActivityId = d.ProfessionalDevelopmentActivityId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 28 and
	e.StatusChangeDate < DATEADD(day, -@Policy9, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'ProfessionalDevelopmentActivityAttachment, Professional Development Activity:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 39
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 28 and
	c.StatusChangeDate < DATEADD(day, -@Policy9, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Professional Development Activity:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 40 same as 38

	-- Policy 41
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 32 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Professional Development Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 42
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 24 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Purchase Order:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 43
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 44 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Request Checklist:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 44
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 6 and
	e.StatusChangeDate < DATEADD(day, -@Policy2, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Review Report:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 45 - null

	-- Policy 46
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 25 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, AIIC Membership Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 47 
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 42 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Source Template:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 48 
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblMaterialRequestRoundAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblMaterialRequestRound c on a.MaterialRequestRoundId = c.MaterialRequestRoundId
	join tblMaterialRequest d on c.MaterialRequestId = d.MaterialRequestId
	where 
	DocumentTypeId = 47 and
	d.StatusChangeDate < DATEADD(day, -@Policy5, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	d.MaterialRequestStatusTypeId in (3))

	select @Log = @Log + @Delimiter + 'MaterialRequestRoundAttachment, Submission Checklist:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 49  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 5 and
	e.StatusChangeDate < DATEADD(day, -@Policy2, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Test Material:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 50  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 12 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Training Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 51  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 11 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Transcript:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 52  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 41 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Transcript or Proof of Enrolment:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 53  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblTestSittingDocument a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblTestSitting c on a.TestSittingId = c.TestSittingId
	join tblCredentialRequest d on c.CredentialRequestId = d.CredentialRequestId
	join tblCredentialApplication e on d.CredentialApplicationId = e.CredentialApplicationId
	where 
	DocumentTypeId = 2 and
	e.StatusChangeDate < DATEADD(day, -@Policy2, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	e.CredentialApplicationStatusTypeId in (6)) -- completed

	select @Log = @Log + @Delimiter + 'TestSittingDocument, Unmarked Test Asset:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 54 and 56
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblWorkPracticeAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	where 
	DocumentTypeId = 27 and
	UploadedDateTime < DATEADD(day, -@Policy3, GetDate()) and
	StoredFileStatusTypeId in (1, 2))

	select @Log = @Log + @Delimiter + 'WorkPracticeAttachment, Work Practice:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count


	-- Policy 55  
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 27 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Work Practice:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Policy 57 and 58 
	set @Count = 
	(select Count(a.StoredFileId) 
	from 
	tblCredentialApplicationAttachment a 
	join tblStoredFile b on a.StoredFileId = b.StoredFileId
	join tblCredentialApplication c on a.CredentialApplicationId = c.CredentialApplicationId
	where 
	DocumentTypeId = 10 and
	c.StatusChangeDate < DATEADD(day, -@Policy1, GetDate()) and
	StoredFileStatusTypeId in (1, 2) and
	c.CredentialApplicationStatusTypeId in (6)) -- completed


	select @Log = @Log + @Delimiter + 'CredentialApplicationAttachment, Work Practice Evidence:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	-- Person Image
	set @Count = 
	(select Count(a.PersonImageId)
	from
	tblPersonImage a
	where
	a.PersonId NOT IN
	(
		SELECT DISTINCT certificationPeriod.personId
		FROM
			tblCertificationPeriod certificationPeriod 
		WHERE 
			certificationPeriod.EndDate > GetDATE() 		
	) AND
	a.PhotoDate < DATEADD(day, -@Policy4, GETDATE()))

	select @Log = @Log + @Delimiter + 'PersonImage, Image:' + CONVERT(varchar(10),@Count);
	set @TotalCount += @Count

	select @Log as 'Log', @TotalCount as 'TotalCount'
END