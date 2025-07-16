USE [$(ncmsDbName)]

--Endorsed Qualification 
--insert into tblEntity
declare @latestNaatiNumber int
select @latestNaatiNumber = NextKey from tblTableData where TableName = 'InstitutionNaatiNumber'
select @latestNaatiNumber 


set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.abbeycollege.com.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.albrightinstitute.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.idealcollege.com.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://aiwt.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.charter.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.deafservices.org.au/accreditedtraining' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://deafsocietynsw.org.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.empire.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.frontierleadership.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://gbca.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.mq.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.monash.edu/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.mait.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.rmit.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://stanleycollege.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.siit.nsw.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)
	 
set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.tafensw.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.tafesa.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.adelaide.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.unimelb.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)
	 
set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://hal.arts.unsw.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.uq.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://study.uwa.edu.au/courses/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('http://www.win.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

set @latestNaatiNumber = @latestNaatiNumber + 1
INSERT INTO tblEntity ([WebsiteURL],[ABN],[Note],[UseEmail],[WebsiteInPD],[GSTApplies],[NAATINumber],[EntityTypeId] ,[AccountNumber]) VALUES ('https://www.westernsydney.edu.au/' ,'' ,'', 0, 0, 1, @latestNaatiNumber, 3, null)

select Max(NaatiNumber) as MaxNaatiNumber from tblEntity

--insert into tblInstitution
declare @latestEntityId int
select @latestEntityId = Max(EntityId) from tblEntity
select @latestEntityId as MaxEntityId
set  @latestEntityId = @latestEntityId - 25
select @latestEntityId as StartEntityId

--starting point
set @latestEntityId = @latestEntityId
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1	 
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)
	 
set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

set @latestEntityId = @latestEntityId + 1
INSERT INTO tblInstitution ([EntityId],[IsManageCoursesAndQualification],[IsGoThroughApprovalProcess] ,[IsUniversity] ,[IsVetRto] ,[RtoNumber] ,[IsOfferCourseToStudentVisa] ,[CricosProviderCode],[TrustedPayer])  VALUES  (@latestEntityId, 1, 1, 0, 0, '', 0, '', 0)

--insert into tblInstitution
declare @latestInstitutionId int
select @latestInstitutionId = Max(InstitutionId) from tblInstitution
select @latestInstitutionId as MaxInstitutionId
set  @latestInstitutionId = @latestInstitutionId - 25
select @latestInstitutionId as StartInstitutionId

set @latestInstitutionId = @latestInstitutionId
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Abbey College', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Albright Institute of Business and Language', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Australian College of Interpreting and Translation', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Australian Ideal College', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Australia-International Institute of Workplace Training', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Charter Australia', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Deaf Services QLD - Access Training and Education', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Deaf Society of NSW', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Empire Institute of Education', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Frontier Education', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Global Business College of Australia', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Macquarie University', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Monash University', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Multilink Academy of Interpreting and Translating (MAIT)', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'RMIT University', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Stanley College', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Sydney Institute of Interpreting and Translating (SIIT)', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'TAFE NSW', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'TAFE SA', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'University of Adelaide', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'University of Melbourne', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'University of New South Wales (UNSW)', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'University of Queensland', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'University of Western Australia (UWA)', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Wentworth Institute', '', GETDate(), '')

set @latestInstitutionId = @latestInstitutionId + 1
INSERT INTO tblInstitutionName  ([InstitutionId], [Name], [Abbreviation], [EffectiveDate], [TradingName]) VALUES  (@latestInstitutionId,'Western Sydney University', '', GETDate(), '')

