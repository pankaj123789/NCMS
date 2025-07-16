using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.Extensions;
using F1Solutions.Naati.Common.Dal.QueryHelper;
using NHibernate.Util;

namespace F1Solutions.Naati.Common.Dal
{
    public class TestMaterialQueryService : ITestMaterialQueryService
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestMaterialQueryService(IAutoMapperHelper autoMapperHelper)
        {
            _autoMapperHelper = autoMapperHelper;
        }

        public TestMaterialDto GetTestMaterials(int id)
        {
            var result = NHibernateSession.Current.Query<TestMaterial>().First(x => x.Id == id);
            return MapToTestMaterialDto(result);
        }

        public GetTestSessionTestMaterialResponse GetTestMaterialsByAttendees(IList<int> ids, bool showAll)
        {
            var result = new GetTestSessionTestMaterialResponse();
            var testSessionAttendees = NHibernateSession.Current.Query<TestSitting>().Where(s => ids.Contains(s.Id)).ToList();

            var credentialTypeIds = new List<int>();
            var languageIds = new List<int>();
            var skillsIds = new List<int>();

            testSessionAttendees.ForEach(
                x =>
                {
                    credentialTypeIds.Add(x.CredentialRequest.CredentialType.Id);
                    languageIds.Add(x.CredentialRequest.Skill.Language1.Id);
                    languageIds.Add(x.CredentialRequest.Skill.Language2.Id);
                    skillsIds.Add(x.CredentialRequest.Skill.Id);
                });

            credentialTypeIds = credentialTypeIds.Distinct().ToList();
            languageIds = languageIds.Distinct().ToList();
            skillsIds = skillsIds.Distinct().ToList();

            var allMaterialsByAttendeeCredentialType = NHibernateSession.Current.Query<TestMaterial>()
                .Where(x => credentialTypeIds.Contains(x.TestComponentType.TestSpecification.CredentialType.Id))
                .Where(y =>
                    (y.TestComponentType.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Language && languageIds.Contains(y.Language.Id)) ||
                    y.TestComponentType.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Skill && skillsIds.Contains(y.Skill.Id))
                .Where(z => z.Available);


            if (showAll)
            {
                result.Results = allMaterialsByAttendeeCredentialType.ToList().Select(MapToTestMaterialDto);
                return result;
            }

            var existingTestMaterialIds = GetExistingTestMaterialIdsByAttendees(ids);
            languageIds = languageIds.Any() ? languageIds : new List<int> { 0 };
            existingTestMaterialIds = existingTestMaterialIds.Any() ? existingTestMaterialIds : new List<int> { 0 };
            if (languageIds.Any() && existingTestMaterialIds.Any())
            {
                result.Results =
                    allMaterialsByAttendeeCredentialType.Where(
                            y => languageIds.Contains(y.Language.Id) || existingTestMaterialIds.Contains(y.Id))
                        .ToList()
                        .Select(MapToTestMaterialDto);
            }

            return result;
        }


        public IList<int> GetExistingTestMaterialIdsByAttendees(IList<int> ids)
        {
            var existingTestMaterialIds = NHibernateSession.Current.Query<TestSittingTestMaterial>().Where(s => ids.Contains(s.TestSitting.Id) && s.TestMaterial.Available).Select(x => x.TestMaterial.Id).ToList();
            return existingTestMaterialIds;
        }

        public GetTestSessionTestMaterialResponse GetTestMaterialsByAttendee(int id)
        {
            var existingTestMaterials = NHibernateSession.Current.Query<TestSittingTestMaterial>().Where(s => s.TestSitting.Id == id && s.TestMaterial.Available).Select(x => MapToTestMaterialDto(x.TestMaterial)).ToList();

            return new GetTestSessionTestMaterialResponse
            {
                Results = existingTestMaterials
            };
        }

