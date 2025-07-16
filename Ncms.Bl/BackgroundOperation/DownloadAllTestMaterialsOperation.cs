using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Words;
using Aspose.Words.Replacing;
using F1Solutions.Naati.Common.Bl.BackgroundOperation;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.NotificationScheduler;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Dal.Services;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundOperations;

namespace Ncms.Bl.BackgroundOperation
{
    public class DownloadAllTestMaterialsOperation : BaseBackgroundOperation, IDownloadAllTestMaterialsOperation
    {
        private readonly IFileCompressionHelper _fileCompressionHelper;
        private readonly ITestMaterialQueryService _testMaterialQueryService;
        private readonly ITestSessionQueryService _testSessionQueryService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ITokenReplacementService _tokenReplacementService;
        private readonly IList<int> _documentProtectionLevels;

        public DownloadAllTestMaterialsOperation(ISystemQueryService systemQueryService,
            IBackgroundTaskLogger backgroundTaskLogger,
            IUtilityQueryService utilityQueryService,
            IFileCompressionHelper fileCompressionHelper,
            ITestMaterialQueryService testMaterialQueryService,
            ITestSessionQueryService testSessionQueryService,
            IFileStorageService fileStorageService, 
            ITokenReplacementService tokenReplacementService,
            INotificationQueryService notificationQueryService,
            INotificationScheduler notificationScheduler
         ) : base(systemQueryService, backgroundTaskLogger, utilityQueryService, notificationQueryService, notificationScheduler)
        {
            _fileCompressionHelper = fileCompressionHelper;
            _testMaterialQueryService = testMaterialQueryService;
            _testSessionQueryService = testSessionQueryService;
            _fileStorageService = fileStorageService;
            _tokenReplacementService = tokenReplacementService;
            _documentProtectionLevels = new List<int>
            {
                (int)ProtectionType.AllowOnlyComments,
                (int)ProtectionType.AllowOnlyFormFields,
                (int)ProtectionType.AllowOnlyRevisions,
                (int)ProtectionType.ReadOnly
            };
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            if (!ValidateParameter(BackgroundOperationParameters.CreatedUserName, parameters) ||
                !ValidateParameter(BackgroundOperationParameters.TestSessionId, parameters) ||
                !ValidateParameter(BackgroundOperationParameters.IncludeExaminers, parameters))
            {
                return;
            }

            var testSessionId = Convert.ToInt32(parameters[BackgroundOperationParameters.TestSessionId]);
            var includeExaminer = Convert.ToBoolean(parameters[BackgroundOperationParameters.IncludeExaminers]);
            var userName = parameters[BackgroundOperationParameters.CreatedUserName];

            try
            {
                var filePath = BulkDownloadTestMaterial(testSessionId, includeExaminer);
                var testSession = _testSessionQueryService.GetTestSessionById(testSessionId);
                var parameter = new NotificationDownloadTestMaterialParameterDto
                {
                    TestSessionId = testSession.Result?.TestSessionId ?? 0,
                    TestSessionName = testSession.Result?.Name,
                    Path = filePath
                };

                var downloadExpiryText = GetSystemValue("DonwnloadTestMaterialExpirationHours");
                var expiryHours = Convert.ToInt32(downloadExpiryText);
                var notificationResponse = CreateNotification(NotificationTypeName.DownloadTestMaterial, userName, parameter, DateTime.Now.AddHours(expiryHours));
                SendNotification(notificationResponse.Data);
            }
            catch (UserFriendlySamException ex)
            {
                TaskLogger.WriteWarning(ex.ToString(), nameof(DownloadAllTestMaterialsOperation), testSessionId.ToString(), false);
            }
            catch (Exception exception)
            {
                TaskLogger.WriteError(exception, nameof(DownloadAllTestMaterialsOperation), testSessionId.ToString(), false);
                var parameter = new ErrorMessageParameterDto()
                {
                    Content = string.Format(Naati.Resources.Shared.ErrorCreatingTestMaterialFile, testSessionId),
                    Exception = exception.Message,
                };
                var errorExpiryText = GetSystemValue("DonwnloadTestMaterialErrorExpirationHours");
                var expiryHours = Convert.ToInt32(errorExpiryText);
                var notificationResponse = CreateNotification(NotificationTypeName.ErrorMessage, userName, parameter, DateTime.Now.AddHours(expiryHours));
                SendNotification(notificationResponse.Data);
            }
        }

