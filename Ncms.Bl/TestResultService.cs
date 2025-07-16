using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Bl.Mappers;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;
using Ncms.Contracts.Models.File;
using Ncms.Contracts.Models.Job;
using Ncms.Contracts.Models.TestResult;
using IApplicationService = Ncms.Contracts.IApplicationService;
using TestComponentModel = Ncms.Contracts.Models.TestComponentModel;

namespace Ncms.Bl
{
    public class TestResultService : ITestResultService
    {
        private readonly IUserService _userService;
        private readonly ITestResultQueryService _testResultQueryService;
        private readonly ITestQueryService _testQueryService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFileService _fileService;
        private readonly IExaminerToolsService _examinerToolsService;
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly ITestSpecificationQueryService _testSpecificationQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;
        internal static int MaxRubricCommentLength = 2000;

        public TestResultService(IUserService userService, ITestResultQueryService testResultQueryService, ITestQueryService testQueryService, 
            IFileStorageService fileStorageService, IFileService fileService, IExaminerToolsService examinerToolService, 
            ITestMaterialQueryService testMaterialQueryService, ITestSpecificationQueryService testSpecificationQueryService, IAutoMapperHelper autoMapperHelper)
        {
            _userService = userService;
            _testResultQueryService = testResultQueryService;
            _testQueryService = testQueryService;
            _fileStorageService = fileStorageService;
            _fileService = fileService;
            _examinerToolsService = examinerToolService;
            _testMaterialQueryService = testMaterialQueryService;
            _testSpecificationQueryService = testSpecificationQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GenericResponse<IEnumerable<string>> ValidateTestResult(TestResultModel testResult)
        {
            var messages = new List<string>();
            var testSummary = _testQueryService.GetTestSummaryFromTestResultId(testResult.TestResultId).Result;
            
            if (testResult.ResultTypeId != (int)TestResultStatusTypeName.AwaitingResults && testSummary.MarkingSchemaTypeId == (int)MarkingSchemaTypeName.Rubric)
            {
                var rubricResult = ServiceLocator.Resolve<ITestService>().GetTestResultRubricMarking(testResult.TestResultId).Data;
                if (rubricResult.ResultAutoCalculation)
                {
                    if (testResult.ResultTypeId == (int)TestResultStatusTypeName.Failed)
                    {
                        if (rubricResult.ComputedEligibleForPass)
                        {
                            messages.Add(Naati.Resources.Test.PassFailed);
                        }

                        if (testResult.EligibleForSupplementary != rubricResult.ComputedEligibleForSupplementary)
                        {
                            messages.Add(Naati.Resources.Test.EligibleForSupplementary);
                        }

                        if (testResult.EligibleForConcededPass != rubricResult.ComputedEligibleForConcededPass)
                        {
                            messages.Add(Naati.Resources.Test.EligibleForConcededPass);
                        }

                    }
                    if (testResult.ResultTypeId == (int)TestResultStatusTypeName.Passed && !rubricResult.ComputedEligibleForPass)
                    {
                        messages.Add(Naati.Resources.Test.PassFailed);
                    }
                }
                
            }

            return new GenericResponse<IEnumerable<string>>(messages);
        }

         public GenericResponse<int> UpdateTestResult(TestResultModel testResult)
        {
            var dto = _autoMapperHelper.Mapper.Map<TestResultDto>(testResult);

            if (dto.ResultTypeId != (int) TestResultStatusTypeName.Failed && dto.ResultTypeId != (int)TestResultStatusTypeName.AwaitingResults)
            {
                dto.EligibleForSupplementary = false;
                dto.EligibleForConcededPass = false;
            }

            var response = _testResultQueryService.UpdateTestResult( new UpdateTestResultRequest(){ TestResult = dto, MaxCommentLength = MaxRubricCommentLength });

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                throw new UserFriendlySamException(response.ErrorMessage);
            }
            return response.TestResultId;

        }