        public GetAttendeesTestSpecificationTestMaterialResponse GetTestSpecificationTestMaterialsByAttendanceId(int attendanceId, bool eportalDownload)
        {
            var response = new GetAttendeesTestSpecificationTestMaterialResponse
            {
                AttendeeTestSpecificationTestMaterialList = new List<AttendeeTestSpecificationTestMaterial>()
            };
            var documentTypeIds = new List<int>
            {
                (int) DocumentTypeEnum.CandidateTestMaterial,
                (int) DocumentTypeEnum.Script,
                (int) DocumentTypeEnum.ExaminerTestMaterial,
                (int) DocumentTypeEnum.CandidateBrief
            };

            var downloadTestMaterialsByAttendanceId = NHibernateSession.Current.Query<TestSittingTestMaterial>().Where(s => s.TestSitting.Id == attendanceId)
                .Select(x =>
                    new
                    {
                        TestMaterialId = x.TestMaterial.Id,
                        TaskTypeLabel = x.TestMaterial.TestComponentType.Label,
                        TestComponentNumber = x.TestComponent.ComponentNumber,
                        Label = x.TestComponent.Label
                    }).ToList().Distinct();

            var downloadTestSpecificationByAttendanceId = NHibernateSession.Current.Query<TestSitting>().Where(s => s.Id == attendanceId)
                .Select(x =>
                    new
                    {
                        TestSpecificationId = x.TestSpecification.Id,
                        Description = x.TestSpecification.Description,
                        CredentialType = x.TestSpecification.CredentialType.InternalName
                    }).ToList().Distinct();

            var attendeeTestSpecificationTestMaterial = new AttendeeTestSpecificationTestMaterial
            {
                AttendanceId = attendanceId,
                AttendeeTestMaterialList = new List<AttendeeTestMaterial>(),
                AttendeeTestSpecification = new AttendeeTestSpecification()
            };

            var attendeeTestMaterialList = new List<AttendeeTestMaterial>();
            var attendeeTestSpecification = new AttendeeTestSpecification();

            //download test materials
            foreach (var downloadTestMaterial in downloadTestMaterialsByAttendanceId)
            {
                var downloadTestMaterialId = downloadTestMaterial.TestMaterialId;
                var attendeeTestMaterial = new AttendeeTestMaterial
                {
                    Id = downloadTestMaterialId,
                    TaskTypeLabel = downloadTestMaterial.TaskTypeLabel,
                    TestComponentNumber = downloadTestMaterial.TestComponentNumber,
                    Label = downloadTestMaterial.Label
                };

                var storedFileList = GetTestMaterialAttachment(downloadTestMaterialId, eportalDownload).ToList()
                    .Where(x => documentTypeIds.Contains(x.StoredFile.DocumentType.Id) && !x.Deleted)
                    .Select(y => new StoredFileMarterialDto
                    {
                        Id = y.StoredFile.Id,
                        FileName = y.StoredFile.FileName,
                        FilePath = y.StoredFile.ExternalFileId,
                        DocumentTypeId = y.StoredFile.DocumentType.Id,
                        Title = y.Title,
                        TestMaterialId = y.TestMaterialId,
                        EportalDownload = y.EportalDownload,
                        MergeDocument = y.MergeDocument
                        
                    }).Distinct().ToList();

                attendeeTestMaterial.StoredFileList = storedFileList;
                attendeeTestMaterialList.Add(attendeeTestMaterial);
            }

            //add test specification and test materials together
            if (attendeeTestMaterialList.Count > 0)
            {
                //download test specification if any testmaterials
                foreach (var downloadTestSpecification in downloadTestSpecificationByAttendanceId)
                {
                    var downloadTestSpecificationId = downloadTestSpecification.TestSpecificationId;
                    attendeeTestSpecification.Id = downloadTestSpecificationId;

                    var storedFileList = GetTestSpecificationAttachment(downloadTestSpecificationId, eportalDownload).ToList()
                        .Where(x => !x.Deleted)
                        .Select(y => new StoredFileMarterialDto
                        {
                            Id = y.StoredFile.Id,
                            FileName = y.StoredFile.FileName,
                            FilePath = y.StoredFile.ExternalFileId,
                            DocumentTypeId = y.StoredFile.DocumentType.Id,
                            Title = y.Title,
                            TestSpecificationId = y.TestSpecificationId,
                            EportalDownload = y.EportalDownload,
                            MergeDocument = y.MergeDocument
                        }).Distinct().ToList();

                    attendeeTestSpecification.StoredFileList = storedFileList;
                }

                attendeeTestSpecificationTestMaterial.AttendeeTestSpecification = attendeeTestSpecification;
                attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList = attendeeTestMaterialList;
                response.AttendeeTestSpecificationTestMaterialList.Add(attendeeTestSpecificationTestMaterial);
            }

            return response;
        }

