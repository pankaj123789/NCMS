
namespace F1Solutions.Naati.Common.Migrations.NAATI._20190911_ConfigureRubricCalculationForCSHI
{
    [NaatiMigration(201909111300)]
    public class ConfigureRubricCalculationForCSHI: NaatiMigration
    {
        public override void Up()
        {
            Execute.Sql(@"
                    SET IDENTITY_INSERT [dbo].[tblSkillType] ON 
                    INSERT INTO [dbo].[tblSkillType] ([SkillTypeId],[Name],[DisplayName],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (31,'CSLI-Knowledge','Specialist Legal Interpreter Knowledge Test', 0,GETDATE(),40)
                    INSERT INTO [dbo].[tblSkillType] ([SkillTypeId],[Name],[DisplayName],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (32,'CSHI-Knowledge','Specialist Health Interpreter Knowledge Test', 0,GETDATE(),40)
                    SET IDENTITY_INSERT [dbo].[tblSkillType] OFF

            ");
            Execute.Sql(@"
                    SET IDENTITY_INSERT [dbo].[tblCredentialCategory] ON 
                    INSERT INTO [dbo].[tblCredentialCategory] ( [CredentialCategoryId],[Name],[DisplayName],[WorkPracticePoints],[WorkPracticeUnits],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES (9,'CSLI Knowledge Test','Specialist Legal Interpreter Knowledge Test',  null, null, 0,GETDATE(),40)            
                    INSERT INTO [dbo].[tblCredentialCategory] ( [CredentialCategoryId],[Name],[DisplayName],[WorkPracticePoints],[WorkPracticeUnits],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES (10,'CSHI Knowledge Test','Specialist Health Interpreter Knowledge Test',  null, null, 0,GETDATE(),40)            
                    SET IDENTITY_INSERT [dbo].[tblCredentialCategory] OFF
            ");
            Execute.Sql(@"
                         SET IDENTITY_INSERT [dbo].[tblCredentialType] ON 
	                     INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[DisplayOrder],[Simultaneous],[SkillTypeId], [Certification],[DefaultExpiry],[AllowBackdating],[Level],[TestSessionBookingAvailabilityWeeks],[TestSessionBookingClosedWeeks],[TestSessionBookingRejectWeeks]   ,[ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                         VALUES (31,9,'CSLI - Knowledge','Specialist Legal Interpreter Knowledge Test', 31,0,31, 0,3,0,0, 52,5,5, 0,GETDATE(),40)

	                     INSERT INTO [dbo].[tblCredentialType] ([CredentialTypeId],[CredentialCategoryId],[InternalName],[ExternalName],[DisplayOrder],[Simultaneous],[SkillTypeId], [Certification],[DefaultExpiry],[AllowBackdating],[Level],[TestSessionBookingAvailabilityWeeks],[TestSessionBookingClosedWeeks],[TestSessionBookingRejectWeeks]   ,[ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                         VALUES (32,10,'CSHI - Knowledge','Specialist Health Interpreter Knowledge Test', 32,0,32, 0,3,0,0, 52,5,5, 0,GETDATE(),40)         
                    SET IDENTITY_INSERT [dbo].[tblCredentialType] OFF
            ");

            Execute.Sql(@"
                    SET IDENTITY_INSERT [dbo].[tblTestSpecification] ON 
	                    INSERT INTO [dbo].[tblTestSpecification] ([TestSpecificationId],[Description],[CredentialTypeId],[Active],[ResultAutoCalculation],[AutomaticIssuing],[MaxScoreDifference],  [ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES(19, 'Certified Provisional Deaf Interpreter', 13, 1,0,0,0,  0,GETDATE(),40)
	                    INSERT INTO [dbo].[tblTestSpecification] ([TestSpecificationId],[Description],[CredentialTypeId],[Active],[ResultAutoCalculation],[AutomaticIssuing],[MaxScoreDifference],  [ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES(20, 'CSLI - Knowledge Test', 31, 1,0,0,0,  0,GETDATE(),40)
	                    INSERT INTO [dbo].[tblTestSpecification] ([TestSpecificationId],[Description],[CredentialTypeId],[Active],[ResultAutoCalculation],[AutomaticIssuing],[MaxScoreDifference],  [ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES(21, 'CSLI - Interpreting Tasks', 9, 1,1,0,0,  0,GETDATE(),40)
	                    INSERT INTO [dbo].[tblTestSpecification] ([TestSpecificationId],[Description],[CredentialTypeId],[Active],[ResultAutoCalculation],[AutomaticIssuing],[MaxScoreDifference],  [ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES(22, 'CSHI - Knowledge Test', 32, 1,0,0,0,  0,GETDATE(),40)
	                    INSERT INTO [dbo].[tblTestSpecification] ([TestSpecificationId],[Description],[CredentialTypeId],[Active],[ResultAutoCalculation],[AutomaticIssuing],[MaxScoreDifference],  [ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES(23, 'CSHI - Interpreting Tasks', 8, 1,1,0,0,  0,GETDATE(),40)
                    SET IDENTITY_INSERT [dbo].[tblTestSpecification] OFF
            ");

            Execute.Sql(@"           

                     SET IDENTITY_INSERT [dbo].[tblTestComponentType] ON 
	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (42,'CPDI Task Type A','Interpreting - Face to face dialogue', 2, 'A', 19, 10, 0, 1, 0, 0, 5, 28,  0, GETDATE(), 40)


	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (43,'CPDI Task Type B','Sight Translation English into NCSL', 2, 'B', 19, 10, 0, 0, 0, 0, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (44,'CPDI Task Type C','Sight Translation English into Auslan', 2, 'C', 19, 10, 0, 0, 0, 0, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (45,'CSLI Knowledge Section 1','Legal terminology and General Legal Knowledge', 1, '1', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (46,'CSLI Knowledge Section 2','Knowledge of legal systems and processes', 1, '2', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (47,'CSLI Knowledge Section 3','Ethics and law', 1, '3', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (48,'CSLI Knowledge Section 4','Culture and law', 1, '4', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (49,'CSLI Knowledge Section 5','The role of the interpreter in the legal context', 1, '5', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (50,'CSLI Knowledge Section 6','Advanced interactional management', 1, '6', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (51,'CSLI Knowledge Section 7','Research and preparation', 1, '7', 20, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (52,'CSLI Task Type B','Consecutive Interpreting – Dialogic Extracts in both directions', 2, 'B', 21, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (53,'CSLI Task Type C','Consecutive Interpreting – Monologue into LOTE', 2, 'C', 21, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (54,'CSLI Task Type D','Consecutive Interpreting – Monologue into English', 2, 'D', 21, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (55,'CSLI Task Type E','Simultaneous Interpreting – Chuchotage into LOTE', 2, 'E', 21, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (56,'CSHI Knowledge Section 1','Medical terminology', 1, '1', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (57,'CSHI Knowledge Section 2','General Medical Knowledge', 1, '2', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (58,'CSHI Knowledge Section 3','Knowledge of health systems', 1, '3', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (59,'CSHI Knowledge Section 4','Ethics, culture, and the role of the interpreter', 1, '4', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (60,'CSHI Knowledge Section 5','Advanced interactional management', 1, '5', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (61,'CSHI Knowledge Section 6','Research and preparation', 1, '6', 22, 0, 0, 0, 0, 0, 0, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (62,'CSHI Task Type B','Consecutive Interpreting – Monologue into LOTE', 2, 'B', 23, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (63,'CSHI Task Type C','Consecutive Interpreting – Monologue into English', 2, 'C', 23, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (64,'CSHI Task Type D','Simultaneous Interpreting – Chuchotage into LOTE', 2, 'D', 23, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)

	                 INSERT INTO [dbo].[tblTestComponentType]  ([TestComponentTypeId],[Name],[Description],
										                   [TestComponentBaseTypeId],[Label],[TestSpecificationId],
										                   [MinExaminerCommentLength],[MinNaatiCommentLength],[RoleplayersRequired],[CandidateBriefRequired],[CandidateBriefAvailabilityDays],[DefaultMaterialRequestHours],[DefaultMaterialRequestDueDays],
										                   [ModifiedByNaati],[ModifiedDate],[ModifiedUser])
                     VALUES (65,'CSHI Task Type E','Simultaneous Interpreting – Monologue into English', 2, 'E', 23, 10, 0, 0, 1, 7, 5, 28,  0, GETDATE(), 40)
                           
	                SET IDENTITY_INSERT [dbo].[tblTestComponentType] OFF

                    ");


            Execute.Sql(@"       
            SET IDENTITY_INSERT [dbo].[tblRubricMarkingCompetency] ON 
			INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES (57,52,'Transfer Competency','TC', 1,  0,GETDATE(),40)           		   
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (58,52, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (59,53, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (60,53, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (61,53, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (62,54, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (63,54, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (64,54, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (65,55, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (66,55, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (67,55, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (68,62, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (69,62, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (70,62, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (71,63, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (72,63, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (73,63, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (74,64, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (75,64, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (76,64, 'Thematic Competency', 'THC', 3, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (77,65, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (78,65, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (79,42, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (80,42, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (81,43, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (82,43, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (83,44, 'Transfer Competency', 'TC', 1, 0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingCompetency] ([RubricMarkingCompetencyId],[TestComponentTypeId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) Values (84,44, 'Language Competency', 'LC', 2, 0,GETDATE(),40)
            SET IDENTITY_INSERT [dbo].[tblRubricMarkingCompetency] OFF
            ");

            Execute.Sql(@"      
            SET IDENTITY_INSERT [dbo].[tblRubricMarkingAssessmentCriterion] ON 
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (130,57,'Meaning Transfer Skill','A',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (131,57,'Rhetorical skill','B',2,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (132,58,'Language proficiency enabling meaning transfer: English','C',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (133,58,'Language proficiency enabling meaning transfer: LOTE','D',2,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (134,59,'Meaning Transfer Skill','A',1,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (135,59,'Application of interpreting mode','B',2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (136,59,'Rhetorical skill','C',3,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (137,60,'Language proficiency enabling meaning transfer: Target Language','D',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (138,61,'Subject matter specific knowledge','E',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (139,62,'Meaning Transfer Skill','A',1,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (140,62,'Application of interpreting mode','B',2,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (141,62,'Rhetorical skill','C',3,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (142,63,'Language proficiency enabling meaning transfer: Target Language','D',1,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (143,64,'Subject matter specific knowledge','E',1,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (144,65,'Meaning Transfer Skill','A',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (145,65,'Application of interpreting mode','B',2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (146,65,'Rhetorical skill','C',3,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (147,66,'Language proficiency enabling meaning transfer: Target Language','D',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (148,67,'Subject matter specific knowledge','E',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES   (149,68,'Meaning Transfer Skill','A',1,   0,GETDATE(),40)

            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (150,68,'Application of interpreting mode', 'B', 2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (151,68,'Rhetorical skill','C',3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (152,69,'Language proficiency enabling meaning transfer: Target Language', 'D',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (153,70, 'Subject matter specific knowledge','E',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (154,71, 'Meaning Transfer Skill', 'A', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (155,71, 'Application of interpreting mode', 'B', 2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (156,71, 'Rhetorical skill', 'C',3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (157,72, 'Language proficiency enabling meaning transfer: Target Language', 'D', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (158,73, 'Subject matter specific knowledge', 'E', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (159,74, 'Meaning Transfer Skill', 'A',1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (160,74, 'Application of interpreting mode', 'B', '2',   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (161,74, 'Rhetorical skill', 'C', 3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (162,75, 'Language proficiency enabling meaning transfer: Target Language', 'D', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (163,76, 'Subject matter specific knowledge', 'E', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (164,77, 'Meaning Transfer Skill', 'A', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (165,77, 'Application of interpreting mode', 'B', 2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (166,77, 'Rhetorical skill', 'C', 3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (167,78, 'Language proficiency enabling meaning transfer: Target Language', 'D', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (168,79, 'Meaning Transfer Skill', 'A', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (169,79, 'Application of interpreting mode', 'B',2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (170,79, 'Interactional management', 'C', 3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (171,79, 'Rhetorical skill', 'D', 4,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (172,80, 'Language proficiency enabling meaning transfer: Auslan', 'E', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (173,80, 'Language proficiency enabling meaning transfer: NCSL', 'F', 2 ,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (174,81, 'Meaning Transfer Skill', 'A', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (175,81, 'Application of interpreting mode', 'B', 2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (176,81, 'Rhetorical skill', 'C', 3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (177,82, 'Language proficiency enabling meaning transfer: NCSL', 'D', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (178,83, 'Meaning Transfer Skill', 'A', 1,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (179,83, 'Application of interpreting mode', 'B', 2,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (180,83, 'Rhetorical skill', 'C', 3,   0,GETDATE(),40)
            INSERT INTO [dbo].[tblRubricMarkingAssessmentCriterion] ([RubricMarkingAssessmentCriterionId],[RubricMarkingCompetencyId],[Name],[Label],[DisplayOrder],[ModifiedByNaati],[ModifiedDate],[ModifiedUser])  VALUES (181,84, 'Language proficiency enabling meaning transfer: Auslan', 'D', 1,   0,GETDATE(),40)
            SET IDENTITY_INSERT [dbo].[tblRubricMarkingAssessmentCriterion] OFF
            ");

            Execute.Sql(@"
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,62,149,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,62,150,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,62,151,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,62,152,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,62,153,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,63,154,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,63,155,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,63,156,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,63,157,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,63,158,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,64,159,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,64,160,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,64,161,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,64,162,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,64,163,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,65,164,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,65,165,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,65,166,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,65,167,2, null, 1, GETDATE(), 40)                                 


                    INSERT INTO [dbo].[tblRubricTestQuestionRule] ([TestSpecificationId],[TestResultEligibilityTypeId],[TestComponentTypeId],[MinimumQuestionsAttempted],[MinimumQuestionsPassed],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,1,null,4,4,null,1,GETDATE(),40)
                    INSERT INTO [dbo].[tblRubricTestQuestionRule] ([TestSpecificationId],[TestResultEligibilityTypeId],[TestComponentTypeId],[MinimumQuestionsAttempted],[MinimumQuestionsPassed],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (23,3,null,4,3,null,1,GETDATE(),40)


                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,52,130,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,52,131,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,52,132,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,52,133,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,53,134,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,53,135,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,53,136,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,53,137,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,53,138,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,54,139,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,54,140,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,54,141,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,54,142,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,54,143,2, null, 1, GETDATE(), 40)                                 

                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,55,144,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,55,145,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,55,146,2, null, 1, GETDATE(), 40)                                 
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,55,147,2, null, 1, GETDATE(), 40)       
                    INSERT INTO [dbo].[tblRubricQuestionPassRule] ([TestSpecificationId],[TestComponentTypeId],[RubricMarkingAssessmentCriterionId],[MaximumBandLevel],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,55,148,2, null, 1, GETDATE(), 40)  


                    INSERT INTO [dbo].[tblRubricTestQuestionRule] ([TestSpecificationId],[TestResultEligibilityTypeId],[TestComponentTypeId],[MinimumQuestionsAttempted],[MinimumQuestionsPassed],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,1,null,4,4,null,1,GETDATE(),40)
                    INSERT INTO [dbo].[tblRubricTestQuestionRule] ([TestSpecificationId],[TestResultEligibilityTypeId],[TestComponentTypeId],[MinimumQuestionsAttempted],[MinimumQuestionsPassed],[RuleGroup],[ModifiedByNaati],[ModifiedDate],[ModifiedUser]) VALUES  (21,3,null,4,3,null,1,GETDATE(),40)

GO
            ");
        }
    }
}
    