        public string BulkDownloadTestMaterial(int testSessionId, bool includeExaminer)
        {
            var allCustomerAttendanceIdsList = GetAllCustomerAttendanceIdsList(testSessionId);

            var customerAttendanceIdList = new List<CustomerAttendanceIdDto>();

            foreach (var s in allCustomerAttendanceIdsList)
            {
                var customerAttendanceId = s.Split('-').ToList();
                customerAttendanceIdList.Add(new CustomerAttendanceIdDto
                {
                    CustomerNo = int.Parse(customerAttendanceId[0]),
                    AttendanceId = int.Parse(customerAttendanceId[1])
                });
            }

            var request = new GetAttendeesTestSpecificationTestMaterialRequest
            {
                CustomerAttendanceIdList = customerAttendanceIdList,
                IncludeExaminer = includeExaminer
            };

            var attendeeTestSpecificationTestMaterialList = _testMaterialQueryService.GetAttendeesTestSpecificationTestMaterialList(request);

            var response = DownloadAllMaterial(attendeeTestSpecificationTestMaterialList.AttendeeTestSpecificationTestMaterialList.ToList(), testSessionId);

            return response;
        }

        private IList<string> GetAllCustomerAttendanceIdsList(int testSessionId)
        {
            var response = _testMaterialQueryService.GetAllCustomerAttendanceIdsList(testSessionId);
            return response.ToList();
        }