        public GetAttendeesTestSpecificationTestMaterialResponse GetAttendeesTestSpecificationTestMaterialList(GetAttendeesTestSpecificationTestMaterialRequest request)
        {
            var response = new GetAttendeesTestSpecificationTestMaterialResponse();

            var documentTypeIds = new List<int> { (int)DocumentTypeEnum.CandidateTestMaterial, (int)DocumentTypeEnum.CandidateBrief, (int)DocumentTypeEnum.Script };

            if (request.IncludeExaminer)
            {
                documentTypeIds.Add((int)DocumentTypeEnum.ExaminerTestMaterial);
            }

            response.AttendeeTestSpecificationTestMaterialList = new List<AttendeeTestSpecificationTestMaterial>();

            foreach (var item in request.CustomerAttendanceIdList)
            {
                var downloadTestMaterialsByAttendanceId = NHibernateSession.Current.Query<TestSittingTestMaterial>().Where(s => s.TestSitting.Id == item.AttendanceId)
                    .Select(x =>
                        new
                        {
                            TestMaterialId = x.TestMaterial.Id,
                            TaskTypeLabel = x.TestMaterial.TestComponentType.Label,
                            TestComponentNumber = x.TestComponent.ComponentNumber,
                            Label = x.TestComponent.Label
                        }).ToList().Distinct();

                var downloadTestSpecificationsByAttendanceId = NHibernateSession.Current.Query<TestSitting>().Where(s => s.Id == item.AttendanceId)
                    .Select(x =>
                        new
                        {
                            TestSpecificationId = x.TestSpecification.Id,
                            Description = x.TestSpecification.Description,
                            CredentialType = x.TestSpecification.CredentialType.InternalName
                        }).ToList().Distinct();

                var attendeeTestSpecificationTestMaterial = new AttendeeTestSpecificationTestMaterial
                {
                    AttendanceId = item.AttendanceId,
                    CustomerNumber = item.CustomerNo,
                    AttendeeTestMaterialList = new List<AttendeeTestMaterial>(),
                    AttendeeTestSpecification = new AttendeeTestSpecification()
                };

                var attendeeTestMaterialList = new List<AttendeeTestMaterial>();
                var attendeeTestSpecification = new AttendeeTestSpecification();

                //download test materials
                foreach (var downloadTestMaterial in downloadTestMaterialsByAttendanceId)
                {
                    var downloadTestMaterialId = downloadTestMaterial.TestMaterialId;
                    var attendeeTestMaterial = new AttendeeTestMaterial
                    {
                        Id = downloadTestMaterialId,
                        TaskTypeLabel = downloadTestMaterial.TaskTypeLabel,
                        TestComponentNumber = downloadTestMaterial.TestComponentNumber,
                        Label = downloadTestMaterial.Label
                    };

                    var storedFileList = GetTestMaterialAttachment(downloadTestMaterialId).ToList()
                        .Where(x => documentTypeIds.Contains(x.StoredFile.DocumentType.Id) && !x.Deleted)
                        .Select(y => new StoredFileMarterialDto
                        {
                            Id = y.StoredFile.Id,
                            FileName = y.StoredFile.FileName,
                            DocumentTypeId = y.StoredFile.DocumentType.Id,
                            Title = y.Title,
                            TestMaterialId = y.TestMaterialId,
                            EportalDownload = y.EportalDownload,
                            MergeDocument = y.MergeDocument

                        }).Distinct().ToList();

                    attendeeTestMaterial.StoredFileList = storedFileList;
                    attendeeTestMaterialList.Add(attendeeTestMaterial);
                }

                //add test specification and test materials together
                if (attendeeTestMaterialList.Count > 0)
                {
                    //download test specification if any testmaterials
                    foreach (var downloadTestSpecification in downloadTestSpecificationsByAttendanceId)
                    {
                        var downloadTestSpecificationId = downloadTestSpecification.TestSpecificationId;
                        attendeeTestSpecification.Id = downloadTestSpecificationId;

                        var storedFileList = GetTestSpecificationAttachment(downloadTestSpecificationId).ToList()
                            .Where(x => !x.Deleted)
                            .Select(y => new StoredFileMarterialDto
                            {
                                Id = y.StoredFile.Id,
                                FileName = y.StoredFile.FileName,
                                DocumentTypeId = y.StoredFile.DocumentType.Id,
                                Title = y.Title,
                                TestSpecificationId = y.TestSpecificationId,
                                EportalDownload = y.EportalDownload,
                                MergeDocument = y.MergeDocument
                            }).Distinct().ToList();

                        attendeeTestSpecification.StoredFileList = storedFileList;
                    }

                    attendeeTestSpecificationTestMaterial.AttendeeTestSpecification = attendeeTestSpecification;
                    attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList = attendeeTestMaterialList;
                    response.AttendeeTestSpecificationTestMaterialList.Add(attendeeTestSpecificationTestMaterial);
                }
            }

            return response;
        }

        public void AssignMaterial(AssignTestMaterialRequest request)
        {
            var attendees = request.TestSittingIds;
            var components = request.TestComponentIds;


            var testSittingTestMaterialList = new List<TestSittingTestMaterial>();

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var attendeeId in attendees)
                {
                    foreach (var component in components)
                    {
                        //deleting
                        var testSittingTestMaterialListByAttendee = NHibernateSession.Current.QueryOver<TestSittingTestMaterial>().Where(x => x.TestSitting.Id == attendeeId && x.TestComponent.Id == component.Key).List<TestSittingTestMaterial>();
                        NHibernateSession.Current.DeleteList(testSittingTestMaterialListByAttendee.ToList());
                        NHibernateSession.Current.Flush();

                        //Adding
                        foreach (var materialId in component.Value)
                        {
                            testSittingTestMaterialList.Add(new TestSittingTestMaterial
                            {
                                TestComponent = NHibernateSession.Current.Get<TestComponent>(component.Key),
                                TestMaterial = NHibernateSession.Current.Get<TestMaterial>(materialId),
                                TestSitting = NHibernateSession.Current.Get<TestSitting>(attendeeId)
                            });
                        }
                    }
                }

                NHibernateSession.Current.SaveList(testSittingTestMaterialList);