        public List<Dictionary<string, object>> CalculateTestComponentResult(List<Dictionary<string, object>> dataSet)
        {
            var result = new List<Dictionary<string, object>>();

            foreach (var d in dataSet)
            {
                var current =
                (from r in result
                 where Convert.ToInt32(r["ComponentNumber"]) == Convert.ToInt32(d["ComponentNumber"])
                 select r).FirstOrDefault();

                if (current == null)
                {
                    current = d.ToDictionary(c => c.Key, c => c.Value);
                    current.Add("Count", 1);

                    result.Add(current);
                }
                else
                {
                    current["Count"] = Convert.ToInt32(current["Count"]) + 1;
                    current["Mark"] = Convert.ToDouble(current["Mark"]) + Convert.ToDouble(d["Mark"]);
                }
            }

            foreach (var r in result)
            {
                r["Mark"] = Math.Round(Convert.ToDouble(r["Mark"]) / Convert.ToInt32(r["Count"]), 2).RoundOffForMidWay();
                r.Remove("Count");
            }

            return result;
        }

        public SpecificationResponseModel Specification(int testResultId, bool useOriginalMark)
        {
            var result = new SpecificationResponseModel { Components = new List<TestComponentModel>() };

            GetTestDetailsResponse response = _examinerToolsService.GetTestDetails(new GetTestDetailsRequest
            {
                TestResultId = testResultId,
                UseOriginalResultMark = useOriginalMark
            });

            foreach (var c in response.Components)
            {
                var mapped = _autoMapperHelper.Mapper.Map<TestComponentModel>(c);
                mapped.ReadOnly = c.MarkingResultTypeId == (int)MarkingResultTypeName.FromOriginal;
                result.Components.Add(mapped);
            }

            result.OverallPassMark = response.OverAllPassMark.OverAllPassMark;
            return result;
        }

        public GetMarksResponseModel GetMarks(GetMarksRequestModel request)
        {
            GetTestDetailsResponse marksResponse = null;

            try
            {
                marksResponse = _examinerToolsService.GetTestDetails(new GetTestDetailsRequest
                {
                    TestResultId = request.TestResultId,
                    UseOriginalResultMark = false
                });
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }

            var testComponentMapper = new TestComponentMapper();
            return new GetMarksResponseModel
            {
                Components = marksResponse?.Components.Select(testComponentMapper.Map).ToList() ?? new List<TestComponentModel>(),
                OverAllPassMark = new TestSpecificationPassMarkModel
                {
                    OverAllPassMark = (marksResponse?.OverAllPassMark?.OverAllPassMark).GetValueOrDefault()
                },
                CommentsGeneral = marksResponse?.CommentsGeneral,
                TestMarkingTypeId = marksResponse?.TestMarkingTypeId ?? 0,
                TestResultId = request.TestResultId,
            };
        }

        public GetMarksResponseModel GetMarks(int credentialRequestId)
        {
            var testSessions = ServiceLocator.Resolve<IApplicationService>()
                .GetCredentialRequest(credentialRequestId)
                .Data.TestSessions;
            var lastTestSession = testSessions.OrderByDescending(x => x.CredentialTestSessionId).First(x => !x.Rejected);
            var testResultId = lastTestSession.TestResultId.GetValueOrDefault();
            return GetMarks(new GetMarksRequestModel() { TestResultId = testResultId });

        }

        public void SaveMarks(SaveMarksRequestModel request)
        {
            var testComponentMapper = new TestComponentMapper();
            var serviceRequest = new SaveMarksRequest
            {
                TestResultId = request.TestResultId,
                Components = request.Components.Select(testComponentMapper.MapInverse).ToArray()
            };

            try
            {
                _testQueryService.SaveMarks(serviceRequest);
            }
            catch (WebServiceException e)
            {
                throw new UserFriendlySamException(e.Message);
            }
        }

        public GenericResponse<TestResultModel> GetTestResult(int testResultId)
        {
            var response = _testResultQueryService.GetTestResultById(testResultId);

            return _autoMapperHelper.Mapper.Map<TestResultModel>(response.Result);
        }

        public void DeleteDocument(int testSittingDocumentId)
        {
            _testResultQueryService.DeleteDocument(new DeleteDocumentRequest { TestSittingDocumentId = testSittingDocumentId });
        }

        public FileResponseModel GetDocument(int testSittingDocumentId)
        {
            var response = _testResultQueryService.GetDocument(new GetDocumentRequest { TestSittingDocumentId = testSittingDocumentId });
            return _fileService.GetData(response.Document.StoredFileId);
        }