        private string DownloadAllMaterial(List<AttendeeTestSpecificationTestMaterial> attendeeTestSpecificationTestMaterialList, int testSessionId)
        {
            var tempFolder = Path.Combine(ConfigurationManager.AppSettings["tempFilePath"], Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            var request = new GetFileStoreTestSpecificationTestMaterialRequest
            {
                AttendeeTestSpecificationTestMaterialList = attendeeTestSpecificationTestMaterialList,
                TempFileStorePath = tempFolder
            };
            GetAttendeesTestSpecificationTestMaterialResponse fileResponse = null;

            fileResponse = _fileStorageService.GetFileStoreTestSpecificationTestMaterialList(request);

            if (fileResponse == null)
            {
                throw new Exception("Null response from file storage service");
            }

            var fileList = GetFilesList(fileResponse.AttendeeTestSpecificationTestMaterialList.ToList(), testSessionId, fileResponse.IsMissingFiles);


            var zipFilePath = _fileCompressionHelper.CreateZipFile(fileList, $"Test Material TS{testSessionId}.zip", true);
            // var zipFileToken = _sharedAccessSignature.GetUrlForFile(zipFile);

            return zipFilePath;
        }

        private IEnumerable<CompressibleFile> GetFilesList(List<AttendeeTestSpecificationTestMaterial> attendeeTestSpecificationTestMaterialList, int testSessionId, bool isMissingFiles)
        {
            var files = new List<CompressibleFile>();

            if (attendeeTestSpecificationTestMaterialList.Count == 0 && isMissingFiles)
            {
                throw new UserFriendlySamException("Test material files are missing.");
            }

            if (attendeeTestSpecificationTestMaterialList.Count == 0)
            {
                throw new UserFriendlySamException("No assigned test materials have been found or the status is not confirmed.");
            }

            foreach (var attendeeTestSpecificationTestMaterial in attendeeTestSpecificationTestMaterialList)
            {
                var customerNumber = attendeeTestSpecificationTestMaterial.CustomerNumber;
                var attendanceId = attendeeTestSpecificationTestMaterial.AttendanceId;
                var hasExaminerAttachmentDocumentType = attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList.Where(x => x.StoredFileList.Any(y => y.DocumentTypeId == (int)DocumentTypeEnum.ExaminerTestMaterial)).ToList().Any();
                var folderPrefix = "Customer " + customerNumber + " - Attendance " + attendanceId + "/";
                var mergeDocumentListPerApplicant = new List<MergedDocument>();

                //Attendee test specification and test materials combined
                foreach (var attendeeTestMaterialFile in GetDownloadAttendeeTestMaterials(attendeeTestSpecificationTestMaterial))
                {
                    foreach (var storedFile in attendeeTestMaterialFile.StoredFileList)
                    {
                        if (storedFile.DocumentTypeId == (int)DocumentTypeEnum.CandidateBrief)
                            continue;

                        if (storedFile.MergeDocument)
                        {
                            mergeDocumentListPerApplicant = GetMergeDocumentListPerApplicant(mergeDocumentListPerApplicant, attendeeTestMaterialFile, storedFile);
                        }
                        else
                        {
                            var filename = GetAttendeeTestMaterialFileName(attendeeTestMaterialFile, storedFile, attendanceId);
                            var folder = hasExaminerAttachmentDocumentType ? storedFile.DocumentTypeId == (int)DocumentTypeEnum.ExaminerTestMaterial ? folderPrefix + "Examiner/" : folderPrefix + "Candidate/"
                                        : folderPrefix;

                            files.Add(new CompressibleFile() { FilePath = storedFile.FilePath, FileName = folder + filename });
                        }
                    }
                }

                if (mergeDocumentListPerApplicant.Count > 0)
                {
                    var filePath = ProcessAsposeMergeDocument(mergeDocumentListPerApplicant, attendanceId, testSessionId);

                    var filename = attendanceId + " - Test Material.docx";
                    var folder = hasExaminerAttachmentDocumentType ? folderPrefix + "Candidate/" : folderPrefix;
                    files.Add(new CompressibleFile() { FilePath = filePath, FileName = folder + filename });
                }
            }

            return files;
        }

        private List<DownloadAttendeeTestMaterial> GetDownloadAttendeeTestMaterials(AttendeeTestSpecificationTestMaterial attendeeTestSpecificationTestMaterial)
        {
            var downloadAttendeeTestMaterials = new List<DownloadAttendeeTestMaterial>();
            var attendeeTestMaterials = attendeeTestSpecificationTestMaterial.AttendeeTestMaterialList;
            var attendeeTestSpecification = attendeeTestSpecificationTestMaterial.AttendeeTestSpecification;

            var downloadAttendeeTestSpecification = new DownloadAttendeeTestMaterial
            {
                TestSpecificationId = attendeeTestSpecification.Id,
                IsTestSpecification = true,
                StoredFileList = attendeeTestSpecification.StoredFileList
            };
            downloadAttendeeTestMaterials.Add(downloadAttendeeTestSpecification);

            foreach (var attendeeTestMaterial in attendeeTestMaterials)
            {
                var downloadAttendeeTestMaterial = new DownloadAttendeeTestMaterial
                {
                    TestMaterialId = attendeeTestMaterial.Id,
                    TaskTypeLabel = attendeeTestMaterial.TaskTypeLabel,
                    TestComponentNumber = attendeeTestMaterial.TestComponentNumber,
                    Label = attendeeTestMaterial.Label,
                    StoredFileList = attendeeTestMaterial.StoredFileList,
                    IsTestSpecification = false
                };
                downloadAttendeeTestMaterials.Add(downloadAttendeeTestMaterial);
            }

            return downloadAttendeeTestMaterials;
        }

        private List<MergedDocument> GetMergeDocumentListPerApplicant(List<MergedDocument> mergeDocumentListPerApplicant, DownloadAttendeeTestMaterial attendeeTestMaterialFile, StoredFileMarterialDto storedFile)
        {
            if (attendeeTestMaterialFile.IsTestSpecification)
            {
                mergeDocumentListPerApplicant.Add(new MergedDocument
                {
                    TestSpecificationId = attendeeTestMaterialFile.TestSpecificationId,
                    IsTestSpecification = true,
                    StoredFile = storedFile
                });
            }
            else
            {
                mergeDocumentListPerApplicant.Add(new MergedDocument
                {
                    TestMaterialId = attendeeTestMaterialFile.TestMaterialId,
                    IsTestSpecification = false,
                    TaskTypeLabel = attendeeTestMaterialFile.TaskTypeLabel,
                    TestComponentNumber = attendeeTestMaterialFile.TestComponentNumber,
                    Label = attendeeTestMaterialFile.Label,
                    StoredFile = storedFile
                });
            }
            return mergeDocumentListPerApplicant;
        }

        private string GetAttendeeTestMaterialFileName(DownloadAttendeeTestMaterial attendeeTestMaterialFile, StoredFileMarterialDto storedFile, int attendanceId)
        {
            var filename = attendeeTestMaterialFile.IsTestSpecification ? attendanceId + " - TestSpec" + attendeeTestMaterialFile.TestSpecificationId + " - " + storedFile.Title + "." + storedFile.FileName.Split('.').Last() :
                attendanceId + " - " + attendeeTestMaterialFile.TaskTypeLabel + attendeeTestMaterialFile.Label + " - #" + attendeeTestMaterialFile.TestMaterialId + " - " + storedFile.Title + "." + storedFile.FileName.Split('.').Last();

            filename = storedFile.DocumentTypeId == (int)DocumentTypeEnum.ExaminerTestMaterial ? "Examiner Only - " + filename : filename;

            return filename;
        }

        private string ProcessAsposeMergeDocument(List<MergedDocument> mergeDocumentListPerApplicant, int attendanceId, int testSessionId)
        {
            var license = new License();
            license.SetLicense("Aspose.Words.lic");
            var dstDoc = new Document();

            dstDoc.RemoveAllChildren();
            var docProtectLevels = new List<int>();

            var mergeDocumentTokenReplacement = GetDocumentTokenReplacement(attendanceId, testSessionId);

            var merDocumentTestSpecification = mergeDocumentListPerApplicant.Find(x => x.IsTestSpecification);
            if (merDocumentTestSpecification != null)
            {
                var srcTestSpecificationDoc = new Document(merDocumentTestSpecification.StoredFile.FilePath);

                docProtectLevels.Add((int)srcTestSpecificationDoc.ProtectionType);

                srcTestSpecificationDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                srcTestSpecificationDoc.FirstSection.PageSetup.RestartPageNumbering = false;
                dstDoc.AppendDocument(srcTestSpecificationDoc, ImportFormatMode.KeepSourceFormatting);
                mergeDocumentTokenReplacement.TestSpecificationId = "SPEC" + merDocumentTestSpecification.TestSpecificationId;
            }

            var merDocumentTestMaterialListPerApplicant = mergeDocumentListPerApplicant.Where(x => !x.IsTestSpecification).OrderBy(z => z.TestComponentNumber).ThenBy(k => k.TestMaterialId).ToList();
            foreach (var merDocumentTestMaterial in merDocumentTestMaterialListPerApplicant)
            {
                var taskTypeTokenValue = merDocumentTestMaterial.TaskTypeLabel + merDocumentTestMaterial.Label;
                var srcTestMaterialDoc = new Document(merDocumentTestMaterial.StoredFile.FilePath);

                docProtectLevels.Add((int)srcTestMaterialDoc.ProtectionType);

                srcTestMaterialDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
                srcTestMaterialDoc.FirstSection.PageSetup.RestartPageNumbering = false;

                var testMaterialIdValue = "#" + merDocumentTestMaterial.TestMaterialId;
                var taskLabelToken = GetTokenNameFor(TokenReplacementField.TaskLabel);
                var testMaterialIdToken = GetTokenNameFor(TokenReplacementField.TestMaterialId);
                var extraTokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { testMaterialIdToken, testMaterialIdValue },
                            { taskLabelToken, taskTypeTokenValue }
                        };

                var taskLabeltokenReplaced = ReplaceTokenArray(extraTokens, srcTestMaterialDoc.Range.Text,
                    (token, value) =>
                    {
                        ReplaceTokenInDocument(srcTestMaterialDoc, token, value);
                    });

                dstDoc.AppendDocument(srcTestMaterialDoc, ImportFormatMode.KeepSourceFormatting);
            }

            var tempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"];
            var filePath = tempFileStorePath + "\\" + attendanceId + " - Test Material.docx";

            var tokensReplaced = ReplaceTokens(mergeDocumentTokenReplacement, dstDoc.Range.Text,
                (token, value) =>
                {
                    ReplaceTokenInDocument(dstDoc, token, value);
                });

            if (docProtectLevels.ToList().Any(x => _documentProtectionLevels.Contains(x)))
            {
                docProtectLevels = docProtectLevels.Where(x => x != (int)ProtectionType.NoProtection).ToList();
                var protectLevel = docProtectLevels.Min();

                switch (protectLevel)
                {
                    case ((int)ProtectionType.ReadOnly):
                        dstDoc.Protect(ProtectionType.ReadOnly);
                        break;
                    case ((int)ProtectionType.AllowOnlyRevisions):
                        dstDoc.Protect(ProtectionType.AllowOnlyRevisions);
                        break;
                    case ((int)ProtectionType.AllowOnlyFormFields):
                        dstDoc.Protect(ProtectionType.AllowOnlyFormFields);
                        break;
                    case ((int)ProtectionType.AllowOnlyComments):
                        dstDoc.Protect(ProtectionType.AllowOnlyComments);
                        dstDoc.RemovePersonalInformation = true;
                        break;
                }
            }

            dstDoc.UpdateFields();

            dstDoc.Save(filePath);

            return filePath;
        }

