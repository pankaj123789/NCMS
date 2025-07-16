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
using Ncms.Contracts;
using GenericResponse = F1Solutions.Naati.Common.Contracts.Bl.DTO.GenericResponse<bool>;

namespace Ncms.Bl
{
    public class TestSpecificationService : ITestSpecificationService
    {
        private readonly ITestSpecificationQueryService _testSpecificationQueryService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public TestSpecificationService(ITestSpecificationQueryService testSpecificationQueryService, 
            IUserService userService, 
            IFileService fileService, 
            IFileStorageService fileStorageService, IAutoMapperHelper autoMapperHelper)
        {
            _testSpecificationQueryService = testSpecificationQueryService;
            _userService = userService;
            _fileService = fileService;
            _fileStorageService = fileStorageService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<int> AddTestSpecification(AddTestSpecificationRequest model)
        {
            if (model.CredentialTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CredentialTypeId));
            }

            model.ModifiedUser = _userService.Get().Id;
            model.ModifiedByNaati = true;
            model.ModifiedDate = DateTime.Now;

            return _testSpecificationQueryService.AddTestSpecifiction(model);
        }

        public IEnumerable<TestSpecificationModel> Get(int id)
        {
            var result =
                _testSpecificationQueryService.Get(new TestSpecificationRequest
                {
                    Id = id
                }).List?.OrderBy(x => x.Id).Select(x => new TestSpecificationModel
                {
                    Reference = $"SPEC{x.Id}",
                    Id = x.Id,
                    Active = x.Active,
                    CredentialTypeId = x.CredentialTypeId,
                    Description = x.Description,
                    OverallPassMark = x.OverallPassMark,
                    CredentialType = x.CredentialType,
                    ResultAutoCalculation = x.ResultAutoCalculation,
                    IsRubric = x.IsRubric
                }).ToList();

            return result;
        }

        public IEnumerable<TestComponentModel> GetTestComponentsBySpecificationId(int testSpecificationId)
        {
            if (testSpecificationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(testSpecificationId));

            var result =
                _testSpecificationQueryService.GetTestComponentsBySpecificationId(new TestSpecificationRequest()
                {
                    Id = testSpecificationId
                }).List?.Select(x => new TestComponentModel
                {
                    Id = x.Id,
                    ComponentNumber = x.ComponentNumber,
                    TaskType = x.TaskType,
                    TaskTypeDescription = x.TaskTypeDescription,
                    BasedOn = x.BasedOn,
                    PassMark = x.PassMark ?? 0,
                    TotalMarks = x.TotalMarks ?? 0,
                    Name = x.Name,
                    MinExaminerCommentLength = x.MinExaminerCommentLength,
                    MinNaatiCommentLength = x.MinNaatiCommentLength
                }).ToList();

            return result;
        }

        public GenericResponse<bool> CanUpload(int testSpecificationId)
        {
            var response = new GenericResponse<bool>()
            {
                Data = false
            };

            if (testSpecificationId <= 0)
                throw new ArgumentOutOfRangeException(nameof(testSpecificationId));

            var canUpload = _testSpecificationQueryService.CanUpload(new TestSpecificationRequest()
            {
                Id = testSpecificationId
            });

            response.Data = canUpload;

            return response;

        }

        public IEnumerable<string> GetDocumentTypesForTestSpecificationType()
        {
            var response = _testSpecificationQueryService.GetDocumentTypesForTestSpecificationType();
            return response.Results;
        }

        public GenericResponse<string> UpdateTestSpecification(TestSpecificationModel model)
        {
            var response = new GenericResponse<string>();

            var request = new TestSpecificationRequest
            {
                Id = model.Id,
                Active = model.Active,
                Description = model.Description,
                ResultAutoCalculation = model.ResultAutoCalculation,
                UserId = _userService.Get().Id
            };
            response = _testSpecificationQueryService.Edit(request, response);

            return response;
        }

        public IEnumerable<TestSpecificationAttachmentModel> GetAttachments(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            var attachments =
                _testSpecificationQueryService.GetAttachments(new TestSpecificationRequest() { Id = id })
                    .List.Select(x => new TestSpecificationAttachmentModel
                    {
                        Id = x.TestSpecificationAttachmentId,
                        TestSpecificationAttachmentId = x.TestSpecificationAttachmentId,
                        DocumentType = x.DocumentType,
                        FileName = x.FileName,
                        FileSize = x.FileSize,
                        FileType = x.DocumentType,
                        StoredFileId = x.StoredFileId,
                        Type = x.Type,
                        TestSpecificationId = x.TestSpecificationId,
                        Title = x.Description,
                        UploadedByName = x.UploadedByName,
                        UploadedDateTime = x.UploadedDateTime,
                        MergeDocument = x.MergeDocument,
                        EportalDownload = x.ExaminerToolsDownload,
                        SoftDeleteDate = x.StoredFileStatusTypeId != 1? x.StoredFileStatusChangeDate:null
                    });
            return attachments;
        }

