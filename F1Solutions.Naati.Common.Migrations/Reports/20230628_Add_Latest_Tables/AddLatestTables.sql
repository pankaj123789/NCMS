
IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'ApplicationCustomFieldsLatest')
BEGIN
	CREATE TABLE [dbo].[ApplicationCustomFieldsLatest](
		[ApplicationCustomFieldId] [int] NOT NULL,
		[PersonId] [int] NOT NULL,
		[ApplicationId] [int] NOT NULL,
		[ApplicationType] [nvarchar](50) NOT NULL,
		[ApplicationStatus] [nvarchar](50) NOT NULL,
		[ApplicationStatusModifiedDate] [datetime] NOT NULL,
		[ApplicationEnteredDate] [datetime] NOT NULL,
		[Section] [nvarchar](100) NULL,
		[FieldName] [nvarchar](50) NOT NULL,
		[Type] [nvarchar](50) NOT NULL,
		[Value] [nvarchar](max) NULL,
		[ModifiedDate] [datetime] NOT NULL,
		[ObsoletedDate] [datetime] NULL,
		[RowStatus] [nvarchar](50) NOT NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'ApplicationLatest')
BEGIN
CREATE TABLE [dbo].[ApplicationLatest](
	[ApplicationId] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[PersonId] [int] NOT NULL,
	[EnteredDate] [datetime] NOT NULL,
	[EnteredOffice] [varchar](100) NOT NULL,
	[ApplicationStatus] [nvarchar](100) NULL,
	[StatusDateModified] [datetime] NULL,
	[Sponsor] [nvarchar](100) NULL,
	[ObsoletedDate] [datetime] NULL,
	[Title] [nvarchar](50) NULL,
	[GivenName] [nvarchar](100) NULL,
	[OtherNames] [nvarchar](100) NULL,
	[FamilyName] [nvarchar](100) NULL,
	[PrimaryAddress] [nvarchar](500) NULL,
	[Country] [nvarchar](50) NULL,
	[PrimaryPhone] [nvarchar](60) NULL,
	[PrimaryEmail] [nvarchar](200) NULL,
	[ApplicationType] [nvarchar](50) NULL,
	[ApplicationReference] [nvarchar](50) NULL,
	[ApplicationOwner] [nvarchar](100) NULL,
	[StatusModifiedUser] [nvarchar](100) NULL,
	[NAATINumber] [int] NULL,
	[PractitionerNumber] [varchar](50) NULL,
	[EnteredUser] [nvarchar](100) NULL,
	[SponsoredOrganisationId] [int] NULL,
	[SponsoredOrganisationName] [nvarchar](100) NULL,
	[SponsoredContactId] [int] NULL,
	[SponsoredContactName] [nvarchar](500) NULL,
	[PreferredTestLocationState] [char](3) NULL,
	[PreferredTestLocationCity] [nvarchar](500) NULL,
	[State] [varchar](3) NULL,
	[Postcode] [varchar](4) NULL,
	[RowStatus] [nvarchar](50) NOT NULL,
	[AutoCreated] [bit] NULL
) ON [PRIMARY]
END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'CredentialRequestsLatest')
BEGIN

CREATE TABLE [dbo].[CredentialRequestsLatest](
	[CredentialRequestId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[GivenName] [nvarchar](100) NULL,
	[OtherNames] [nvarchar](100) NULL,
	[FamilyName] [nvarchar](100) NULL,
	[PrimaryAddress] [nvarchar](500) NULL,
	[Country] [nvarchar](50) NULL,
	[PrimaryEmail] [nvarchar](200) NULL,
	[PrimaryPhone] [nvarchar](60) NULL,
	[ApplicationId] [int] NOT NULL,
	[ApplicationType] [nvarchar](50) NOT NULL,
	[ApplicationReference] [nvarchar](50) NOT NULL,
	[ApplicationOwner] [nvarchar](100) NULL,
	[CredentialTypeInternalName] [nvarchar](50) NOT NULL,
	[CredentialTypeExternalName] [nvarchar](50) NOT NULL,
	[Certification] [bit] NOT NULL,
	[Language1] [nvarchar](50) NOT NULL,
	[Language2] [nvarchar](50) NOT NULL,
	[DirectionDisplayName] [nvarchar](max) NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObsoletedDate] [datetime] NULL,
	[StatusDateModified] [datetime] NULL,
	[StatusModifiedUser] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[NAATINumber] [int] NULL,
	[PractitionerNumber] [varchar](50) NULL,
	[Language1Code] [nvarchar](10) NULL,
	[Language1Group] [nvarchar](50) NULL,
	[Language2Code] [nvarchar](10) NULL,
	[Language2Group] [nvarchar](50) NULL,
	[State] [varchar](3) NULL,
	[Postcode] [varchar](4) NULL,
	[CredentialId] [int] NULL,
	[LinkedCredentialRequestId] [int] NULL,
	[LinkedCredentialRequestReason] [nvarchar](50) NULL,
	[RowStatus] [nvarchar](50) NOT NULL,
	[AutoCreated] [bit] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'MarkLatest')
