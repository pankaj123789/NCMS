SET IDENTITY_INSERT [dbo].[tblCredentialType] ON 

if not exists(select null from tblCredentialType where CredentialTypeId in (36))
begin
insert into tblCredentialType (CredentialTypeId,CredentialCategoryId,InternalName,ExternalName,DisplayOrder,Simultaneous,SkillTypeId,Certification,DefaultExpiry,
ModifiedByNaati,ModifiedDate,ModifiedUser,AllowBackdating,[Level],TestSessionBookingAvailabilityWeeks,TestSessionBookingClosedWeeks,TestSessionBookingRejectWeeks, AllowAvailabilityNotice)
values (36, 2, 'Cert. Conference Interpreter – English into Auslan', 'Cert. Conference Interpreter – English into Auslan', 36, 1, 22, 1, null, 0, GETDATE(), 40, 4, 0, 104, 5, 5, 1 )
      
end

if not exists(select null from tblCredentialType where CredentialTypeId in (37))
begin
insert into tblCredentialType (CredentialTypeId,CredentialCategoryId,InternalName,ExternalName,DisplayOrder,Simultaneous,SkillTypeId,Certification,DefaultExpiry,
ModifiedByNaati,ModifiedDate,ModifiedUser,AllowBackdating,[Level],TestSessionBookingAvailabilityWeeks,TestSessionBookingClosedWeeks,TestSessionBookingRejectWeeks, AllowAvailabilityNotice)
values (37, 2, 'Cert. Conference Interpreter – Auslan into English', 'Cert. Conference Interpreter – Auslan into English', 37, 1, 22, 1, null, 0, GETDATE(), 40, 4, 0, 104, 5, 5, 1)
end

SET IDENTITY_INSERT [dbo].[tblCredentialType] OFF 

insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (36, 52477, '[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Letter', 0, GETDATE(), 40)
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (36, 14896, '[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Certificate', 0, GETDATE(), 40)

insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (37, 52477, '[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Letter', 0, GETDATE(), 40)
insert into tblCredentialTypeTemplate (CredentialTypeId, StoredFileId, DocumentNameTemplate, ModifiedByNaati, ModifiedDate, ModifiedUser)
values (37, 14896, '[[Practitioner Number]] [[Credential Type]] [[Skill]] [[Document Number]] Certificate', 0, GETDATE(), 40)