        public GenericResponse<IEnumerable<TestSittingDocumentModel>> GetTestDocuments(int testSittingId)
        {
            var response = _testResultQueryService.GetDocuments(new GetDocumentsRequest { TestSittingId = testSittingId, DocumentTypes = new[] { StoredFileType.GeneralTestDocument } });
            var documents = response.Documents.Select(_autoMapperHelper.Mapper.Map<TestSittingDocumentModel>);
            var documentWithSoftDelete = new List<TestSittingDocumentModel>();
            foreach(var document in documents)
            {
                if(document.StoredFileStatusTypeId != 1)
                {
                    document.SoftDeleteDate = document.StoredFileStatusChangeDate;
                }
                documentWithSoftDelete.Add(document);
            }
            return new GenericResponse<IEnumerable<TestSittingDocumentModel>>(documentWithSoftDelete);
        }

        public GenericResponse<IEnumerable<ExaminerDocumentModel>> GetExaminerDocuments(int testSittingId)
        {
            var documentTypes = _fileService.GetDocumentTypesForCategory((int)DocumentTypeCategoryTypeName.Tests)
                .Select(x => (StoredFileType)Enum.Parse(typeof(StoredFileType), x.Name)).ToArray();

            var testDocumentsResponse = _testResultQueryService.GetDocuments(new GetDocumentsRequest
            {
                TestSittingId = testSittingId,
                DocumentTypes = documentTypes,
                ExaminerDocuments = true,                
            });
            var documents = testDocumentsResponse.Documents.Select(ToExaminerDocumentModel);

            var testMaterialIds = _testMaterialQueryService.GetExistingTestMaterialIdsByAttendees(new List<int> {testSittingId});
            var attachments = _testMaterialQueryService.GetTestMaterialsAttachments(testMaterialIds, true).Select(ToExaminerDocumentModel);

            var testSpecificationDocuments = _testSpecificationQueryService.GetExaminerAttachmentsForSitting(testSittingId).List.Select(ToExaminerDocumentModel);

            var allDocuments = documents.Concat(attachments).Concat(testSpecificationDocuments);

            return new GenericResponse<IEnumerable<ExaminerDocumentModel>>(allDocuments);
        }

        private ExaminerDocumentModel ToExaminerDocumentModel(TestMaterialAttachmentDto dto)
        {
            return new ExaminerDocumentModel
            {
                StoredFileId = dto.StoredFile.Id,
                Title = dto.Title,
                DocumentTypeDisplayName = dto.StoredFile.DocumentType.DisplayName,
                FileName = dto.StoredFile.FileName,
                FileSize = dto.StoredFile.FileSize,
                UploadedByPersonName = dto.StoredFile.UploadedByName,
                UploadedByUserName = dto.UploadedBy,
                UploadedDateTime = dto.UploadedDateTime,
                DocumentSource = "Test Material Attachment"
            };
        }

        private ExaminerDocumentModel ToExaminerDocumentModel(TestSittingDocumentDto dto)
        {
            return new ExaminerDocumentModel
            {
                StoredFileId = dto.StoredFileId,
                Title = dto.Title,
                DocumentTypeDisplayName = dto.DocumentTypeDisplayName,
                FileName = dto.FileName,
                FileSize = dto.FileSize,
                UploadedByPersonName = dto.UploadedByPersonName,
                UploadedByUserName = dto.UploadedByUserName,
                UploadedDateTime = dto.UploadedDateTime,
                DocumentSource = "Test Assets",  
                SoftDeleteDate = dto.StoredFileStatusTypeId != 1? dto.StoredFileStatusChangeDate:null
            };
        }

        private ExaminerDocumentModel ToExaminerDocumentModel(TestSpecificationAttachmentDto dto)
        {
            return new ExaminerDocumentModel
            {
                StoredFileId = dto.StoredFileId,
                Title = dto.FileName,
                DocumentTypeDisplayName = dto.DocumentType,
                FileName = dto.FileName,
                FileSize = dto.FileSize,
                UploadedByPersonName = dto.UploadedByName,
                UploadedByUserName = dto.UploadedByName,
                UploadedDateTime = dto.UploadedDateTime,
                DocumentSource = "Test Specification Document",
                SoftDeleteDate = dto.StoredFileStatusTypeId != 1 ? dto.StoredFileStatusChangeDate : null
            };
        }

