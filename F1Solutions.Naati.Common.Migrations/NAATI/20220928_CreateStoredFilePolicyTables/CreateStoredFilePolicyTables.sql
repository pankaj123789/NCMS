/****** Object:  Table [dbo].[tblStoredFileDeletePolicy]    Script Date: 27/09/2022 8:05:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
DROP TABLE IF EXISTS [dbo].[tblStoredFileDeletePolicyDocumentType]
GO
DROP TABLE IF EXISTS [dbo].[tblStoredFileDeletePolicy]
GO
CREATE TABLE [dbo].[tblStoredFileDeletePolicy](
	[StoredFileDeletePolicyId] [int] IDENTITY(1,1) NOT NULL,
	[PolicyExecutionOrder] [int] NOT NULL,
	[PolicyDescription] [nvarchar](max) NOT NULL,
	[ModifiedByNaati] [bit] NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedUser] [int] NULL,
	[DaysToKeep] [int] NOT NULL,
 CONSTRAINT [PK_tblStoredFileDeletePolicy] PRIMARY KEY CLUSTERED 
(
	[StoredFileDeletePolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblStoredFileDeletePolicyDocumentType]    Script Date: 27/09/2022 8:05:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblStoredFileDeletePolicyDocumentType](
	[StoredFileDeletePolicyDocumentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[StoredFileDeletePolicyId] [int] NULL,
	[Description] [nvarchar](max) NOT NULL,
	[EntityType] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblStoredFileDeletePolicyDocumentType] PRIMARY KEY CLUSTERED 
(
	[StoredFileDeletePolicyDocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblStoredFileDeletePolicy] ON 
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (1, 1, N'Delete after 1 year from Application Completed.', NULL, NULL, NULL, 365)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (2, 2, N'Delete after 3 years from Application Completed.', NULL, NULL, NULL, 1095)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (3, 3, N'Delete 3 years after the document was uploaded.', NULL, NULL, NULL, 1095)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (4, 4, N'If the user is not a Practitioner, delete after 3 years.', NULL, NULL, NULL, 1095)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (5, 5, N'Delete after 3 years from the Test Material Request being Finalised.', NULL, NULL, NULL, 1095)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (6, 6, N'Delete after 6 months from Application Completed.', NULL, NULL, NULL, 183)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (7, 7, N'Delete 5 years after the document was uploaded.', NULL, NULL, NULL, 1825)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (8, 8, N'Delete after 4 years from the test result issued', NULL, NULL, NULL, 1460)
GO
INSERT [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId], [PolicyExecutionOrder], [PolicyDescription], [ModifiedByNaati], [ModifiedDate], [ModifiedUser], [DaysToKeep]) VALUES (9, 9, N'Delete after 1 year of the recertification application completed', NULL, NULL, NULL, 365)
GO
SET IDENTITY_INSERT [dbo].[tblStoredFileDeletePolicy] OFF
GO
SET IDENTITY_INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ON 
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (1, 25, 6, N'Application documents', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (2, 26, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (4, 21, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (5, 13, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (6, 43, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (8, 40, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (9, 33, NULL, N'Test specification document
', N'tblTestSpecificationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (10, 34, NULL, N'Test specification document
', N'tblTestSpecificationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (11, 30, NULL, N'Test material document
', N'tblTestMaterialAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (12, 30, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (13, 18, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (14, 46, NULL, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (15, 4, 2, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (16, 14, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (17, 15, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (18, 31, NULL, N'Test material document
', N'tblTestMaterialAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (19, 31, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (20, 39, 7, N'Person documents
', N'tblPersonAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (21, 39, 7, N'Note attachment
', N'tblNoteAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (22, 39, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (23, 8, 3, N'Note attachment
', N'tblNoteAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (24, 8, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (25, 8, 6, N'Note attachment
', N'tblNoteAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (26, 7, 6, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (28, 45, NULL, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (29, 9, 6, N'Note attachment
', N'tblNoteAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (30, 9, 6, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (31, 3, 8, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (32, 37, 6, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (33, 38, 3, N'Person documents
', N'tblPersonAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (34, 38, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (35, 17, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (36, 36, 2, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (37, 28, 9, N'Professional development activity document
', N'tblProfessionalDevelopmentActivityAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (38, 28, 9, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (39, 32, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (40, 24, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (41, 44, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (42, 6, 2, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (43, 35, NULL, N'Test material document
', N'tblTestMaterialAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (44, 35, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (45, 42, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (46, 47, 5, N'Material request round document
', N'tblMaterialRequestRoundAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (47, 5, 2, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (48, 12, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (49, 11, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (50, 41, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (51, 2, 2, N'Test documents
', N'tblTestSittingDocument
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (52, 27, 3, N'Work practice activity document
', N'tblWorkPracticeAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (53, 27, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (54, 27, 3, N'Work practice activity document
', N'tblWorkPracticeAttachment
')
GO
INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] ([StoredFileDeletePolicyDocumentTypeId], [DocumentTypeId], [StoredFileDeletePolicyId], [Description], [EntityType]) VALUES (55, 10, 1, N'Application documents
', N'tblCredentialApplicationAttachment
')
GO
SET IDENTITY_INSERT [dbo].[tblStoredFileDeletePolicyDocumentType] OFF
GO
ALTER TABLE [dbo].[tblStoredFileDeletePolicyDocumentType]  WITH CHECK ADD  CONSTRAINT [FK_tblStoredFileDeletePolicyDocumentType_tblStoredFileDeletePolicy] FOREIGN KEY([StoredFileDeletePolicyId])
REFERENCES [dbo].[tblStoredFileDeletePolicy] ([StoredFileDeletePolicyId])
GO
ALTER TABLE [dbo].[tblStoredFileDeletePolicyDocumentType] CHECK CONSTRAINT [FK_tblStoredFileDeletePolicyDocumentType_tblStoredFileDeletePolicy]
GO
ALTER TABLE [dbo].[tblStoredFileDeletePolicyDocumentType]  WITH CHECK ADD  CONSTRAINT [FK_tblStoredFileDeletePolicyDocumentType_tluDocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[tluDocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[tblStoredFileDeletePolicyDocumentType] CHECK CONSTRAINT [FK_tblStoredFileDeletePolicyDocumentType_tluDocumentType]
GO
