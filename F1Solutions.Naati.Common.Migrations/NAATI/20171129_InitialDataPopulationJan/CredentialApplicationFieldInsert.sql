SET IDENTITY_INSERT [dbo].[tblCredentialApplicationField] ON 
                                                                                                                                                                                                                                                    
INSERT [dbo].[tblCredentialApplicationField] ([CredentialApplicationFieldId], [CredentialApplicationTypeId], [Reference], [Name], [Section], [DataTypeId], [DefaultValue], [PerCredentialRequest], [Description], [Mandatory], [DisplayOrder], [Reportable]) VALUES (46, 2, NULL, N'Recognised Work Practice', N'Requirements', 1, NULL, 0, N'', 0, NULL,0)
INSERT [dbo].[tblCredentialApplicationField] ([CredentialApplicationFieldId], [CredentialApplicationTypeId], [Reference], [Name], [Section], [DataTypeId], [DefaultValue], [PerCredentialRequest], [Description], [Mandatory], [DisplayOrder], [Reportable]) VALUES (47, 2, NULL, N'Non Endorsed Advanced Qualification', N'Qualifications', 1, NULL, 0, N'', 0, NULL,1)

SET IDENTITY_INSERT [dbo].[tblCredentialApplicationField] OFF