IF NOT EXISTS (SELECT TOP 1 * FROM tblEndorsedQualification)
	BEGIN

	-- insert into tblEndorsedQualification
	select @latestInstitutionId = Max(InstitutionId) from tblInstitution
	select @latestInstitutionId as MaxInstitutionId
	set @latestInstitutionId = @latestInstitutionId - 25
	select @latestInstitutionId as StartInstitutionId

	--Abbey College	
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           

	--Albright Institute of Business and Language
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)           

	--Australian College of Interpreting and Translation
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-02-26', '2020-12-31', '', 1)           

	--Australian Ideal College
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)     
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)     
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)     
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)     
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)     
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)     

	--Australia-International Institute of Workplace Training	 
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-02-26', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Diploma of Interpreting (PSP50916)', 6, '2018-02-26', '2020-12-31', '', 1)       

	--Charter Australia
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)           

	--Deaf Services QLD - Access Training and Education
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-22', '2020-12-31', '', 1)           

	--Deaf Society of NSW
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           

	--Empire Institute of Education
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)           

	--Frontier Education	 
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Diploma of Interpreting (PSP50916)', 6, '2018-02-20', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-02-20', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Diploma of Interpreting (PSP50916)', 6, '2018-02-20', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Hobart, TAS', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-02-20', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Cairns, QLD', 'Diploma of Interpreting (PSP50916)', 6, '2018-02-20', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Cairns, QLD', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-02-20', '2020-12-31', '', 1)           

	--Global Business College of Australia
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)   

	--Macquarie University
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma of Translating and Interpreting', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma of Translating and Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma of Auslan-English Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies (Auslan pathway)', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies (Auslan pathway)', 9, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies (Auslan pathway)', 8, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies (Spoken languages pathway)', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies (Spoken languages pathway)', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Advanced Translation and Interpreting Studies', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Advanced Translation and Interpreting Studies', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Advanced Translation and Interpreting Studies', 3, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Advanced Translation and Interpreting Studies', 9, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Advanced Translation and Interpreting Studies', 8, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies with Master of Applied Linguistics and TESOL', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies with Master of Applied Linguistics and TESOL', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies with Master of International Relations', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting Studies with Master of International Relations', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Conference Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Conference Interpreting', 2, '2018-08-16', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Conference Interpreting', 10, '2018-01-01', '2020-12-31', '', 1)           

	--Monash University
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (MITS)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (MITS)', 3, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (MITS)', 9, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (MITS)', 8, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (MITS)', 10, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - Jean Moulin University Lyon III, France', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - Kobe City University of Foreign Studies, Japan', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - Kobe City University of Foreign Studies, Japan', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - Kobe City University of Foreign Studies, Japan', 10, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - University of Trieste, Italy', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - University of Trieste, Italy', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Interpreting and Translation Studies (International Double Major) - University of Trieste, Italy', 10, '2018-01-01', '2020-12-31', '', 1)    

	--Multilink Academy of Interpreting and Translating (MAIT)
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)       

	--RMIT University
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Graduate Diploma in Translating and Interpreting (GD168)', 2, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Graduate Diploma in Translating and Interpreting (GD168)', 7, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Graduate Diploma in Translating and Interpreting (GD168)', 3, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Graduate Diploma in Translating and Interpreting (GD168)', 9, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Graduate Diploma in Translating and Interpreting (GD168)', 8, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translating and Interpreting (MC214)', 2, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translating and Interpreting (MC214)', 7, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translating and Interpreting (MC214)', 3, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translating and Interpreting (MC214)', 9, '2018-01-01', '2020-12-31', '', 1)   
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translating and Interpreting (MC214)', 8, '2018-01-01', '2020-12-31', '', 1)   

	--Stanley College
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)
           
	--Sydney Institute of Interpreting and Translating (SIIT)
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    

	--TAFE NSW
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Meadowbank, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Bankstown, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Granville, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    

	--TAFE SA
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    

	--University of Adelaide
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Graduate Diploma in Interpreting, Translation and Transcultural Communication', 7, '2018-02-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Graduate Diploma in Interpreting, Translation and Transcultural Communication', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Master of Arts (Interpreting, Translation and Transcultural Communication)', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Adelaide, SA', 'Master of Arts (Interpreting, Translation and Transcultural Communication)', 2, '2018-01-01', '2020-12-31', '', 1)    

	--University of Melbourne
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translation', 2, '2018-01-01', '2020-12-31', '', 1) 	 
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Master of Translation (Enhanced)', 2, '2018-01-01', '2020-12-31', '', 1) 	 

	--University of New South Wales (UNSW)
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 3, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 9, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 8, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and Interpreting', 10, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Interpreting', 9, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Interpreting', 8, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Interpreting', 10, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Translation', 2, '2018-01-01', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'NSW	Master of Translation', 3, '2018-01-01', '2020-12-31', '', 1)           

	--University of Queensland
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Japanese Interpreting and Translation (MAJIT)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Japanese Interpreting and Translation (MAJIT)', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Japanese Interpreting and Translation (MAJIT)', 3, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Japanese Interpreting and Translation (MAJIT)', 10, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Chinese Translation and Interpreting', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Chinese Translation and Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Chinese Translation and Interpreting', 8, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Master of Arts in Chinese Translation and Interpreting', 9, '2018-01-01', '2020-12-31', '', 1)    

	--University of Western Australia (UWA)
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Master of Translation Studies', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Master of Translation Studies', 3, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Perth, WA', 'Graduate Diploma of Translation Studies', 2, '2018-01-01', '2020-12-31', '', 1)    

	--Wentworth Institute
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    

	--Western Sydney University
	set @latestInstitutionId = @latestInstitutionId + 1
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation)', 7, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation)', 2, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation)', 8, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation)', 9, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation) Dean''s Scholars', 7, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation) Dean''s Scholars', 2, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation) Dean''s Scholars', 8, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts (Interpreting and Translation) Dean''s Scholars', 9, '2018-01-01', '2020-12-31', '', 1)  

	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Translation and TESOL', 2, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Interpreting and Translation', 7, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Interpreting and Translation', 2, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Interpreting and Translation', 8, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Master of Interpreting and Translation', 9, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma in Interpreting', 7, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma in Interpreting', 8, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma in Interpreting', 9, '2018-01-01', '2020-12-31', '', 1)  
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Graduate Diploma in Translation', 2, '2018-01-01', '2020-12-31', '', 1)  

	--Manually added
	--Australian National University (ANU)	
	DECLARE @P_EntityId INT
	DECLARE  @P_InstitutionId INT

	select @P_EntityId = EntityId from tblEntity where NAATINumber in (950014)
	select @P_InstitutionId = InstitutionId from tblInstitution where EntityId = @P_EntityId
	update tblEntity set WebsiteURL = 'http://programsandcourses.anu.edu.au/' where EntityId = @P_EntityId
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@P_InstitutionId , 'Canberra, ACT', 'Master of Translation', 2, '2018-02-23', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@P_InstitutionId , 'Canberra, ACT', 'Master of Translation (Advanced)', 2, '2018-02-23', '2020-12-31', '', 1)           
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@P_InstitutionId , 'Canberra, ACT', 'Master of Translation (Advanced)', 6, '2018-02-23', '2020-12-31', '', 1)           

	--Babel International College
	select @P_EntityId = EntityId from tblEntity where NAATINumber in (940725)
	select @P_InstitutionId = InstitutionId from tblInstitution where EntityId = @P_EntityId
	update tblEntity set WebsiteURL = 'http://www.bic.wa.edu.au/' where EntityId = @P_EntityId
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@P_InstitutionId , 'Perth, WA', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@P_InstitutionId , 'Perth, WA', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-06-08', '2021-08-05', '', 1)    

	--Newton College
	select @P_EntityId = EntityId from tblEntity where NAATINumber in (940818)
	select @P_InstitutionId = InstitutionId from tblInstitution where EntityId = @P_EntityId
	update tblEntity set WebsiteURL = 'http://www.newton.edu.au/' where EntityId = @P_EntityId
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-03-01', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Melbourne, VIC', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Advanced Diploma of Translating (PSP60816)', 2, '2018-01-01', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Advanced Diploma of Interpreting (PSP60916)', 7, '2018-01-01', '2020-12-31', '', 1)       
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Brisbane, QLD', 'Diploma of Interpreting (PSP50916)', 6, '2018-01-01', '2020-12-31', '', 1)       

	--University of Auckland
	select @P_EntityId = EntityId from tblEntity where NAATINumber in (911565)
	select @P_InstitutionId = InstitutionId from tblInstitution where EntityId = @P_EntityId
	update tblEntity set WebsiteURL = 'https://www.auckland.ac.nz' where EntityId = @P_EntityId
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Auckland, New Zealand', 'Postgraduate Diploma in Translation Studies (Level 8)', 2, '2018-02-23', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Auckland, New Zealand', 'Master of Translation (Level 9) Certified Translator', 2, '2018-01-22', '2020-12-31', '', 1)    
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Auckland, New Zealand', 'Master of Translation (Level 9) Certified Advanced Translator', 3, '2018-01-22', '2020-12-31', '', 1)    

	--University of Sydney
	select @P_EntityId = EntityId from tblEntity where NAATINumber in (950019)
	select @P_InstitutionId = InstitutionId from tblInstitution where EntityId = @P_EntityId
	update tblEntity set WebsiteURL = 'https://sydney.edu.au/' where EntityId = @P_EntityId
	INSERT INTO tblEndorsedQualification ([InstitutionId], [Location], [Qualification], [CredentialTypeId], [EndorsementPeriodFrom], [EndorsementPeriodTo], [Notes], [Active]) VALUES  (@latestInstitutionId , 'Sydney, NSW', 'Bachelor of Arts/Bachelor of Advanced Studies (Languages)', 2, '2018-01-01', '2020-12-31', '', 1)    


	--select * from tblEndorsedQualification

	-- update tblTableData with NaatiNumber NextKey
	select @latestNaatiNumber = Max(NAATINumber) from tblEntity
	update tblTableData set NextKey = @latestNaatiNumber where TableName  = 'InstitutionNaatiNumber'	 

END
--Add role for F1\josh.yip
declare @userId int
select @userId = UserId from tblUser where USERNAME = 'F1\josh.yip'

IF NOT EXISTS (select RoleId from tblUserRole where UserId = @userId and RoleId = 12) 
BEGIN
   INSERT INTO [dbo].[tblUserRole] ([UserId],[RoleId])  VALUES (@userId ,12)
END


INSERT INTO tblApiAccess 
(InstitutionId, CreatedDate, ModifiedDate, MODIFIEDUSERID, INACTIVE, PublicKey,PrivateKey,Permissions)
VALUES( 1303, GETDATE(), GETDATE(), 40, 0, 'T3T14130231723C23183723040C2368723714130231723C23183723040C23T3T',
'T3TCC102348410301371418C10E4CC4234ECC102348410301371418C10E4CT3T', 32767)
