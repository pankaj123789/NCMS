
SET IDENTITY_INSERT [dbo].[tblCredentialApplicationForm] ON 

INSERT [dbo].[tblCredentialApplicationForm] ([CredentialApplicationFormId], [CredentialApplicationTypeId], [Name], [Description], [LoginRequired]) VALUES (1, 3, N'CCL Testing', N'CCL Application', 0)

INSERT [dbo].[tblCredentialApplicationForm] ([CredentialApplicationFormId], [CredentialApplicationTypeId], [Name], [Description], [LoginRequired]) VALUES (2, 2, N'Certification & Recognised Practising', N'Certification Application', 0)

SET IDENTITY_INSERT [dbo].[tblCredentialApplicationForm] OFF
