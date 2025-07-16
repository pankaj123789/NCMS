using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate.Criterion;
using NHibernate.Transform;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestSpecificationQueryService : ITestSpecificationQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly ITestSessionQueryService _testSessionQueryService;

        public TestSpecificationQueryService(IAutoMapperHelper autoMapperHelper, ITestSessionQueryService testSessionQueryService)
        {
            _autoMapperHelper = autoMapperHelper;
            _testSessionQueryService = testSessionQueryService;
        }

        public TestSpecificationResponse Get(TestSpecificationRequest request)
        {
            var testSpecification = NHibernateSession.Current.Query<TestSpecification>().ToList().Select(MapToTestSpecificationDto());
            if (request.Id > 0)
                testSpecification = testSpecification.Where(x => x.Id == request.Id);

            return new TestSpecificationResponse() { List = testSpecification.ToList() };
        }

        public TestSpecificationResponse GetTestSpecificationByCredentialTypeId(SpecificationByCredentialTypeRequest request)
        {
            var testSpecification = NHibernateSession.Current.Query<TestSpecification>().Where(x => x.CredentialType.Id == request.CredentialTypeId)
                .ToList().Select(MapToTestSpecificationDto());

            return new TestSpecificationResponse() { List = testSpecification.ToList() };
        }

        public TestSpecification GetTestSpecificationById(int testSpecificationId)
        {
            var testSpecification = NHibernateSession.Current.Get<TestSpecification>(testSpecificationId);
            return testSpecification;
        }

        public GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForTestSpecificationType()
        {
            var names =
                NHibernateSession.Current.Query<DocumentType>()
                    .Where(x => x.DocumentTypeCategory.Id == (int)DocumentTypeCategoryEnum.TestSpecification)
                    .Select(x => x.Name);

            return new GetDocumentTypesForApplicationTypeResponse
            {
                Results = names
            };
        }

        private static Func<TestSpecification, TestSpecificationDto> MapToTestSpecificationDto()
        {
            return x =>
                new TestSpecificationDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    CredentialTypeId = x.CredentialType.Id,
                    CredentialType = x.CredentialType.DisplayName,
                    Active = x.Active,
                    OverallPassMark = x.ActiveTestSpecificationStandardMarkingScheme?.OverallPassMark,
                    ResultAutoCalculation = x.ResultAutoCalculation,
                    IsRubric = x.TestComponentTypes.Any(y => y.RubricMarkingCompetencies.Any())
                };
        }

        public GenericResponse<string> Edit(TestSpecificationRequest request, GenericResponse<string> response)
        {
            if (request.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Id));

            var testSpecification = NHibernateSession.Current.Get<TestSpecification>(request.Id);

            if (testSpecification == null)
            {
                response.Errors.Add($"Test Specification not found for {request.Id}");
                return response;
            };

            var futureTestSessions = NHibernateSession.Current.Query<TestSession>().Where(x => (x.TestDateTime > DateTime.Now)
                                                                                          && (x.DefaultTestSpecification.Id == request.Id)
                                                                                          && (x.IsActive == true)).ToList();

            if (futureTestSessions.Count > 0 && testSpecification.Active)
            {
                response.Data = "Error has occured: Test Specification cannot be inactive as it has future active test sessions assigned.";
                response.Errors.Add("Error has occured: Test Specification can not be inactive as it has future active test sessions.");
                return response;
            }

            if (testSpecification.Active != request.Active || 
                testSpecification.Description != request.Description ||
                testSpecification.ResultAutoCalculation != request.ResultAutoCalculation)
            {
                testSpecification.Active = request.Active;
                testSpecification.Description = request.Description;
                testSpecification.ResultAutoCalculation = request.ResultAutoCalculation;
                testSpecification.ModifiedByNaati = true;
                testSpecification.ModifiedDate = DateTime.Now;
                testSpecification.ModifiedUser = NHibernateSession.Current.Get<User>(request.UserId);
                NHibernateSession.Current.Flush();
                NHibernateSession.Current.SaveOrUpdate(testSpecification);
            }
            response.Data = "Test Specification was updated successfully. No Errors Occured.";
            response.Messages.Add("Test Specification was updated successfully. No Errors Occured.");
            return response;
        }

        public TestSpecificationAttachmentResponse GetExaminerAttachmentsForSitting(int testSittingId)
        {
            var testSittings = NHibernateSession.Current.Query<TestSitting>().Where(x => x.Id == testSittingId).ToList();
            var ids = testSittings.GroupBy(x => x.TestSpecification.Id).Select(y => y.First().TestSpecification.Id);

            IEnumerable<TestSpecificationAttachmentDto> attachments = new List<TestSpecificationAttachmentDto>();

            foreach (var i in ids)
            {
                var attachmentGroup = GetAttachments(new TestSpecificationRequest { Id = i }).List.Where(x => x.ExaminerToolsDownload);
                attachments = attachments.Concat(attachmentGroup);
            }

            return new TestSpecificationAttachmentResponse() { List = attachments };
        }

        public TestSpecificationAttachmentResponse GetAttachments(TestSpecificationRequest request)
        {
            if (request.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Id));

            var attachments =
                NHibernateSession.Current.Query<TestSpecificationAttachment>()
                    .Where(x => x.TestSpecification.Id == request.Id)
                    .Select(x => new TestSpecificationAttachmentDto
                    {
                        TestSpecificationAttachmentId = x.Id,
                        TestSpecificationId = x.TestSpecification.Id,
                        StoredFileId = x.StoredFile.Id,
                        FileName = x.StoredFile.FileName,
                        Description = x.Title,
                        DocumentType = x.StoredFile.DocumentType.DisplayName,
                        UploadedByName = x.StoredFile.UploadedByUser.FullName,
                        UploadedByPersonName = x.StoredFile.UploadedByPerson != null ?
                            x.StoredFile.UploadedByPerson.GivenName + " " + x.StoredFile.UploadedByPerson.Surname : "",
                        UploadedDateTime = x.StoredFile.UploadedDateTime,
                        FileSize = x.StoredFile.FileSize,
                        Type = (StoredFileType)x.StoredFile.DocumentType.Id,
                        MergeDocument = x.MergeDocument,
                        ExaminerToolsDownload = x.ExaminerToolsDownload,
                        StoredFileStatusChangeDate = x.StoredFile.StoredFileStatusChangeDate,
                        StoredFileStatusTypeId = x.StoredFile.StoredFileStatusType.Id
                    }).ToList();

            return new TestSpecificationAttachmentResponse() { List = attachments };
        }

        public AttachmentResponse CreateOrReplaceAttachment(TestSpecificationAttachmentRequest request)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = request.StoredFileId != 0 ? (int?)request.StoredFileId : null,
                UpdateFileName = request.StoredFileId != 0 ? request.FileName : null,
                Type = request.Type,
                StoragePath = request.StoragePath,
                UploadedByUserId = request.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = request.FilePath,
            });

            var storedFile = NHibernateSession.Current.Load<StoredFile>(response.StoredFileId);
            var testSpecification = NHibernateSession.Current.Load<TestSpecification>(request.TestSpecificationId);

            if (storedFile == null)
            {
                throw new WebServiceException($"Stored File is not found (Stored File ID {response.StoredFileId})");
            }
            if (testSpecification == null)
            {
                throw new WebServiceException($"Test Specification is not found (Test Material ID {request.TestSpecificationId})");
            }

            TestSpecificationAttachment domain;
            if (request.Id > 0)
            {
                domain = NHibernateSession.Current.Get<TestSpecificationAttachment>(request.Id);
            }
            else
            {
                domain = new TestSpecificationAttachment();
            }

            domain.StoredFile = storedFile;
            domain.Title = request.Title;
            domain.Deleted = request.Deleted;
            domain.TestSpecification = testSpecification;
            domain.ExaminerToolsDownload = request.EportalDownload;
            domain.MergeDocument = request.MergeDocument;


            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                if (request.MergeDocument)
                {
                    var testSpecificationAttachments = NHibernateSession.Current.QueryOver<TestSpecificationAttachment>().Where(x => x.TestSpecification.Id == request.TestSpecificationId).List();

                    foreach (var testSpecificationAttachment in testSpecificationAttachments)
                    {
                        testSpecificationAttachment.MergeDocument = false;
                    }

                    foreach(var testSpecificationAttachment in testSpecificationAttachments)
                    {
                        NHibernateSession.Current.SaveOrUpdate(testSpecificationAttachment);
                    }
                }

                if (request.Id > 0)
                {
                    domain.MergeDocument = request.MergeDocument;
                }

                NHibernateSession.Current.SaveOrUpdate(domain);
                transaction.Commit();
            }

            return new AttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        public void DeleteAttachment(TestSpecificationAttachmentRequest request)
        {
            var attachment = NHibernateSession.Current.Query<TestSpecificationAttachment>().SingleOrDefault(n => n.StoredFile.Id == request.StoredFileId);
            NHibernateSession.Current.Delete(attachment);
            NHibernateSession.Current.Flush();

            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            fileService.DeleteFile(new DeleteFileRequest
            {
                StoredFileId = request.StoredFileId
            });
        }

        public TestComponentResponse GetTestComponentsBySpecificationId(TestSpecificationRequest request)
        {
            if (request.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Id));

            var testComponents =
                NHibernateSession.Current.Query<TestComponent>()
                    .Where(x => x.TestSpecification.Id == request.Id).ToList()
                    .Select(x => new TestSpecificationComponentDto
                    {
                        Id = x.Id,
                        ComponentNumber = x.ComponentNumber,
                        Label = x.Label,
                        TaskType = x.Type.Label + "-" + x.Type.Name,
                        TaskTypeDescription = x.Type.Description,
                        BasedOn = x.Type.TestComponentBaseType.DisplayName,
                        PassMark = x.Type.ActiveTestComponentTypeStandardMarkingScheme?.PassMark,
                        TotalMarks = x.Type.ActiveTestComponentTypeStandardMarkingScheme?.TotalMarks,
                        Name = x.Name,
                        MinNaatiCommentLength = x.Type.MinNaatiCommentLength,
                        MinExaminerCommentLength = x.Type.MinExaminerCommentLength,
                        RolePlayersRequired = x.Type.RoleplayersRequired
                    }).ToList();

            return new TestComponentResponse() { List = testComponents };
        }

        public bool CanUpload(TestSpecificationRequest request)
        {

            if (request.Id <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.Id));

            var testComponentTypes =
                NHibernateSession.Current.Query<TestComponentType>()
                    .Where(x => x.TestSpecification.Id == request.Id).ToList();

            if (testComponentTypes.Count > 0)
            {
                return false;
            }

            return true;
        }

        public GetQuestionPassRulesResponse GetQuestionPassRules(GetRubricConfigurationRequest request)
        {
            var response = new GetQuestionPassRulesResponse();

            var testSpecification = GetTestSpecificationAndFillConfigurations(request.TestSpecificationId, response);
            if (testSpecification == null)
            {
                return response;
            }

            response.Configurations = testSpecification.RubricQuestionPassRules.Select(_autoMapperHelper.Mapper.Map<RubricQuestionPassRuleDto>);

            return response;
        }

        public SaveRubricConfigurationResponse SaveQuestionPassRules(SaveQuestionPassRulesRequest request)
        {
            var response = new SaveRubricConfigurationResponse();
            if (request == null)
            {
                return response;
            }

            var session = NHibernateSession.Current;
            var testSpecification = session.Load<TestSpecification>(request.TestSpecificationId);
            if (testSpecification == null)
            {
                return response;
            }

            if (testSpecification.RubricQuestionPassRules != null && testSpecification.RubricQuestionPassRules.Any())
            {
                while (testSpecification.RubricQuestionPassRules.Any())
                {
                    var rqpr = testSpecification.RubricQuestionPassRules.First();
                    testSpecification.RemoveRubricQuestionPassRule(rqpr);
                    session.Delete(rqpr);
                }
            }

            if (request.Configurations == null || !request.Configurations.Any())
            {
                session.Flush();
                return response;
            }

            var user = session.Load<User>(request.UserId);
            foreach (var c in request.Configurations)
            {
                session.Save(new RubricQuestionPassRule
                {
                    TestSpecificationId = c.TestSpecificationId,
                    TestComponentTypeId = c.TestComponentTypeId,
                    RubricMarkingAssessmentCriterionId = c.RubricMarkingAssessmentCriterionId,
                    MaximumBandLevel = c.MaximumBandLevel,
                    RuleGroup = c.RuleGroup,
                    ModifiedUser = user,
                    ModifiedByNaati = true,
                    ModifiedDate = DateTime.Now
                });
            }

            session.Flush();

            return response;
        }

        public GetTestBandRulesResponse GetTestBandRules(GetRubricConfigurationRequest request)
        {
            var response = new GetTestBandRulesResponse();

            var testSpecification = GetTestSpecificationAndFillConfigurations(request.TestSpecificationId, response);
            if (testSpecification == null)
            {
                return response;
            }

            response.Configurations = testSpecification.RubricTestBandRules.Select(_autoMapperHelper.Mapper.Map<RubricTestBandRuleDto>);

            return response;
        }

        public SaveRubricConfigurationResponse SaveTestBandRules(SaveTestBandRulesRequest request)
        {
            var response = new SaveRubricConfigurationResponse();
            if (request == null)
            {
                return response;
            }

            var session = NHibernateSession.Current;
            var testSpecification = session.Load<TestSpecification>(request.TestSpecificationId);

            if (testSpecification == null)
            {
                return response;
            }

            if (testSpecification.RubricTestBandRules != null && testSpecification.RubricTestBandRules.Any())
            {
                while (testSpecification.RubricTestBandRules.Any())
                {
                    var rqpr = testSpecification.RubricTestBandRules.First();
                    testSpecification.RemoveRubricTestBandRule(rqpr);
                    session.Delete(rqpr);
                }
            }

            if (request.Configurations == null || !request.Configurations.Any())
            {
                session.Flush();
                return response;
            }

            var user = session.Load<User>(request.UserId);
            foreach (var c in request.Configurations)
            {
                var testResultEligibilityType = session.Load<TestResultEligibilityType>(c.TestResultEligibilityTypeId);
                session.Save(new RubricTestBandRule
                {
                    TestSpecificationId = c.TestSpecificationId,
                    TestComponentTypeId = c.TestComponentTypeId,
                    RubricMarkingAssessmentCriterionId = c.RubricMarkingAssessmentCriterionId,
                    MaximumBandLevel = c.MaximumBandLevel,
                    TestResultEligibilityType = testResultEligibilityType,
                    NumberOfQuestions = c.NumberOfQuestions,
                    RuleGroup = c.RuleGroup,
                    ModifiedUser = user,
                    ModifiedByNaati = true,
                    ModifiedDate = DateTime.Now
                });
            }

            session.Flush();

            return response;
        }

        public GetTestQuestionRulesResponse GetTestQuestionRules(GetRubricConfigurationRequest request)
        {
            var response = new GetTestQuestionRulesResponse();

            var testSpecification = GetTestSpecificationAndFillConfigurations(request.TestSpecificationId, response);
            if (testSpecification == null)
            {
                return response;
            }

            response.Configurations = testSpecification.RubricTestQuestionRules.Select(_autoMapperHelper.Mapper.Map<RubricTestQuestionRuleDto>);

            return response;
        }

        public SaveRubricConfigurationResponse SaveTestQuestionRules(SaveTestQuestionRulesRequest request)
        {
            var response = new SaveRubricConfigurationResponse();
            if (request == null)
            {
                return response;
            }

            var session = NHibernateSession.Current;
            var testSpecification = session.Load<TestSpecification>(request.TestSpecificationId);
            if (testSpecification == null)
            {
                return response;
            }

            if (testSpecification.RubricTestQuestionRules != null && testSpecification.RubricTestQuestionRules.Any())
            {
                while (testSpecification.RubricTestQuestionRules.Any())
                {
                    var rqpr = testSpecification.RubricTestQuestionRules.First();
                    testSpecification.RemoveRubricTestQuestionRule(rqpr);
                    session.Delete(rqpr);
                }
            }

            if (request.Configurations == null || !request.Configurations.Any())
            {
                session.Flush();
                return response;
            }

            var user = session.Load<User>(request.UserId);
            foreach (var c in request.Configurations)
            {
                var testResultEligibilityType = session.Load<TestResultEligibilityType>(c.TestResultEligibilityTypeId);
                session.Save(new RubricTestQuestionRule
                {
                    TestSpecificationId = c.TestSpecificationId,
                    TestComponentTypeId = c.TestComponentTypeId,
                    TestResultEligibilityType = testResultEligibilityType,
                    MinimumQuestionsAttempted = c.MinimumQuestionsAttempted,
                    MinimumQuestionsPassed = c.MinimumQuestionsPassed,
                    RuleGroup = c.RuleGroup,
                    ModifiedUser = user,
                    ModifiedByNaati = true,
                    ModifiedDate = DateTime.Now
                });
            }

            session.Flush();

            return response;
        }

        public LookupTypeResponse GetTestSpecifications(int credentialTypeId)
        {
            var results = NHibernateSession.Current.Query<TestSpecification>()
                .Where(x => x.CredentialType.Id == credentialTypeId)
                .ToList()
                .Select(y =>
                {
                    var postFix = y.Active ? string.Empty : " - (Inactive)";
                    return new LookupTypeDto()
                    {
                        Id = y.Id,
                        DisplayName = $"{y.Description}{postFix}"
                    };
                });

            return new LookupTypeResponse
            {
                Results = results
            };
        }

        public GetRubricMarkingBandResponse GetMarkingBand(GetMarkingBandRequest request)
        {
            var response = new GetRubricMarkingBandResponse();

            var rubricMarkingBand = NHibernateSession.Current.Get<RubricMarkingBand>(request.RubricMarkingBandId);
            if (rubricMarkingBand == null)
            {
                return response;
            }

            response = new GetRubricMarkingBandResponse
            {
                RubricMarkingBand = _autoMapperHelper.Mapper.Map<RubricMarkingBandDto>(rubricMarkingBand)
            };

            return response;
        }

        public UpdateRubricMarkingBandResponse UpdateMarkingBand(UpdateMarkingBandRequest request)
        {
            var response = new UpdateRubricMarkingBandResponse();

            if (request == null)
            {
                return response;
            }

            var rubricMarkingBand = NHibernateSession.Current.Get<RubricMarkingBand>(request.RubricMarkingBand.BandId);
            if (rubricMarkingBand == null)
            {
                throw new WebServiceException($"Rubric Marking Band is not found (Band ID {request.RubricMarkingBand.BandId})");
            }

            var session = NHibernateSession.Current;

            var user = session.Load<User>(request.UserId);

            rubricMarkingBand.Description = request.RubricMarkingBand.Description;
            rubricMarkingBand.Label = request.RubricMarkingBand.Label;

            rubricMarkingBand.ModifiedByNaati = true;
            rubricMarkingBand.ModifiedDate = DateTime.Now;
            rubricMarkingBand.ModifiedUser = user;

            session.Save(rubricMarkingBand);

            session.Flush();

            return response;
        }

        public GetRubricConfigurationResponse GetRubricConfiguration(GetRubricConfigurationRequest request)
        {
            var response = new GetRubricConfigurationResponse();

            var testSpecification = GetTestSpecificationAndFillConfigurations(request.TestSpecificationId, response);
            if (testSpecification == null)
            {
                return response;
            }

            return response;
        }

        private TestSpecification GetTestSpecificationAndFillConfigurations(int testSpecificationId, GetRubricConfigurationResponse response)
        {
            var testSpecification = NHibernateSession.Current.Load<TestSpecification>(testSpecificationId);
            if (testSpecification == null)
            {
                return testSpecification;
            }

            var testComponents = testSpecification.TestComponents.ToList();
            if (testComponents == null || !testComponents.Any())
            {
                return testSpecification;
            }

            var distinctComponents = from x in testComponents
                                     group x by x.Type.Id into g
                                     select g.First();

            var testComponentDtos = distinctComponents.Select(TestResultQueryService.MapComponent).ToList();

            response.TestSpecificationId = testSpecification.Id;
            response.TestComponents = testComponentDtos;

            return testSpecification;
        }

        public GenericResponse<int> AddTestSpecifiction(AddTestSpecificationRequest model)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(model.CredentialTypeId);

            if(string.IsNullOrEmpty(model.Title))
            {
                throw new WebServiceException($"Title cannot be empty");
            }

            if (credentialType == null)
            {
                throw new WebServiceException($"Credential Type is not found (Stored File ID {model.CredentialTypeId})");
            }

            var  testSpecification = new TestSpecification();

            testSpecification.CredentialType = credentialType;
            testSpecification.Description = model.Title;
            testSpecification.Active = false;
            testSpecification.ModifiedDate = model.ModifiedDate;
            testSpecification.ModifiedUser = NHibernateSession.Current.Get<User>(model.ModifiedUser);
            testSpecification.ModifiedByNaati = model.ModifiedByNaati;

            NHibernateSession.Current.SaveOrUpdate(testSpecification);
            NHibernateSession.Current.Flush();

            return testSpecification.Id;
        }

        public GenericResponse<List<int>> GetTestSessionIdsWhereMaterialsNotYetFullyAllocated()
        {
            TestSession mTestSession = null;
            TestSitting mTestSitting = null;

            var testSessionsWithoutMaterialReminderDaysString = NHibernateSession.Current.Query<SystemValue>().Where(x => x.ValueKey.Equals("TestSittingsWithoutMaterialReminderDays")).First().Value;
            var testSessionsWithoutMaterialReminderDaysInt = Convert.ToInt16(testSessionsWithoutMaterialReminderDaysString);

            int dto = 0;

            var projectionsList = Projections.ProjectionList()
            .Add(Projections.Group(() => mTestSession.Id).WithAlias(() => dto));

            var whereFilter = Restrictions.Conjunction()
                .Add(Restrictions.Gt(Projections.Property(() => mTestSession.TestDateTime), DateTime.Now))
                .Add(Restrictions.Lt(Projections.Property(() => mTestSession.TestDateTime), DateTime.Now.AddDays(testSessionsWithoutMaterialReminderDaysInt)))
                .Add(Restrictions.Eq(Projections.Property(() => mTestSession.IsActive), true));

            var testSessionIdResult = NHibernateSession.Current.QueryOver(() => mTestSession)
                .Inner.JoinAlias(x => mTestSession.TestSittings, () => mTestSitting)
                .Where(whereFilter)
                .Select(projectionsList);

            var transformedTestSessionIdResults = testSessionIdResult.List<int>().ToList();
            // create empty list of int so we have somewhere to store the ids to be removed
            var fullyAllocatedIds = new List<int>();

            foreach (var testSessionId in transformedTestSessionIdResults)
            {
                // set fully allocated to true
                var fullyAllocated = true;
                // Get all applicants for the current test session id
                var applicants = _testSessionQueryService.GetApplicantsById(testSessionId, false).Results;
                // for each applicant of the test session, find any test material ids that are null and if any set fully allocated to false and break out of loop
                // as there is no reason to continue
                foreach(var applicant in applicants)
                {
                    if (applicant.TestTasks.Any(x => x.TestMaterialId == null))
                    {
                        fullyAllocated = false;
                        break;
                    }
                    continue;
                }
                // if fullt allocated is still true after iterating all the applicants then add the current test session id to the fully allocated if list
                if (fullyAllocated)
                {
                    fullyAllocatedIds.Add(testSessionId);
                }
                continue;
            }
            // for each id in fully allocated ids, check if the id is in the test session id list and if so remove that id as it should not be included
            // in our output
            foreach(var id in fullyAllocatedIds)
            {
                if (transformedTestSessionIdResults.Contains(id))
                {
                    transformedTestSessionIdResults.RemoveAll(x => x == id);
                }
            }

            return new GenericResponse<List<int>>(transformedTestSessionIdResults);
        }
    }
}