                transaction.Commit();
            }
        }

        public void RemoveTestMaterials(AssignTestMaterialRequest request)
        {
            var attendees = request.TestSittingIds;

            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                foreach (var attendeeId in attendees)
                {
                    //deleting
                    var testSittingTestMaterialListByAttendee = NHibernateSession.Current.QueryOver<TestSittingTestMaterial>().Where(x => x.TestSitting.Id == attendeeId).List<TestSittingTestMaterial>();

                    NHibernateSession.Current.DeleteList(testSittingTestMaterialListByAttendee.ToList());
                    NHibernateSession.Current.Flush();
                }

                transaction.Commit();
            }
        }

        public IEnumerable<PersonTestTaskDto> GetTestTasksPendingToAssign(int testSessionId)
        {
            var testSittings = NHibernateSession.Current.Query<TestSitting>()
                .Where(x => !x.Rejected && x.TestSession.Id == testSessionId)
                .ToList();

            var result = new List<PersonTestTaskDto>();
            var testSessionQueryService = new TestSessionQueryService(_autoMapperHelper);
            foreach (var testSitting in testSittings)
            {
                var person = testSitting.CredentialRequest.CredentialApplication.Person;
                var personName = person.FullName;
                var naatiNumber = person.GetNaatiNumber();
                var testTasks = testSessionQueryService.GetTestTasksByAttendanceId(testSitting);

                var testComponentIds = NHibernateSession.Current.Query<TestSittingTestMaterial>()
                    .Where(x => x.TestSitting.Id == testSitting.Id)
                    .Select(x => x.TestComponent.Id)
                    .ToList();

                var missingMaterials = testTasks.Where(x => testComponentIds.All(y => y != x.TestComponentId))
                    .Select(z => new PersonTestTaskDto
                    {
                        NaatiNumber = naatiNumber,
                        PersonName = personName,
                        TaskLabel = z.TestComponentTypeLabel + z.Label
                    });

                result.AddRange(missingMaterials);
            }

            return result;
        }

        public IEnumerable<TestSpecificationDetailsDto> GetTestSpecificationDetails(TestSpecificationDetailsRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();
            var testSpecifications = queryHelper.GetTestSpecificationDetails(request.TestSessionIds);
            foreach (var testSpecification in testSpecifications)
            {
                testSpecification.ApplicantsWithoutMaterials =
                    queryHelper.GetTotalPendingSittingsWithTasksToAssign(testSpecification.Id, request.TestSessionIds);
            }

            return testSpecifications;
        }


        public IEnumerable<SpecificationSkillDto> GetTestSpecificationSkills(TestSpecificationSkillsRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();
            return queryHelper.GetTestSpecificationSkills(request.TestSessionIds, request.TestSpecificationId);
        }

        public IEnumerable<TestMaterialDetailDto> GetTestSpecificationMaterials(TestSpecificationMaterialRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();
            LoggingHelper.LogInfo($"Start GetTestSpecificationMaterials at {DateTime.Now.ToString()}");
            var result = queryHelper.GetTestSpecificationMaterials(request).ToList();
            LoggingHelper.LogInfo($"End GetTestSpecificationMaterials at {DateTime.Now.ToString()}");
            if (request.IncludeSkillTypes.HasValue && request.IncludeSkillTypes.Value)
            {
                var skillsToIncludeList = GetSkillsToInclude();
                var includedSkills = NHibernateSession.Current.Query<SystemValue>().SingleOrDefault(x => x.ValueKey == "TestMaterialIncludedSkillTypes").Value;
                var skillsToInclude = string.Join(",", skillsToIncludeList);
                var sql = @"SELECT 
                            TestMaterial.TestMaterialId as Id,
                            TestComponentType.TestComponentTypeId as TypeId ,
                            TestComponentType.Label as TypeLabel ,
                            TestMaterial.Title Title ,
		                    2 as StatusId,
                            TestMaterialDomain.TestMaterialDomainId,
                            TestComponentType.Description as TypeDescription ,
                            --INCLUDEDSKILLTYPES as TypeDescription ,
                            3 as applicantRangeTypeId,
                            TestMaterialLastUsed.LastUsedDate,
                            TestMaterialType.TestMaterialTypeId

	                    FROM
	                    tblTestMaterial TestMaterial inner join
	                    tblTestMaterialType TestMaterialType on TestMaterial.TestMaterialTypeId = TestMaterialType.TestMaterialTypeId inner join
	                    tblTestMaterialDomain TestMaterialDomain on TestMaterialDomain.TestMaterialDomainId = TestMaterial.TestMaterialDomainId inner join
	                    tblTestComponentType TestComponentType on TestComponentType.TestComponentTypeId = TestMaterial.TestComponentTypeId inner join
	                    tblTestComponentBaseType TestComponentBaseType on TestComponentBaseType.TestComponentBaseTypeId = TestComponentType.TestComponentBaseTypeId inner join
	                    tblTestSpecification TestSpecification on TestSpecification.TestSpecificationId = TestComponentType.TestSpecificationId inner join
	                    vwTestMaterialLastUsed TestMaterialLastUsed  on TestMaterialLastUsed.TestMaterialId = TestMaterial.TestMaterialId inner join
	                    tblSkill Skill on Skill.SkillId = TestMaterial.SkillId

                    WHERE Skill.SkillId in (SKILLIST)";

                sql = sql.Replace("SKILLIST", skillsToInclude);
                sql = sql.Replace("INCLUDEDSKILLTYPES", includedSkills);

                LoggingHelper.LogInfo($"Start GetTestSpecificationMaterials with IncludedSkillTypes at {DateTime.Now.ToString()}");
                var addedResults = NHibernateSession.Current.TransformSqlQueryDataRowResult<TestMaterialDetailDto>(sql);
                LoggingHelper.LogInfo($"End GetTestSpecificationMaterials with IncludedSkillTypes at {DateTime.Now.ToString()}");
                result.AddRange(addedResults.OrderBy(x=>x.TypeLabel));

            }
            return result;
        }


        public IEnumerable<TestTaskDetailDto> GetTestSpecificationTasks(TestTaskRequest request)
        {
            var result = NHibernateSession.Current.Query<TestComponent>()
                .Where(x => x.TestSpecification.Id == request.TestSpecificationId)
                .Select(y =>
                    new TestTaskDetailDto
                    {
                        Id = y.Id,
                        TaskLabel = y.Label,
                        TypeId = y.Type.Id,
                        TypeLabel = y.Type.Label,
                        TaskName = y.Name,
                        RoleplayersRequired = y.Type.RoleplayersRequired
                    }).ToList();

            return result;
        }

        public TestMaterialSummaryDto GetTestMaterialsSummary(TestMaterialsSummaryRequest request)
        {
            var dto = new TestMaterialSummaryDto();

            var queryHelper = new TestMaterialQueryHelper();

            dto.TestSittingIds = queryHelper.GetNotSatTestAttendanceIdsInTestSessions(request);
            dto.ApplicantIdsToOverride = queryHelper.GetApplicantIdsToOverridMaterials(request);
            dto.NewMaterialApplicantsIds = queryHelper.GetNewMaterialsApplicantIds(request);
            dto.ApplicantsAlreadySat = queryHelper.GetApplicantWithAlreadySatMaterials(request);

            return dto;

        }

        public GetExaminersAndRolePlayersResponse GetExaminersAndRolePlayers(TestMaterialsSummaryRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();

            return new GetExaminersAndRolePlayersResponse
            {
                Results = queryHelper.GetApplicantExaminersAndRolePlayers(request)
            };
        }

        public TestMaterialSearchResultResponse SearchTestMaterials(TestMaterialSearchRequest request)
        {
            var testmaterialQueryHelper = new TestMaterialQueryHelper();
            var results = testmaterialQueryHelper.SearchTestMaterials(request);
            return new TestMaterialSearchResultResponse() { Results = results };
        }


        public IEnumerable<SupplementarytTestApplicantDto> GetSupplementaryTestApplicants(SupplementaryTestRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();
            var result = queryHelper.GetSupplementaryTestApplicants(request);
            return result;
        }

        public void DeleteMaterialById(int testSittingTestMaterialId)
        {
            var testSittingTestMaterial = NHibernateSession.Current.Get<TestSittingTestMaterial>(testSittingTestMaterialId);
            NHibernateSession.Current.Delete(testSittingTestMaterial);
            NHibernateSession.Current.Flush();
        }

        public IEnumerable<string> GetAllCustomerAttendanceIdsList(int testSessionId)
        {
            var result = NHibernateSession.Current.Query<TestSitting>().Where(x => x.TestSession.Id == testSessionId &&
                                                                                   x.CredentialRequest.CredentialRequestStatusType.Id == (int)CredentialRequestStatusTypeName.TestSessionAccepted &&
                                                                                   !x.Rejected).ToList();

            var allCustomerAttendanceIds = result.Select(x => x.CredentialRequest.CredentialApplication.Person.NaatiNumberDisplay + "-" + x.Id);

            return allCustomerAttendanceIds;
        }

        public DocumentAdditionalTokenValueDto GetDocumentAdditionalTokens(int attendanceId)
        {
            var response = NHibernateSession.Current.Get<TestSitting>(attendanceId);

            var documentAdditionalTokens = new DocumentAdditionalTokenValueDto
            {
                ApplicationReference = "APP" + response.CredentialRequest.CredentialApplication.Id,
                CredentialRequestType = response.CredentialRequest.CredentialType.ExternalName,
                Skill = response.CredentialRequest.Skill.DisplayName,
                TestSessionName = response.TestSession.Name,
                VenueName = response.TestSession.Venue.Name,
                TestTessionDate = response.TestSession.TestDateTime.ToString("dd/MM/yyyy"),
                TestTessionTime = response.TestSession.TestDateTime.ToString("hh:mm tt")
            };

            return documentAdditionalTokens;
        }

        public IEnumerable<TestMaterialAttachmentDto> GetTestMaterialAttachment(int testMaterialId, bool eportalDownload = false, bool includeDeleted = false)
        {
            return GetTestMaterialsAttachments(new List<int> { testMaterialId }, eportalDownload, includeDeleted);
        }

        public IEnumerable<TestMaterialAttachmentDto> GetTestMaterialsAttachments(IList<int> testMaterialIds, bool eportalDownload = false, bool includeDeleted = false)
        {
            var testMaterialAttachmentDtoDownloadListWithEportal = NHibernateSession.Current.Query<TestMaterialAttachment>()
                .Where(x => testMaterialIds.Contains(x.TestMaterial.Id) && (x.ExaminerToolsDownload == eportalDownload || !eportalDownload))
                .Where(x => (!x.Deleted || includeDeleted))
                .Select(x => new TestMaterialAttachmentDto
                {
                    StoredFile = new StoredFileDto
                    {
                        Id = x.StoredFile.Id,
                        FileName = x.StoredFile.FileName,
                        DocumentType = new DocumentTypeDto
                        {
                            Id = x.StoredFile.DocumentType.Id,
                            Name = x.StoredFile.DocumentType.Name,
                            DisplayName = x.StoredFile.DocumentType.DisplayName
                        },
                        ExternalFileId = x.StoredFile.ExternalFileId,
                        FileSize = x.StoredFile.FileSize,
                        StoredFileStatusChangedDate = x.StoredFile.StoredFileStatusChangeDate,
                        StoredFileStatusType = x.StoredFile.StoredFileStatusType.Id
                    },
                    Title = x.Title,
                    Deleted = x.Deleted,
                    UploadedDateTime = x.StoredFile.UploadedDateTime,
                    UploadedBy = x.StoredFile.UploadedByUser.FullName,
                    TestMaterialId = x.TestMaterial.Id,
                    Id = x.Id,
                    EportalDownload = x.ExaminerToolsDownload,
                    MergeDocument = x.MergeDocument
                }).ToList();

            return testMaterialAttachmentDtoDownloadListWithEportal;
        }


        public IEnumerable<AttendeeTestSpecificationAttachmentDto> GetTestSpecificationAttachment(int testSpecificationId, bool eportalDownload = false)
        {
            var attendeeTestSpecificationAttachmentDtoListWithEportalDownload =
                NHibernateSession.Current.Query<TestSpecificationAttachment>()
                    .Where(x => x.TestSpecification.Id == testSpecificationId && x.ExaminerToolsDownload)
                    .Select(x => new AttendeeTestSpecificationAttachmentDto
                    {
                        StoredFile = new StoredFileDto
                        {
                            Id = x.StoredFile.Id,
                            FileName = x.StoredFile.FileName,
                            DocumentType = new DocumentTypeDto
                            {
                                Id = x.StoredFile.DocumentType.Id,
                                Name = x.StoredFile.DocumentType.Name,
                                DisplayName = x.StoredFile.DocumentType.DisplayName
                            },
                            ExternalFileId = x.StoredFile.ExternalFileId,
                            FileSize = x.StoredFile.FileSize
                        },
                        Title = x.Title,
                        Deleted = x.Deleted,
                        UploadedDateTime = x.StoredFile.UploadedDateTime,
                        UploadedBy = x.StoredFile.UploadedByUser.FullName,
                        TestSpecificationId = testSpecificationId,
                        Id = x.Id,
                        MergeDocument = x.MergeDocument
                    }).ToList();

            var attendeeTestSpecificationAttachmentDtoList =
                NHibernateSession.Current.Query<TestSpecificationAttachment>()
                    .Where(x => x.TestSpecification.Id == testSpecificationId)
                    .Select(x => new AttendeeTestSpecificationAttachmentDto
                    {
                        StoredFile = new StoredFileDto
                        {
                            Id = x.StoredFile.Id,
                            FileName = x.StoredFile.FileName,
                            DocumentType = new DocumentTypeDto
                            {
                                Id = x.StoredFile.DocumentType.Id,
                                Name = x.StoredFile.DocumentType.Name,
                                DisplayName = x.StoredFile.DocumentType.DisplayName
                            },
                            ExternalFileId = x.StoredFile.ExternalFileId,
                            FileSize = x.StoredFile.FileSize
                        },
                        Title = x.Title,
                        Deleted = x.Deleted,
                        UploadedDateTime = x.StoredFile.UploadedDateTime,
                        UploadedBy = x.StoredFile.UploadedByUser.FullName,
                        TestSpecificationId = testSpecificationId,
                        Id = x.Id,
                        MergeDocument = x.MergeDocument
                    }).ToList();

            if (eportalDownload)
                return attendeeTestSpecificationAttachmentDtoListWithEportalDownload;

            return attendeeTestSpecificationAttachmentDtoList;
        }

        public GetDocumentTypesForApplicationTypeResponse GetDocumentTypesForTestMaterialType()
        {
            var names =
                NHibernateSession.Current.Query<DocumentType>()
                    .Where(x => x.DocumentTypeCategory.Id == (int)DocumentTypeCategoryEnum.TestMaterial)
                    .Select(x => x.Name);

            return new GetDocumentTypesForApplicationTypeResponse
            {
                Results = names
            };
        }

        public void DeleteAttachment(int storedFileId)
        {
            var attachment = NHibernateSession.Current.Query<TestMaterialAttachment>().FirstOrDefault(n => n.StoredFile.Id == storedFileId);
            if (attachment != null)
            {
                attachment.Deleted = true;
                NHibernateSession.Current.SaveOrUpdate(attachment);
                NHibernateSession.Current.Flush();
            }
        }

        public CreateOrUpdateResponse CreateOrUpdateTestMaterial(TestMaterialRequest model)
        {
            var credentialType = NHibernateSession.Current.Get<CredentialType>(model.CredentialTypeId);
            var testComponentType = NHibernateSession.Current.Get<TestComponentType>(model.TestComponentTypeId);
            var language = model.LanguageId.HasValue ? NHibernateSession.Current.Get<Language>(model.LanguageId) : null;
            var skill = model.SkillId.HasValue ? NHibernateSession.Current.Get<Skill>(model.SkillId) : null;
            var testMaterialTypeId = model.IsTestMaterialTypeSource ? (int)TestMaterialTypeName.Source : (int)TestMaterialTypeName.Test;
            var testMaterialType = NHibernateSession.Current.Get<TestMaterialType>(testMaterialTypeId);
            var testMaterialDomain = NHibernateSession.Current.Get<TestMaterialDomain>(model.TestMaterialDomainId);

            if (credentialType == null)
            {
                throw new WebServiceException($"Credential Type is not found (Stored File ID {model.CredentialTypeId})");
            }
            if (testMaterialDomain == null)
            {
                throw new WebServiceException($"Test Material Domain {model.TestMaterialDomainId} was not found ");
            }
            if (testComponentType == null)
            {
                throw new WebServiceException($"TestComponentType is not found (Work Practice ID {model.TestComponentTypeId})");
            }
            if (testComponentType.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Language && language == null)
            {
                throw new WebServiceException($"Language is not found (Work Practice ID {model.LanguageId})");
            }
            if (testComponentType.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Skill && skill == null)
            {
                throw new WebServiceException($"Skill is not found (Work Practice ID {model.SkillId})");
            }

            TestMaterial testMaterial;
            if (model.Id > 0)
            {
                testMaterial = NHibernateSession.Current.Get<TestMaterial>(model.Id);
            }
            else
            {
                testMaterial = new TestMaterial();
            }

            testMaterial.Available = model.Available;
            testMaterial.TestMaterialDomain = testMaterialDomain;
            //testMaterial.CredentialType = credentialType;
            testMaterial.Language = language;
            testMaterial.Notes = model.Notes;
            testMaterial.Title = model.Title;
            testMaterial.Skill = skill;
            testMaterial.TestComponentType = testComponentType;
            testMaterial.TestMaterialType = testMaterialType;

            NHibernateSession.Current.SaveOrUpdate(testMaterial);
            NHibernateSession.Current.Flush();

            return new CreateOrUpdateResponse { Id = testMaterial.Id };
        }

        public AttachmentResponse CreateOrUpdateTestMaterialAttachment(TestMaterialAttachmentRequest model)
        {
            var fileService = new FileSystemFileStorageService(_autoMapperHelper);
            var response = fileService.CreateOrUpdateFile(new CreateOrUpdateFileRequest
            {
                UpdateStoredFileId = model.UpdateStoredFileId == 0 ? null : model.UpdateStoredFileId,
                Type = model.FileType,
                StoragePath = model.StoragePath,
                UploadedByUserId = model.UploadedByUserId,
                UploadedDateTime = DateTime.Now,
                FilePath = model.FilePath,
            });

            var storedFile = NHibernateSession.Current.Get<StoredFile>(response.StoredFileId);
            var testMaterial = NHibernateSession.Current.Get<TestMaterial>(model.TestMaterialId);
            if (storedFile == null)
            {
                throw new WebServiceException($"Stored File is not found (Stored File ID {response.StoredFileId})");
            }
            if (testMaterial == null)
            {
                throw new WebServiceException($"Test Material is not found (Test Material ID {model.TestMaterialId})");
            }

            TestMaterialAttachment domain;
            if (model.Id > 0)
            {
                domain = NHibernateSession.Current.Get<TestMaterialAttachment>(model.Id);
                var sentBriefs = NHibernateSession.Current.Query<CandidateBrief>()
                    .Where(x => x.TestMaterialAttachment.Id == model.Id && x.EmailedDate != null)
                    .ToList();
                sentBriefs.ForEach(x => { x.EmailedDate = null; NHibernateSession.Current.SaveOrUpdate(x); });
            }
            else
            {
                domain = new TestMaterialAttachment();
            }

            domain.StoredFile = storedFile;
            domain.Title = model.Title;
            domain.Deleted = model.Deleted;
            domain.TestMaterial = testMaterial;
            domain.ExaminerToolsDownload = model.AvailableForExaminers;
            domain.MergeDocument = model.MergeDocument;


            using (var transaction = NHibernateSession.Current.BeginTransaction())
            {
                if (model.MergeDocument)
                {
                    var testMaterialAttachments = NHibernateSession.Current.QueryOver<TestMaterialAttachment>().Where(x => x.TestMaterial.Id == model.TestMaterialId).List();

                    foreach (var testMaterialAttachment in testMaterialAttachments)
                    {
                        testMaterialAttachment.MergeDocument = false;
                    }

                    foreach(var testMaterialAttachment in testMaterialAttachments)
                    {
                        NHibernateSession.Current.SaveOrUpdate(testMaterialAttachment);
                    }
                }

                if (model.Id > 0)
                {
                    domain.MergeDocument = model.MergeDocument;
                }

                NHibernateSession.Current.SaveOrUpdate(domain);
                transaction.Commit();
            }

            return new AttachmentResponse
            {
                StoredFileId = response.StoredFileId
            };
        }

        private TestMaterialDto MapToTestMaterialDto(TestMaterial domain)
        {
            var data = new TestMaterialDto
            {
                Id = domain.Id,
                Language = domain.Language?.Name ?? domain.Skill?.DisplayName,
                LanguageId = domain.Language?.Id,
                CredentialType = domain.TestComponentType.TestSpecification.CredentialType.InternalName,
                TestComponentTypeName = domain.TestComponentType.Name,
                CredentialTypeId = domain.TestComponentType.TestSpecification.CredentialType.Id,
                TestComponentTypeId = domain.TestComponentType.Id,
                SkillId = domain.Skill?.Id,
                Available = domain.Available,
                Title = domain.Title,
                TestSpecificationId = domain.TestComponentType.TestSpecification.Id,
                TestSpecificationActive = domain.TestComponentType.TestSpecification.Active,
                HasFile = domain.TestMaterialAttachments.Count(y => !y.Deleted) > 0,
                TestMaterialTypeId = domain.TestMaterialType.Id,
                DefaultMaterialRequestHours = domain.TestComponentType.DefaultMaterialRequestHours,
                SkillName = domain.Skill?.DisplayName,
                TestMaterialDomainId = domain.TestMaterialDomain.Id,
                Notes = domain.Notes,
                SourceTestMaterialId = domain.MaterialRequests.FirstOrDefault()?.SourceMaterial?.Id
            };

            data.Links = NHibernateSession.Current.Query<TestMaterialLink>()
                .Where(x => x.FromTestMaterial.Id == domain.Id)
                .Select(y => new TestMaterialLinkDto
                {
                    Id = y.Id,
                    FromTestMaterialId = domain.Id,
                    ToTestMaterialId = y.ToTestMaterial.Id,
                    TypeId = y.TestMaterialLinkType.Id
                })
                .ToList();

            return data;
        }


        public GetTestMaterialsFromTestTaskResponse GetTestMaterialsFromTestTask(GetTestMaterialsFromTestTaskRequest request)
        {
            var testComponent = NHibernateSession.Current.Load<TestComponent>(request.TestComponentId);

            if (testComponent == null)
            {
                throw new WebServiceException($"Test Component is not found (Test Component ID {request.TestComponentId})");
            }

            var testMaterials = NHibernateSession.Current.Query<TestMaterial>()
                .Where(x => x.TestComponentType.Id == testComponent.Type.Id);

            if (request.SkillId.HasValue)
            {
                var skill = NHibernateSession.Current.Load<Skill>(request.SkillId.Value);

                if (testComponent.Type.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Language)
                {
                    testMaterials = testMaterials.Where(tm => tm.Language.Id == skill.Language1.Id || tm.Language.Id == skill.Language2.Id);
                }

                if (testComponent.Type.TestComponentBaseType.Id == (int)TestComponentBaseTypeName.Skill)
                {
                    testMaterials = testMaterials.Where(tm => tm.Skill.Id == skill.Id);
                }
            }

            var includedMaterials = NHibernateSession.Current.Query<TestMaterial>();



            var results = testMaterials.Where(tm => tm.Available
                    && (tm.TestMaterialType.Id == (int)TestMaterialTypeName.Test || tm.TestMaterialType.Id == (int)TestMaterialTypeName.Source))
                    .ToList().Select(MapToTestMaterialDto).ToList();

            if (request.IncludeSystemValueSkillTypes.HasValue && request.IncludeSystemValueSkillTypes.Value == true)
            {
                var skillsToInclude = GetSkillsToInclude();
                includedMaterials = includedMaterials.Where(tm => skillsToInclude.Contains(tm.Skill.Id));
                var includedResults = includedMaterials.Select(MapToTestMaterialDto).ToList();
                results.AddRange(includedResults);
            }

            return new GetTestMaterialsFromTestTaskResponse
            {
                Results = results
            };
        }

        private IList<int> GetSkillsToInclude()
        {
            var includedSkills = NHibernateSession.Current.Query<SystemValue>().SingleOrDefault(x => x.ValueKey == "TestMaterialIncludedSkillTypes").Value;

            var skillsReturned =  NHibernateSession.Current.TransformSqlQueryDataRowResult<SkillReturned>($"select SkillId from tblSkill a inner join tblLanguage b on a.Language1Id = b.LanguageId or a.Language2Id = b.LanguageId where b.Name in ({includedSkills})");
            return skillsReturned.Select(x => x.SkillId).ToList();
        }

        public class SkillReturned
        {
            public int SkillId { get; set; }
        }

        public IEnumerable<TestMaterialApplicantDto> GetApplicantWithAlreadySatMaterialsForTestSession(int testSessionId)
        {
            var queryHelper = new TestMaterialQueryHelper();

            return queryHelper.GetApplicantWithAlreadySatMaterialsForTestSession(testSessionId);
        }

        public IEnumerable<TestSittingDetailsDto> GetPeopleWithOtherTestSittingAssingnedForTheSameDay(int testSessionId)
        {
            var queryHelper = new TestMaterialQueryHelper();

            return queryHelper.GetPeopleWithOtherTestSittingAssingnedForTheSameDay(testSessionId);
        }

        public IEnumerable<ApplicationBriefDto> GetPendingCandidateBriefsToSend(PendingBriefRequest request)
        {
            var queryHelper = new TestMaterialQueryHelper();

            var allBriefs = queryHelper.GetPendingCandidateBriefsToSend(request);

            //if supplementary then need to only send relevant material
            //if parts of the test were passed then non need to send those4 attachments
            allBriefs = queryHelper.FilterOutSupplementaryMaterialsThatPassed(allBriefs);

            return allBriefs;
        }

        public ServiceResponse<IEnumerable<DocumentLookupTypeDto>> GetTestMaterialDocuments(int testMaterialId)
        {
            var result = NHibernateSession.Current.Query<TestMaterialAttachment>()
                .Where(x => x.TestMaterial.Id == testMaterialId && x.Deleted == false).ToList().Select(y => new DocumentLookupTypeDto()
                {
                    Id = y.StoredFile.Id,
                    DisplayName = y.Title,
                    DocumentTypeId = y.StoredFile.DocumentType.Id,
                    FileType = Path.GetExtension(y.StoredFile.FileName),
                    Size = y.StoredFile.FileSize,
                    UploadedBy = y.StoredFile.UploadedByUser != null ? y.StoredFile.UploadedByUser.FullName : y.StoredFile.UploadedByPerson.FullName,
                    ExaminersAvailable = y.ExaminerToolsDownload,
                    MergeDocument = y.MergeDocument
                }).ToList();

            return new ServiceResponse<IEnumerable<DocumentLookupTypeDto>>() { Data = result };
        }

        public LookupTypeResponse GetTestMaterialDomains(int credentialTypeId)
        {
            return GetCredentialTypeDomains(new List<int>() { credentialTypeId });
        }

        public LookupTypeResponse GetCredentialTypeDomains(List<int> credentialTypeIdIntList)
        {
            var response = NHibernateSession.Current.Query<CredentialTypeTestMaterialDomain>()
                .Where(x => credentialTypeIdIntList.Contains(x.CredentialType.Id)).Select(
                    y => new LookupTypeDto()
                    {
                        Id = y.TestMaterialDomain.Id,
                        DisplayName = y.TestMaterialDomain.DisplayName
                    }).ToList().OrderBy(y => y.DisplayName);

            return new LookupTypeResponse() { Results = response };
        }

        public GetTestMaterialCreationPaymentsResponse GetTestMaterialCreationPayments(GetTestMaterialCreationPaymentsRequest request)
        {

            var sql = $"select * from vwTestMaterialCreationPayments where NaatiNumber ={request.ExaminerNaatiNumber} order by TestMaterialId desc";

            var results = NHibernateSession.Current.TransformSqlQueryDataRowResult<TestMaterialCreationPaymentDto>(sql);
            return new GetTestMaterialCreationPaymentsResponse
            {
                Payments = results
            };
        }

        public GenericResponse<List<string>> GetIncludeSystemValueSkillNames()
        {
            var valueList = new List<string>();
            valueList.Add(NHibernateSession.Current.Query<SystemValue>().SingleOrDefault(x => x.ValueKey == "TestMaterialIncludedSkillTypes").Value);

            return valueList;
        }
    }
}