BEGIN

CREATE TABLE [dbo].[MarkLatest](
	[PersonId] [int] NOT NULL,
	[TestSittingId] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObsoletedDate] [datetime] NULL,
	[ExaminerNAATINumber] [int] NULL,
	[ExaminerName] [nvarchar](252) NULL,
	[IncludeMarks] [bit] NULL,
	[MarkId] [int] NOT NULL,
	[Mark] [float] NULL,
	[TotalMark] [int] NULL,
	[OverallMark] [float] NULL,
	[TestResultId] [int] NULL,
	[ComponentName] [nvarchar](100) NOT NULL,
	[ExaminerType] [nvarchar](50) NULL,
	[Cost] [money] NULL,
	[PaperLost] [bit] NULL,
	[PaperReceived] [datetime] NULL,
	[SentToPayroll] [datetime] NULL,
	[PassMark] [float] NULL,
	[OverallPassMark] [int] NULL,
	[OverallTotalMark] [int] NULL,
	[PrimaryFailureReason] [nvarchar](50) NULL,
	[PoorPerformanceReasons] [nvarchar](100) NULL,
	[MarkerComments] [nvarchar](4000) NULL,
	[SubmittedDate] [datetime] NULL,
	[RowStatus] [nvarchar](50) NOT NULL
) ON [PRIMARY]

END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'MarkRubricLatest')
BEGIN

CREATE TABLE [dbo].[MarkRubricLatest](
	[TestResultId] [int] NOT NULL,
	[TestComponentId] [int] NOT NULL,
	[JobExaminerId] [int] NOT NULL,
	[RubricAssessementCriterionResultId] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObsoletedDate] [datetime] NULL,
	[TestSittingId] [int] NULL,
	[TestSessionId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[CustomerNo] [int] NULL,
	[CandidateName] [nvarchar](252) NULL,
	[PaidReview] [bit] NULL,
	[Supplementary] [bit] NULL,
	[TestTaskTypeLabel] [nvarchar](100) NULL,
	[TestTaskTypeName] [nvarchar](50) NULL,
	[TestTaskLabel] [nvarchar](100) NULL,
	[TestTaskName] [nvarchar](100) NULL,
	[TestTaskNumber] [int] NULL,
	[MarkType] [nvarchar](100) NULL,
	[ResultType] [nvarchar](100) NULL,
	[ExaminerCustomerNo] [int] NULL,
	[ExaminerName] [nvarchar](252) NULL,
	[ExaminerType] [nvarchar](100) NULL,
	[Cost] [money] NULL,
	[ExaminerSubmittedDate] [datetime] NULL,
	[IncludeMarks] [bit] NULL,
	[WasAttempted] [bit] NULL,
	[Successful] [bit] NULL,
	[RubricCompetencyLabel] [nvarchar](100) NULL,
	[RubricCompetencyName] [nvarchar](50) NULL,
	[RubricCriterionName] [nvarchar](100) NULL,
	[RubricCriterionLabel] [nvarchar](100) NULL,
	[RubricSelectedBandLabel] [nvarchar](100) NULL,
	[RubricSelectedBandLevel] [int] NULL,
	[RowStatus] [nvarchar](50) NOT NULL
) ON [PRIMARY]

END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'TestLatest')
BEGIN

CREATE TABLE [dbo].[TestLatest](
	[TestSittingId] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[CredentialRequestId] [int] NULL,
	[TestResultId] [int] NULL,
	[CredentialApplicationId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Rejected] [bit] NOT NULL,
	[Sat] [bit] NULL,
	[ResultType] [varchar](50) NULL,
	[ThirdExaminerRequired] [bit] NULL,
	[ProcessedDate] [datetime] NULL,
	[SatDate] [datetime] NULL,
	[ResultChecked] [bit] NULL,
	[ObsoletedDate] [datetime] NULL,
	[Venue] [nvarchar](100) NOT NULL,
	[CandidateName] [nvarchar](252) NULL,
	[LanguageName1] [nvarchar](50) NULL,
	[LanguageName2] [nvarchar](50) NOT NULL,
	[CredentialTypeInternalName] [nvarchar](50) NULL,
	[CredentialTypeExternalName] [nvarchar](50) NULL,
	[Skill] [nvarchar](max) NULL,
	[ApplicationType] [nvarchar](50) NULL,
	[SupplementaryTest] [bit] NOT NULL,
	[RowStatus] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'TestResultLatest')