        public int CreateOrReplaceAttachment(TestSpecificationAttachmentModel model)
        {
            var fieTypeId = (int)model.Type;
            var allowedDocumentTypes = _fileService.GetAllowedDocumentTypesToUpload();

            if (allowedDocumentTypes.All(x => x.Id != fieTypeId))
            {
                throw new UserFriendlySamException("You do not have permission to change this document type");
            }

            var request = new TestSpecificationAttachmentRequest()
            {
                Id = model.Id,
                DocumentType = model.DocumentType,
                FileName = model.FileName,
                TestSpecificationAttachmentId = model.TestSpecificationAttachmentId,
                FilePath = model.FilePath,
                StoredFileId = model.StoredFileId,
                UploadedDateTime = model.UploadedDateTime,
                TestSpecificationId = model.TestSpecificationId,
                FileType = model.FileType,
                UploadedByName = model.UploadedByName,
                Title = model.Title,
                Type = model.Type,
                UploadedByUserId = model.UploadedByUserId,
                StoragePath = model.StoragePath,
                MergeDocument = model.MergeDocument,
                EportalDownload = model.EportalDownload
            };
            return _testSpecificationQueryService.CreateOrReplaceAttachment(request).StoredFileId;
        }

        public void DeleteAttachment(int storedFileId)
        {
            if (storedFileId <= 0)
                throw new ArgumentOutOfRangeException(nameof(storedFileId));

            _testSpecificationQueryService.DeleteAttachment(new TestSpecificationAttachmentRequest()
            {
                StoredFileId = storedFileId,
                Type = StoredFileType.CandidateCoverSheet
            });
        }

        public GetQuestionPassRulesResponseModel GetQuestionPassRules(int testSpecificationId)
        {
            var response = _testSpecificationQueryService.GetQuestionPassRules(new GetRubricConfigurationRequest
            {
                TestSpecificationId = testSpecificationId
            });

            return _autoMapperHelper.Mapper.Map<GetQuestionPassRulesResponseModel>(response);
        }

        public GenericResponse SaveQuestionPassRules(int testSpecificationId, IEnumerable<RubricQuestionPassRuleModel> request)
        {
            var errors = new List<string>();

            var duplicated = from r in request
                             group r by new { r.RubricMarkingAssessmentCriterionId, r.TestComponentTypeId, r.RuleGroup, r.TestSpecificationId } into g
                             where g.Count() > 1
                             select g.Key;

            if (duplicated.Any())
            {
                var specification = _testSpecificationQueryService.GetQuestionPassRules(new GetRubricConfigurationRequest
                {
                    TestSpecificationId = duplicated.FirstOrDefault().TestSpecificationId
                });

                foreach (var d in duplicated)
                {
                    var component = specification.TestComponents.FirstOrDefault(c => c.TestComponentTypeId == d.TestComponentTypeId);
                    if (component == null)
                    {
                        throw new NullReferenceException($"Invalid TestComponentTypeId: {d.TestComponentTypeId}");
                    }

                    var rubricMarkingAssessmentCriterion = component.RubricMarkingCompentencies.SelectMany(r => r.RubricMarkingAssessmentCriteria).FirstOrDefault(a => a.AssessmentCriterionId == d.RubricMarkingAssessmentCriterionId);
                    if (rubricMarkingAssessmentCriterion == null)
                    {
                        throw new NullReferenceException($"Invalid RubricMarkingAssessmentCriterionId: {d.RubricMarkingAssessmentCriterionId}");
                    }

                    errors.Add($"Duplicated entries for criterion {rubricMarkingAssessmentCriterion.Name} in the task {component.Name}");
                }
            }

            if (errors.Any())
            {
                return new GenericResponse(false)
                {
                    Errors = errors
                };
            }

            var configurations = request.Select(r => _autoMapperHelper.Mapper.Map<RubricQuestionPassRuleDto>(r));
            var user = _userService.Get();

            _testSpecificationQueryService.SaveQuestionPassRules(new SaveQuestionPassRulesRequest
            {
                TestSpecificationId = testSpecificationId,
                Configurations = configurations,
                UserId = user.Id
            });

            return true;
        }

        public GetTestBandRulesResponseModel GetTestBandRules(int testSpecificationId)
        {
            var response = _testSpecificationQueryService.GetTestBandRules(new GetRubricConfigurationRequest
            {
                TestSpecificationId = testSpecificationId
            });

            return _autoMapperHelper.Mapper.Map<GetTestBandRulesResponseModel>(response);
        }

        public GetRubricConfigurationResponseModel GetRubricConfiguration(int testSpecificationId)
        {
            var response = _testSpecificationQueryService.GetRubricConfiguration(new GetRubricConfigurationRequest
            {
                TestSpecificationId = testSpecificationId
            });

            return _autoMapperHelper.Mapper.Map<GetRubricConfigurationResponseModel>(response);
        }