        //private bool ReplaceTaskLabelToken(string taskLabel, string text, Action<string, string> replacementAction)
        //{
        //    var taskLabelToken = GetTokenNameFor(TokenReplacementField.TaskLabel);

        //    var extraTokens =
        //        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //        {
        //            { taskLabelToken, taskLabel },
        //        };

        //    IEnumerable<string> errors;
        //    _tokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out errors);

        //    if (errors.Any())
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        private DocumentTokenReplacement GetDocumentTokenReplacement(int attendanceId, int testSessionId)
        {
            var mergeDocumentTokenReplacement = new DocumentTokenReplacement
            {
                AttendanceId = attendanceId.ToString(),
                TestTessionId = testSessionId.ToString()
            };
            var mergeDocumentAdditionalTokenValue = _testMaterialQueryService.GetDocumentAdditionalTokens(attendanceId);

            mergeDocumentTokenReplacement.ApplicationReference = mergeDocumentAdditionalTokenValue.ApplicationReference;
            mergeDocumentTokenReplacement.CredentialRequestType = mergeDocumentAdditionalTokenValue.CredentialRequestType;
            mergeDocumentTokenReplacement.Skill = mergeDocumentAdditionalTokenValue.Skill;
            mergeDocumentTokenReplacement.VenueName = mergeDocumentAdditionalTokenValue.VenueName;
            mergeDocumentTokenReplacement.TestSessionName = mergeDocumentAdditionalTokenValue.TestSessionName;
            mergeDocumentTokenReplacement.StrTestTessionDate = mergeDocumentAdditionalTokenValue.TestTessionDate;
            mergeDocumentTokenReplacement.StrTestTessionTime = mergeDocumentAdditionalTokenValue.TestTessionTime;

            return mergeDocumentTokenReplacement;
        }