BEGIN

CREATE TABLE [dbo].[TestResultLatest](
	[TestResultId] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObsoletedDate] [datetime] NULL,
	[PersonId] [int] NULL,
	[TestSittingId] [int] NULL,
	[ResultDueDate] [datetime] NULL,
	[LanguageName1] [nvarchar](50) NULL,
	[CandidateName] [nvarchar](252) NULL,
	[NAATINumber] [int] NULL,
	[NAATINumberDisplay] [nvarchar](50) NULL,
	[PaidReview] [bit] NULL,
	[TotalMarks] [float] NULL,
	[PassMark] [float] NULL,
	[TotalCost] [money] NULL,
	[GeneralComments] [nvarchar](500) NULL,
	[OverallResult] [nvarchar](50) NULL,
	[ResultDate] [datetime] NULL,
	[LanguageName2] [nvarchar](50) NOT NULL,
	[CredentialTypeInternalName] [nvarchar](100) NULL,
	[CredentialTypeExternalName] [nvarchar](100) NULL,
	[EligibleForSupplementary] [bit] NOT NULL,
	[EligibleForConcededPass] [bit] NOT NULL,
	[MarksOverridden] [bit] NOT NULL,
	[RowStatus] [nvarchar](50) NOT NULL
) ON [PRIMARY]

END

IF NOT EXISTS(SELECT 1 FROM sys.tables WHERE name = N'TestSessionsLatest')
BEGIN

CREATE TABLE [dbo].[TestSessionsLatest](
	[TestSittingId] [int] NOT NULL,
	[TestSessionId] [int] NOT NULL,
	[CredentialRequestId] [int] NOT NULL,
	[TestSessionName] [nvarchar](200) NULL,
	[TestLocationState] [nvarchar](50) NULL,
	[TestLocationCountry] [nvarchar](50) NULL,
	[TestLocationName] [nvarchar](1000) NULL,
	[VenueName] [nvarchar](200) NULL,
	[VenueAddress] [nvarchar](510) NULL,
	[TestDate] [datetime] NULL,
	[TestStartTime] [datetime] NULL,
	[TestArrivalTime] [datetime] NULL,
	[TestEndTime] [datetime] NULL,
	[ApplicationType] [nvarchar](100) NULL,
	[CredentialTypeInternalName] [nvarchar](100) NULL,
	[CredentialTypeExternalName] [nvarchar](100) NULL,
	[TestSessionCompleted] [bit] NULL,
	[PersonId] [int] NOT NULL,
	[CustomerNo] [int] NULL,
	[Title] [nvarchar](50) NULL,
	[GivenName] [nvarchar](100) NULL,
	[OtherNames] [nvarchar](100) NULL,
	[FamilyName] [nvarchar](100) NULL,
	[PrimaryAddress] [nvarchar](500) NULL,
	[State] [char](50) NULL,
	[Postcode] [char](4) NULL,
	[Country] [nvarchar](50) NULL,
	[PrimaryPhone] [nvarchar](60) NULL,
	[PrimaryEmail] [nvarchar](200) NULL,
	[ApplicationId] [int] NULL,
	[ApplicationReference] [nvarchar](10) NULL,
	[Certification] [nvarchar](100) NULL,
	[Language1] [nvarchar](50) NULL,
	[Language1Code] [nvarchar](10) NULL,
	[Language1Group] [nvarchar](100) NULL,
	[Language2] [nvarchar](50) NULL,
	[Language2Code] [nvarchar](50) NULL,
	[Language2Group] [nvarchar](100) NULL,
	[Skill] [nvarchar](max) NULL,
	[Status] [nvarchar](100) NULL,
	[StatusDateModified] [datetime] NOT NULL,
	[StatusModifiedUser] [nvarchar](100) NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ObsoletedDate] [datetime] NULL,
	[Capacity] [int] NULL,
	[VenueCapacityOverridden] [bit] NULL,
	[RowStatus] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

