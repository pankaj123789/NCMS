
CREATE NONCLUSTERED INDEX [CredentialApplicationPerson_Idx] ON [dbo].[tblCredentialApplication]
(
	[CredentialApplicationId] ASC,
	[PersonId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


CREATE STATISTICS PersonCredentialApplication_Stats ON [dbo].[tblCredentialApplication]([PersonId], [CredentialApplicationId])
