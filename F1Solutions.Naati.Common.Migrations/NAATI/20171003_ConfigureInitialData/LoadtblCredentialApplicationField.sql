SET IDENTITY_INSERT [dbo].[tblCredentialApplicationField] ON 

INSERT [dbo].[tblCredentialApplicationField] 
       ([CredentialApplicationFieldId], [CredentialApplicationTypeId], [Reference], [Name], [Section], [DataTypeId], [DefaultValue], [PerCredentialRequest], [Description], [Mandatory], [DisplayOrder]) 
VALUES (1, 1, NULL, N'Transition All Credentials',N'Credentials (mandatory)', 1, NULL, 0,N'Does the applicant wish to transition all of the current credentials?', 1, NULL),
       (2, 1, NULL, N'All Credentials Have Expiry Dates',N'Credentials (mandatory)', 1, NULL, 0,N'Do all of the credentials have an expiry date?', 1, NULL),
       (3, 1, NULL, N'Products Claimed',N'Products and Online Directory (mandatory)', 1, NULL, 0,N'Is the applicant claiming all products that they are entitled to?', 1, NULL),
       (4, 1, NULL, N'List in Online Directory',N'Products and Online Directory (mandatory)', 1, NULL, 0,N'Does the applicant wish to be listed on the online directory?', 1, NULL),
       (5, 1, NULL, N'Reference Letter Provided',N'Work Practice Evidence (for credentials without an expiry)', 1, NULL, 0,N'Has the applicant provided a Reference letter from an employer or service provider?', 0, NULL),
       (6, 1, NULL, N'Work Practice Record Provided',N'Work Practice Evidence (for credentials without an expiry)', 1, NULL, 0,N'Has the applicant provided a completed work practice record?', 0, NULL),
       (7, 1, NULL, N'Proof Of Income Provided',N'Work Practice Evidence (for credentials without an expiry)', 1, NULL, 0,N'Has the applicant provided Proof of Income from an accountant?', 0, NULL),
       (8, 1, NULL, N'Statement Provided',N'Work Practice Evidence (for credentials without an expiry)', 1, NULL, 0,N'If the applicant did not provide any work practice evidence, did they supply a brief statement abou why this was not attached?', 0, NULL),
       (9, 1, NULL, N'Reference Letter Provided',N'Chuchotage (for Professional Interpreters only)', 1, NULL, 0,N'Has the applicant provided a Reference letter from an employer or service provider for evidence of chuchotage?', 0, NULL),
       (10, 1, NULL, N'Evidence of Training/Study Provided',N'Chuchotage (for Professional Interpreters only)', 1, NULL, 0,N'Has the applicant provided evidence of training/formal unit of study (as part of a qualification)?', 0, NULL),
       (11, 1, NULL, N'Evidence of Completed PD Provided',N'Chuchotage (for Professional Interpreters only)', 1, NULL, 0,N'Has the applicant provided evidence of a completed professional development session?', 0, NULL),
       (12, 1, NULL, N'Award Certified Provisional Interpreter',N'Chuchotage (for Professional Interpreters only)', 1, NULL, 0,N'Does the applicant wish to be awarded Certified Provisional Interpreter during the interim period (until Professional Development evidence is received by NAATI)?', 0, NULL)

SET IDENTITY_INSERT [dbo].[tblCredentialApplicationField] OFF 