        private bool ReplaceTokenArray(Dictionary<string, string> extraTokens, string text, Action<string, string> replacementAction)
        {
            _tokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out var errors);
            return !errors.Any();
        }

        private bool ReplaceTokens(DocumentTokenReplacement request, string text, Action<string, string> replacementAction)
        {
            var applicationReference = GetTokenNameFor(TokenReplacementField.ApplicationReference);
            var credentialRequestType = GetTokenNameFor(TokenReplacementField.CredentialRequestType);
            var skill = GetTokenNameFor(TokenReplacementField.Skill);
            var venueName = GetTokenNameFor(TokenReplacementField.VenueName);

            var testSessionId = GetTokenNameFor(TokenReplacementField.TestSessionId);
            var testSessionDate = GetTokenNameFor(TokenReplacementField.TestSessionDate);
            var testSessionStartTime = GetTokenNameFor(TokenReplacementField.TestSessionStartTime);
            var testAttendanceId = GetTokenNameFor(TokenReplacementField.TestAttendanceId);

            var testSessionName = GetTokenNameFor(TokenReplacementField.TestSessionName);
            var testSpecificationId = GetTokenNameFor(TokenReplacementField.TestSpecificationId);

            var extraTokens =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { applicationReference, request.ApplicationReference },
                    { credentialRequestType, request.CredentialRequestType },
                    { skill, request.Skill },
                    { venueName, request.VenueName },

                    { testAttendanceId, request.AttendanceId },
                    { testSessionId, request.TestTessionId },
                    { testSessionDate, request.StrTestTessionDate },
                    { testSessionStartTime, request.StrTestTessionTime },

                    { testSessionName, request.TestSessionName },
                    { testSpecificationId, request.TestSpecificationId }
                };

            IEnumerable<string> errors;
            _tokenReplacementService.ReplaceTemplateFieldValues(text, replacementAction, extraTokens, true, out errors);

            if (errors.Any())
            {
                return false;
            }

            return true;
        }

        private string GetTokenNameFor(TokenReplacementField token)
        {
            return typeof(TokenReplacementField)
                .GetMember(token.ToString())
                .First().GetCustomAttribute<DisplayAttribute>()
                .Name;
        }


        private void ReplaceTokenInDocument(Document document, string token, string value)
        {
            document.Range.Replace(token, SanitiseTokenValue(value) ?? string.Empty, new FindReplaceOptions());
        }

        private string SanitiseTokenValue(string tokenValue)
        {
            if (!String.IsNullOrEmpty(tokenValue))
            {
                // replace CRLF with something Aspose.Words understands, and remove excess whitespace and trailing commas
                var lines = tokenValue.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                return String.Join("&l", lines.Select(x => x.Trim().Trim(',')));
            }
            return tokenValue;
        }

    }
}