        public GenericResponse<IEnumerable<TestSittingDocumentModel>> GetAssetDocuments(int testSittingId)
        {
            var documentTypes = _fileService.GetDocumentTypesForCategory((int) DocumentTypeCategoryTypeName.Tests)
                .Select(x=> (StoredFileType)Enum.Parse(typeof(StoredFileType), x.Name)).ToArray();

            var response = _testResultQueryService.GetDocuments(new GetDocumentsRequest
            {
                TestSittingId = testSittingId,
                DocumentTypes = documentTypes
            });
            var documents = response.Documents.Select(_autoMapperHelper.Mapper.Map<TestSittingDocumentModel>);
            var documentsWithSoftDelete = new List<TestSittingDocumentModel>();

            foreach(var document in documents)
            {
                if(document.StoredFileStatusTypeId != 1)
                {
                    document.SoftDeleteDate = document.StoredFileStatusChangeDate;
                }
                documentsWithSoftDelete.Add(document);
            }
            return new GenericResponse<IEnumerable<TestSittingDocumentModel>>(documentsWithSoftDelete);
        }

        public FileModel GetDocumentsAsZip(int testResultId)
        {
            var documents = GetAssetDocuments(testResultId).Data;

            var filesRequest = new GetFilesRequest { StoredFileIds = documents.Select(d => d.StoredFileId).ToArray(), TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] };
            var fileResponse = _fileStorageService.GetFiles(filesRequest);

            if (fileResponse == null)
            {
                throw new Exception("Null reponse from file storage service");
            }

            return new BulkFileDownloadModel
            {
                FileData = FileService.CreateZip(fileResponse.FilePaths),
                FileName = $"Test Sitting {DateTime.Now:ddMMyy HHmmss}.zip",
                FileType = FileType.Zip
            };
        }

        public int CreateOrUpdateDocument(CreateOrReplaceTestSittingDocumentModel request)
        {
            var document = _autoMapperHelper.Mapper.Map<CreateOrReplaceTestSittingDocumentDto>(request);

            document.UploadedByUserId = _userService.Get()?.Id ?? 0;

            var titleLastPart = document.Title.Split('.').Last();

            if (!string.Equals(("." + titleLastPart), Path.GetExtension(request.File)))
            {
                document.StoragePath = $@"{request.Type}\{request.TestSittingId}\{request.Title}{Path.GetExtension(request.File)}";
                document.Title = $@"{request.Title}{Path.GetExtension(request.File)}";
            }
            else
            {
                document.StoragePath = $@"{request.Type}\{request.TestSittingId}\{request.Title}";
                document.Title = $@"{request.Title}";
            }

            if (document.Title.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new UserFriendlySamException("A file name cannot contain any of the following characters: \\ / : * ? < > | []");
            }

            var allowedDocumentTypes = _fileService.GetAllowedDocumentTypesToUpload();
            var type = (StoredFileType)Enum.Parse(typeof(StoredFileType), document.Type);
            if (allowedDocumentTypes.All(x => x.Id != (int)type))
            {
                throw new UserFriendlySamException("You don''t have permission to upload these files");
            }

            var response = _testResultQueryService.CreateOrUpdateDocument(new CreateOrUpdateDocumentRequest
            {
                Document = document
            });

            return response.StoredFileId;
        }

        public void UpdateDueDate(UpdateDueDateRequestModel request)
        {
            _testQueryService.UpdateDueDate(new UpdateDueDateRequest { JobIds = request.JobIds, DueDate = request.DueDate });
        }

        public GenericResponse<bool> UpdateAutomaticIssuingExaminer(int testSittingId, bool? automaticIssuingExaminer)
        {
            var response = _testResultQueryService.UpdateTestResultAutomaticIssuingExaminer(testSittingId, automaticIssuingExaminer);

            return response.Data;
        }

        public bool? GetAutomaticIssuingExaminer(int testSittingId)
        {
            if (testSittingId > 1)
            {
                var automaticIssuingExaminer = _testResultQueryService.GetAutomaticIssuingExaminer(testSittingId);
                return automaticIssuingExaminer;
            }

            return null;
        }

        public bool GetTestSpecificationAutomaticIssuingByTestSittingId(int testSittingId)
        {
            var automaticIssuing = _testResultQueryService.GetTestSpecificationAutomaticIssuingByTestSittingId(testSittingId);

            return automaticIssuing;
        }
    }
}