        public GenericResponse SaveTestBandRules(int testSpecificationId, IEnumerable<RubricTestBandRuleModel> request)
        {
            var errors = ValidateTestBandRules(request);

            if (errors.Any())
            {
                return new GenericResponse(false)
                {
                    Errors = errors
                };
            }

            var configurations = request.Select(r => _autoMapperHelper.Mapper.Map<RubricTestBandRuleDto>(r));
            var user = _userService.Get();
            _testSpecificationQueryService.SaveTestBandRules(new SaveTestBandRulesRequest
            {
                TestSpecificationId = testSpecificationId,
                Configurations = configurations,
                UserId = user.Id
            });

            return true;
        }

        private List<string> ValidateTestBandRules(IEnumerable<RubricTestBandRuleModel> request)
        {
            var errors = new List<string>();

            var duplicated = from r in request
                             group r by new { r.RubricMarkingAssessmentCriterionId, r.TestComponentTypeId, r.RuleGroup, r.TestSpecificationId, r.TestResultEligibilityTypeId } into g
                             where g.Count() > 1
                             select g.Key;

            if (duplicated.Any())
            {
                var specification = _testSpecificationQueryService.GetQuestionPassRules(new GetRubricConfigurationRequest
                {
                    TestSpecificationId = duplicated.FirstOrDefault().TestSpecificationId
                });

                foreach (var d in duplicated)
                {
                    var component = specification.TestComponents.FirstOrDefault(c => c.TestComponentTypeId == d.TestComponentTypeId);
                    if (component == null)
                    {
                        throw new NullReferenceException($"Invalid TestComponentTypeId: {d.TestComponentTypeId}");
                    }

                    var rubricMarkingAssessmentCriterion = component.RubricMarkingCompentencies.SelectMany(r => r.RubricMarkingAssessmentCriteria).FirstOrDefault(a => a.AssessmentCriterionId == d.RubricMarkingAssessmentCriterionId);
                    if (rubricMarkingAssessmentCriterion == null)
                    {
                        throw new NullReferenceException($"Invalid RubricMarkingAssessmentCriterionId: {d.RubricMarkingAssessmentCriterionId}");
                    }

                    errors.Add($"Duplicated entries for criterion {rubricMarkingAssessmentCriterion.Name} in the task {component.Name}" + (!String.IsNullOrWhiteSpace(d.RuleGroup) ? $" and group { d.RuleGroup }" : null));
                }
            }

            foreach (var r in request)
            {
                if (r.NumberOfQuestions <= 0)
                {
                    errors.Add("Invalid Number of Questions for one or more conditions.");
                }
                if (r.TestResultEligibilityTypeId <= 0)
                {
                    errors.Add("Invalid Result Eligibility Type for one or more conditions.");
                }
            }

            return errors;
        }

        public GetTestQuestionRulesResponseModel GetTestQuestionRules(int testSpecificationId)
        {
            var response = _testSpecificationQueryService.GetTestQuestionRules(new GetRubricConfigurationRequest
            {
                TestSpecificationId = testSpecificationId
            });

            return _autoMapperHelper.Mapper.Map<GetTestQuestionRulesResponseModel>(response);
        }

        public GetRubricMarkingBandResponseModel GetMarkingBand(int rubricMarkingBandId)
        {
            var response = _testSpecificationQueryService.GetMarkingBand(new GetMarkingBandRequest
            {
                RubricMarkingBandId = rubricMarkingBandId
            });

            return _autoMapperHelper.Mapper.Map<GetRubricMarkingBandResponseModel>(response);
        }

        public void SaveTestQuestionRules(int testSpecificationId, IEnumerable<RubricTestQuestionRuleModel> request)
        {
            ValidateTestQuestionRule(request);
            var configurations = request.Select(r => _autoMapperHelper.Mapper.Map<RubricTestQuestionRuleDto>(r));
            var user = _userService.Get();
            _testSpecificationQueryService.SaveTestQuestionRules(new SaveTestQuestionRulesRequest
            {
                TestSpecificationId = testSpecificationId,
                Configurations = configurations,
                UserId = user.Id
            });
        }

        private void ValidateTestQuestionRule(IEnumerable<RubricTestQuestionRuleModel> request)
        {
            foreach (var r in request)
            {
                if (r.MinimumQuestionsAttempted <= 0)
                {
                    throw new UserFriendlySamException("Invalid Number of Minimum Questions Attempted for one or more conditions.");
                }
                if (r.MinimumQuestionsPassed < 0)
                {
                    throw new UserFriendlySamException("Invalid Number of Minimum Questions Passed for zero or more conditions.");
                }
                if (r.TestResultEligibilityTypeId <= 0)
                {
                    throw new UserFriendlySamException("Invalid Result Eligibility Type for one or more conditions.");
                }
            }
        }

        public void UpdateMarkingBand(RubricMarkingBandModel rubricMarkingBandModel)
        {
            var user = _userService.Get();
            _testSpecificationQueryService.UpdateMarkingBand(new UpdateMarkingBandRequest
            {
                UserId = user.Id,
                RubricMarkingBand = _autoMapperHelper.Mapper.Map<RubricMarkingBandDto>(rubricMarkingBandModel)
            });
        }
    }
}
