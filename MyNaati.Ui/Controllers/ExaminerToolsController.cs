using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;
using F1Solutions.Naati.Common.Contracts.Dal.Portal.SystemValues;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Contracts.Security;
using MoreLinq;
using MyNaati.Bl.BackOffice.Helpers;
using MyNaati.Contracts.BackOffice;
using MyNaati.Contracts.BackOffice.MaterialRequest;
using MyNaati.Contracts.BackOffice.NcmsIntegration;
using MyNaati.Contracts.BackOffice.Panel;
using MyNaati.Contracts.Portal;
using MyNaati.Ui.Common;
using MyNaati.Ui.Helpers;
using MyNaati.Ui.UI;
using MyNaati.Ui.ViewModels.ExaminerTools;
using TestComponentModel = MyNaati.Ui.ViewModels.ExaminerTools.TestComponentModel;
using DeleteAttachmentRequest = MyNaati.Contracts.BackOffice.DeleteAttachmentRequest;
using DeleteMaterialRequest = MyNaati.Contracts.BackOffice.DeleteMaterialRequest;
using DeleteMaterialRequestRoundAttachmentRequest = MyNaati.Contracts.BackOffice.DeleteMaterialRequestRoundAttachmentRequest;
using DeleteUnavailabilityRequest = MyNaati.Contracts.BackOffice.DeleteUnavailabilityRequest;
using GetMaterialFileRequest = MyNaati.Contracts.BackOffice.GetMaterialFileRequest;
using GetMaterialRequestRoundAttachmentRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestRoundAttachmentRequest;
using GetMaterialRequestRoundAttachmentsRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestRoundAttachmentsRequest;
using GetMaterialRequestsRequest = MyNaati.Contracts.BackOffice.GetMaterialRequestsRequest;
using GetMembershipsRequest = MyNaati.Contracts.BackOffice.Panel.GetMembershipsRequest;
using GetPanelsRequest = MyNaati.Contracts.BackOffice.Panel.GetPanelsRequest;
using GetRolePlayerSettingsRequest = MyNaati.Contracts.BackOffice.GetRolePlayerSettingsRequest;
using GetRolePlaySessionRequest = MyNaati.Contracts.BackOffice.GetRolePlaySessionRequest;
using GetTestAssetsFileRequest = MyNaati.Contracts.BackOffice.GetTestAssetsFileRequest;
using GetTestAttendanceDocumentRequest = MyNaati.Contracts.BackOffice.GetTestAttendanceDocumentRequest;
using GetTestDetailsRequest = MyNaati.Contracts.BackOffice.GetTestDetailsRequest;
using GetTestMaterialRequest = MyNaati.Contracts.BackOffice.GetTestMaterialRequest;
using GetTestMaterialsFileRequest = MyNaati.Contracts.BackOffice.GetTestMaterialsFileRequest;
using GetTestsMaterialRequest = MyNaati.Contracts.BackOffice.GetTestsMaterialRequest;
using GetTestsRequest = MyNaati.Contracts.BackOffice.GetTestsRequest;
using ListUnavailabilityRequest = MyNaati.Contracts.BackOffice.ListUnavailabilityRequest;
using MaterialRequestRoundAttachmentDto = MyNaati.Contracts.BackOffice.MaterialRequestRoundAttachmentDto;
using RolePlayerSettingsDto = MyNaati.Contracts.BackOffice.RolePlayerSettingsDto;
using RolePlayerSettingsRequest = MyNaati.Contracts.BackOffice.RolePlayerSettingsRequest;
using SaveAttachmentRequest = MyNaati.Contracts.BackOffice.SaveAttachmentRequest;
using SaveMaterialRequest = MyNaati.Contracts.BackOffice.SaveMaterialRequest;
using SaveMaterialRequestRoundLinkRequest = MyNaati.Contracts.BackOffice.SaveMaterialRequestRoundLinkRequest;
using SaveUnavailabilityRequest = MyNaati.Contracts.BackOffice.SaveUnavailabilityRequest;
using SubmitMaterialRequest = MyNaati.Contracts.BackOffice.SubmitMaterialRequest;
using SubmitTestRequest = MyNaati.Contracts.BackOffice.SubmitTestRequest;
using TestComponentContract = MyNaati.Contracts.BackOffice.TestComponentContract;
using UpdateMaterialRequestMembersRequest = MyNaati.Contracts.BackOffice.UpdateMaterialRequestMembersRequest;
using MyNaati.Ui.ViewModels.Account;

namespace MyNaati.Ui.Controllers
{
    [Authorize]
    [RoutePrefix("examinertools")]
    public class ExaminerToolsController : BaseController
    {
        private readonly IExaminerToolsService mExaminerToolsService;
        private readonly ISystemValueService mSystemValueService;
        private readonly IMaterialRequestService mMaterialRequestService;
        private readonly INcmsIntegrationService _ncmsService;
        private readonly IFileCompressionHelper _fileCompressionHelper;
        private readonly IExaminerToolsInternalService mExaminerToolsInternalService;
        private readonly IPanelMembershipService mPanelService;
        private readonly IExaminerHelper mExaminerHelper;
        private ILookupProvider mLookupProvider;
        private ISharedAccessSignature mSharedAccessSignature;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly F1Solutions.Naati.Common.Contracts.Dal.QueryServices.ISystemQueryService _systemQueryService;

        public ExaminerToolsController(IExaminerToolsService examinerToolsService, IExaminerToolsInternalService examinerToolsInternalService, IPanelMembershipService panelService, IExaminerHelper examinerHelper, ILookupProvider lookupProvider, ISystemValueService systemValueService, IMaterialRequestService materialRequestService, INcmsIntegrationService ncmsService, IFileCompressionHelper fileCompressionHelper, ISharedAccessSignature mSharedAccessSignature, IAutoMapperHelper autoMapperHelper, F1Solutions.Naati.Common.Contracts.Dal.QueryServices.ISystemQueryService systemQueryService)
        {
            mExaminerToolsService = examinerToolsService;
            mExaminerToolsInternalService = examinerToolsInternalService;
            mPanelService = panelService;
            mExaminerHelper = examinerHelper;
            mLookupProvider = lookupProvider;
            mSystemValueService = systemValueService;
            mMaterialRequestService = materialRequestService;
            _ncmsService = ncmsService;
            _fileCompressionHelper = fileCompressionHelper;
            this.mSharedAccessSignature = mSharedAccessSignature;
            _autoMapperHelper = autoMapperHelper;
            _systemQueryService = systemQueryService;
        }

        public ActionResult Index()
        {
            var model = new IndexModel();
            model.RolePlayerAvailable = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "RolePlayerAvailable" }).Value;
            return View(model);
        }

        public ActionResult ManageTests()
        {
            // to be enabled in a later release
            //var mfaConfigurationAndEmailCodeActiveStatus = GetMfaConfigurationAndEmailCodeActiveStatus(CurrentUserNaatiNumber);
            //if (!mfaConfigurationAndEmailCodeActiveStatus.MfaConfiguredAndActiveOrEmailCodeActive)
            //{
            //    return View("_MfaAndAccessCodePartial", new MfaAndAccessCodeModel()
            //    {
            //        MfaConfigured = false,
            //        MfaConfiguredAndActiveOrEmailCodeActive = false,
            //        ReturnController = "ExaminerTools",
            //        ReturnView = "ManageTests"
            //    });
            //}

            return View(new ManageTestsModel
            {
                TestList = GetTests(false)
            });
        }

        [HttpPost]
        public ActionResult Tests()
        {
            var gridData = GetTests(false);
            var count = gridData.Count;
            var pageCount = Math.Ceiling((double)count / 10);

            var finalResult = new
            {
                Results = gridData.ToList(),
                TotalResultsCount = count,
                TotalPageCount = pageCount,
                PageNumber = 1,
                PageSize = 10
            };

            return Json(new
            {
                Data = finalResult,
                IsError = false,
                Message = ""
            });
        }

        /*
         * This Action Method is being called from SubmitNewTestMaterial which is not being used anymore
         */
        [HttpPost]
        public ActionResult TestMaterials()
        {
            var searchResults = GetTestsMaterial().Select(x => new
            {
                //AssetsCount = x.HasAssets, 
                x.Category,
                x.Direction,
                DueDate = x.DueDate?.ToString("d/MM/yyyy"),
                x.JobExaminerID,
                x.JobID,
                x.SkillDisplayName,
                x.Level,
                x.MaterialID,
                x.Materials,
                x.Status,
                TestDate = x.TestDate.ToString("d/MM/yyyy"),
                x.TestSittingId,
                x.TestMaterialID,
                x.TestResultID
            }).ToList();

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = searchResults.Count,
                recordsFiltered = searchResults.Count,
                data = searchResults
            });
        }

        [HttpGet]
        public ActionResult SubmitTestResults(int? id, string returnElement = null)
        {
            var examinerAllocatedTestSittings = GetTests(false).Select(x => x.TestSittingId);

            var testsittingId = (int)mExaminerToolsService.GetTestSittingIdByTestResult(id.GetValueOrDefault());
            if (!examinerAllocatedTestSittings.Contains(testsittingId))
            {
                return Redirect(Url.Action("ManageTests"));
                //throw new MyNaatiSecurityException();
            }

            var model = new SubmitTestModel { TestResultID = id, ReturnElement = returnElement };
            var components = new List<TestComponentModel>();
            var naatiNumber = User.NaatiNumber();
            var details = mExaminerToolsService.GetTestDetails(new GetTestDetailsRequest
            {
                TestResultId = id.Value,
                NaatiNumber = naatiNumber
            });

            model.Feedback = details.Feedback;

            model.TestType = (TestType)details.TestMarkingTypeId;

            if (id.HasValue && model.TestType == TestType.Standard)
            {
                var draft = mExaminerToolsInternalService.GetTest(new GetTestRequest
                {
                    UserID = naatiNumber,
                    TestID = id.Value
                });

                var tests = GetTests(false);

                var selectedTest = tests.FirstOrDefault(x => x.TestResultID == id);

                if (selectedTest == null)
                {
                    return Redirect(Url.Action("ManageTests"));
                }

                model.OverallPassMark = details.OverAllPassMark;
                model.LanguageName = selectedTest.SkillDisplayName;

                var hasDraft = draft != null;

                if (hasDraft)
                {
                    model.Comments = draft.Comments;
                    model.Feedback = draft.Feedback;
                    model.ReasonsForPoorPerformance = draft.ReasonsForPoorPerformance;
                    model.PrimaryReasonForFailure = draft.PrimaryReasonForFailure;
                }

                foreach (var c in details.Components)
                {
                    var component = _autoMapperHelper.Mapper.Map<TestComponentModel>(c);

                    if (hasDraft && !component.ReadOnly)
                    {
                        var componentDraft = draft.Components.FirstOrDefault(com => com.Id == c.Id);
                        if (componentDraft != null)
                        {
                            component.Mark = componentDraft.Mark;
                        }
                    }

                    components.Add(component);
                }
            }

            model.Components = components;
            if (!GetRubrickMarkingsIfAvailable(model))
            {
                return Redirect(Url.Action("ManageTests"));
            }
            CompleteModel(model);

            var viewName = model.TestType == TestType.Standard ? "StandardTest" : "RubricTest";

            return View(viewName, model);
        }

        private bool GetRubrickMarkingsIfAvailable(SubmitTestModel model)
        {
            if (model.TestType != TestType.Rubric)
            {
                return true;
            }

            var naatiNumber = User.NaatiNumber();
            var rubrickMarking = mExaminerToolsService.GetExaminerRubricMarking(naatiNumber, model.TestResultID.GetValueOrDefault());

            if (rubrickMarking.SubmittedDate.HasValue)
            {
                return false;
            }
            model.Components = rubrickMarking.TestComponents.Select(_autoMapperHelper.Mapper.Map<TestComponentModel>).ToList();
            return true;
        }

        public ActionResult DeleteAttachment(int id)
        {
            if (!mExaminerToolsService.IsValidAttendeeDeleteAttachment(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            mExaminerToolsService.DeleteAttachment(new DeleteAttachmentRequest { TestAttendanceDocumentId = id });
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitTestResults(SubmitTestModel request)
        {
            SaveTest(request);

            var submit = request.Action == "Submit";
            if (submit)
            {

                if (request.TestType == TestType.Standard)
                {
                    if (!ModelState.IsValid)
                    {
                        FixValidationMessages(request);
                        return SubmitTestResults(request.TestResultID, request.ReturnElement);
                    }

                    if (ValidateExceededMarks(request))
                    {
                        return SubmitTestResults(request.TestResultID, request.ReturnElement);
                    }
                }

                var hasAttachments = false;
                var naatiNumber = User.NaatiNumber();

                if (request.Attachments == null)
                {
                    var attachments = mExaminerToolsService.GetTestDetails(new GetTestDetailsRequest
                    {
                        TestResultId = request.TestResultID.GetValueOrDefault(),
                        NaatiNumber = naatiNumber
                    }).Attachments;

                    hasAttachments = attachments.Any();
                }

                if (hasAttachments)
                {
                    var testResultId = request.TestResultID;

                    mExaminerToolsService.SaveExaminerPapersRecievedDateRequested(new SaveExaminerPapersRecievedDateRequest
                    {
                        NAATINumber = naatiNumber,
                        TestResultId = testResultId.GetValueOrDefault()
                    });
                }

                SubmitTest(request);
                if (ModelState.IsValid)
                {
                    TempData["AlertMessage"] = "The test were submitted successfully.";

                    return RedirectToAction("ManageTests");
                }

                return SubmitTestResults(request.TestResultID, request.ReturnElement);
            }

            ModelState.Clear();
            TempData["AlertMessage"] = "Saved successfully.";

            return SubmitTestResults(request.TestResultID, request.ReturnElement);
        }

        public ActionResult AdviseAvailability()
        {
            var model = new AdviseAvailabilityModel();
            if (User.IsRolePlayer())
            {
                model.Locations = mLookupProvider.TestLocations.Select(t => new SelectListItem { Value = t.SamId.ToString(), Text = t.DisplayText });
                var response = mExaminerToolsService.GetRolePlayerSettings(new GetRolePlayerSettingsRequest { NaatiNumber = User.NaatiNumber() });
                model.RolePlayerSettings = _autoMapperHelper.Mapper.Map<RolePlayerSettingsModel>(response.Settings) ?? new RolePlayerSettingsModel();
                model.RolePlayerAvailable = _systemQueryService.GetSystemValue(new GetSystemValueRequest() { ValueKey = "RolePlayerAvailable" }).Value;
            }
            return View(model);
        }

        public ActionResult RolePlaySessions()
        {
            return View();
        }

        public ActionResult RolePlaySession(int id)
        {
            var response = mExaminerToolsService.GetRolePlaySession(new GetRolePlaySessionRequest { TestSessionRolePlayerId = id, NaatiNumber = CurrentUserNaatiNumber });
            var sessions = response.Sessions;
            return View(response.Sessions.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult RolePlaySession(GetRolePlaySessionRequest request)
        {
            request.NaatiNumber = CurrentUserNaatiNumber;
            var obj = new NcmsRolePlayerActionRequest { TestSessionRolePlayerId = request.TestSessionRolePlayerId, ActionId = request.ActionId };

            var response = _ncmsService.ExecuteRolePlayerAction(obj);
            if (!response.Success)
            {
                throw new Exception(string.Concat(",", response.Errors));
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Sessions()
        {
            var response = mExaminerToolsService.GetRolePlaySession(new GetRolePlaySessionRequest { NaatiNumber = CurrentUserNaatiNumber });
            var sessions = response.Sessions;

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = sessions.Count,
                recordsFiltered = sessions.Count,
                data = sessions
            });
        }

        [HttpPost]
        public ActionResult Availabilities()
        {
            var response = mExaminerToolsService.ListUnavailability(new ListUnavailabilityRequest
            {
                NAATINumber = User.NaatiNumber()
            });

            var periods = response.Periods.Select(x => new
            {
                x.Id,
                StartDate = x.StartDate.ToString("d/MM/yyyy"),
                EndDate = x.EndDate.ToString("d/MM/yyyy")
            }).ToList();

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = periods.Count,
                recordsFiltered = periods.Count,
                data = periods
            });
        }

        [HttpPost]
        public void DeleteAdviseAvailability(int id)
        {
            if (!mExaminerToolsService.IsValidExaminerForAvailability(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }
            mExaminerToolsService.DeleteUnavailability(new DeleteUnavailabilityRequest { Id = id });
        }

        [HttpPost]
        public ActionResult AdviseAvailability(AdviseAvailabilityModel request)
        {
            var data = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var endDate = request.EndDate.GetValueOrDefault();
                var startDate = request.StartDate.GetValueOrDefault();
                var valid = true;

                if (endDate.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("EndDate", $"The end date must be greater than or equal to the current date ({DateTime.Now.Date:dd/MM/yyyy}).");
                    valid = false;
                }
                if (startDate.Date > endDate.Date)
                {
                    ModelState.AddModelError("StartDate", $"The start date cannot be greater than or equal to the end date ({endDate.Date:dd/MM/yyyy}).");
                    valid = false;
                }

                if (valid)
                {
                    var response = mExaminerToolsService.ListUnavailability(new ListUnavailabilityRequest
                    {
                        NAATINumber = User.NaatiNumber()
                    });

                    var isOverlaping = response.Periods.Any(p =>
                    {
                        bool overlap = startDate < p.EndDate && p.StartDate < endDate;
                        var isTheSame = p.Id == request.Id;

                        return !isTheSame && overlap;
                    });

                    if (isOverlaping)
                    {
                        ModelState.AddModelError("StartDate", $"This period overlaps with an existing period.");
                        valid = false;
                    }
                }

                if (valid)
                {
                    mExaminerToolsService.SaveUnavailability(new SaveUnavailabilityRequest
                    {
                        Id = request.Id,
                        NAATINumber = User.NaatiNumber(),
                        StartDate = request.StartDate.GetValueOrDefault(),
                        EndDate = endDate
                    });

                    data.Success = true;
                }
            }

            if (!data.Success)
            {
                data.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            TempData["AlertMessage"] = "Period added successfully.";
            return Json(data);
        }

        [HttpPost]
        public ActionResult RolePlayerSettings(RolePlayerSettingsModel request)
        {
            var data = new JsonUpdateResponse { Success = false };

            if (ModelState.IsValid)
            {
                var valid = true;
                if (request.RolePlayLocations == null || request.RolePlayLocations.Length == 0)
                {
                    ModelState.AddModelError("RolePlayLocations", $"The Locations are required.");
                    valid = false;
                }
                if (request.MaximumRolePlaySessions < 1 || request.MaximumRolePlaySessions > 40)
                {
                    ModelState.AddModelError("MaximumRolePlaySessions", $"Maximum Sessions must be greater than 1 and less than 40.");
                    valid = false;
                }

                if (valid)
                {
                    mExaminerToolsService.SaveRolePlayerSettings(new RolePlayerSettingsRequest
                    {
                        Settings = new RolePlayerSettingsDto
                        {
                            NaatiNumber = CurrentUserNaatiNumber,
                            MaximumRolePlaySessions = request.MaximumRolePlaySessions,
                            RolePlayLocations = request.RolePlayLocations
                        }
                    });

                    data.Success = true;
                }
            }

            if (!data.Success)
            {
                data.Errors = ViewData.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            TempData["AlertMessage"] = "Role-player Settings saved successfully.";
            return Json(data);
        }

        //public ActionResult SubmitNewTestMaterial()
        //{
        //	var model = new ManageTestsModel();
        //	return View(model);
        //}

        public ActionResult EditTestMaterial(int id)
        {
            return View(id);
        }

        [HttpPost]
        public ActionResult Materials(int id)
        {
            var request = new GetTestMaterialRequest { TestMaterialId = id, NAATINumber = User.NaatiNumber() };

            var response = mExaminerToolsService.GetTestMaterial(request);

            var material = _autoMapperHelper.Mapper.Map<TestMaterialContract, TestModel>(response);
            var materials = material.Materials;

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = materials.Count,
                recordsFiltered = materials.Count,
                data = materials
            });
        }

        [HttpPost]
        public ActionResult Attachments(int id)
        {
            var details = mExaminerToolsService.GetTestDetails(new GetTestDetailsRequest
            {
                TestResultId = id,
                NaatiNumber = User.NaatiNumber()
            });

            var attachments = details.Attachments;

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = attachments.Length,
                recordsFiltered = attachments.Length,
                data = attachments
            });
        }

        [HttpPost]
        public void SubmitNewTestMaterial(int id)
        {
            mExaminerToolsService.SubmitMaterial(new SubmitMaterialRequest { JobExaminerId = id });
        }

        [HttpGet]
        public void DeleteMaterialFile(int id)
        {
            mExaminerToolsService.DeleteMaterial(new DeleteMaterialRequest { MaterialId = id, NAATINumber = User.NaatiNumber() });
        }

        [HttpPost]
        public ActionResult DownloadTestMaterials(int id)
        {
            var file = mExaminerToolsService.GetTestMaterialsFile(new GetTestMaterialsFileRequest { TestMaterialId = id, NAATINumber = User.NaatiNumber(), TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] });
            Response.BufferOutput = false;

            try
            {
                var compressedFile = _fileCompressionHelper.CreateZipFile(file.FilePaths, $"TestMaterial_{id}");
                return Redirect(mSharedAccessSignature.GetUrlForFile(compressedFile));
            }
            catch (IOException)
            {
                return Redirect(Url.Action("FileTooLarge"));
            }

    
        }

        [HttpPost]
        public ActionResult DownloadMaterialFile(int id)
        {
            var file = mExaminerToolsService.GetMaterialFile(new GetMaterialFileRequest { MaterialId = id, TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] });
            return new FilePathResult(file.FilePaths[0], MimeMapping.GetMimeMapping(file.FilePaths[0]));
        }

        [HttpPost]
        public ActionResult DownloadAttachment(int id)
        {
            if (!mExaminerToolsService.IsValidAttendeeDeleteAttachment(id, CurrentUserNaatiNumber))
            {
                throw new MyNaatiSecurityException();
            }

            var file = mExaminerToolsService.GetTestAttendanceDocument(new GetTestAttendanceDocumentRequest { TestAttendanceDocumentId = id, TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] });
            return new FilePathResult(file.FilePaths[0], MimeMapping.GetMimeMapping(file.FilePaths[0]));
        }

        [HttpGet]
        public ActionResult DownloadTestAssets()
        {
            return RedirectToAction("ManageTests");
        }

        [HttpPost]
        public ActionResult DownloadTestAssets(int id)
        {
            var examinerAllocatedTestSittings = GetTests(false).Select(x => x.TestSittingId);
            if (!examinerAllocatedTestSittings.Contains(id))
            {
                throw new MyNaatiSecurityException();
            }

            List<string> filePaths = new List<string>();

            var testSpecificationTestMaterials = mExaminerToolsService.GetTestMaterialsByAttendaceId(id);
            var tempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"];

            var request = new GetFileStoreTestSpecificationTestMaterialRequest
            {
                AttendeeTestSpecificationTestMaterialList = testSpecificationTestMaterials.AttendeeTestSpecificationTestMaterialList.ToList(),
                TempFileStorePath = tempFileStorePath,
                AttendanceId = id
            };

            GetAttendeesTestSpecificationTestMaterialResponse fileResponse = null;
            fileResponse = mExaminerToolsService.GetFileStoreTestSpecificationTestMaterialList(request);
            if (fileResponse == null)
            {
                throw new Exception("Null response from file storage service");
            }

            var testMaterialFilePaths = mExaminerToolsService.TestMaterialReplaceTokens(fileResponse, tempFileStorePath);
            filePaths.AddRange(testMaterialFilePaths.ToArray());

            var file = mExaminerToolsService.GetTestAssetsFile(new GetTestAssetsFileRequest { TestSittingId = id, TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"] });

            filePaths.AddRange(file.FilePaths);

            try
            {
                var compressedFile = _fileCompressionHelper.CreateZipFile(filePaths, $"TestMaterial_{id}");
                return Redirect(mSharedAccessSignature.GetUrlForFile(compressedFile));
                //return new FilePathResult(compressedFile, MimeMapping.GetMimeMapping(compressedFile));
            }
            catch (IOException)
            {
                return Redirect(Url.Action("FileTooLarge"));
            }
        }

        [HttpPost]
        public ActionResult SubmitNewTestMaterialFile()
        {
            var ids = new List<int>();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
                {
                    continue;
                }

                var testMaterialId = Convert.ToInt32(Request["id"]);
                string path;

                using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + file.FileName))
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(fileStream);
                    path = fileStream.Name;
                }

                var request = new SaveMaterialRequest
                {
                    NAATINumber = User.NaatiNumber(),
                    FilePath = path,
                    Title = file.FileName,
                    TestMaterialId = testMaterialId
                };

                var response = mExaminerToolsService.SaveMaterial(request);
                ids.Add(response.TestMaterialId);
            }

            return Json(new { success = true, id = ids });
        }

        [HttpPost]
        public ActionResult SubmitTestResultAttachment()
        {
            var naatiNumber = User.NaatiNumber();
            var ids = new List<int>();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                if (file == null)
                {
                    continue;
                }

                var fileName = Path.GetFileName(file.FileName);

                if (string.IsNullOrWhiteSpace(fileName) ||
                    !ApplicationSettingsHelper.IncludedFileExtensionsList.Contains(fileName.Split('.').Last().ToLower()) ||
                    file.ContentLength == 0)
                {
                    continue;
                }

                string path;

                using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + file.FileName))
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(fileStream);
                    path = fileStream.Name;
                }

                var saveAttachmentRequest = new SaveAttachmentRequest
                {
                    FilePath = path,
                    NAATINumber = naatiNumber,
                    TestResultId = Convert.ToInt32(Request["id"]),
                    Title = Path.GetFileName(file.FileName),
                    Type = int.Parse(Request.RawUrl.Split('/').Last()),
                };

                var response = mExaminerToolsService.SaveAttachment(saveAttachmentRequest);
                ids.Add(response.TestAttendanceDocumentId);
            }

            return Json(new { success = ids.Any(), id = ids, error = ids.Any() ? "" : "Unsupported File type" });
        }

        public ActionResult PanelManagement()
        {
            var model = new PanelManagementModel
            {
                DateDueFrom = DateTime.Now,
                Panels = new List<SelectListItem>(),
                People = new List<SelectListItem>(),
                Statuses = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Submitted", Text = "Submitted" },
                    new SelectListItem { Value = "InProgress", Text = "In Progress", Selected = true }
                }
            };

            var naatiNumber = User.NaatiNumber();

            var panelRequest = new GetPanelsRequest
            {
                NAATINumber = naatiNumber,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner },
                Chair = true,
                IsVisibleInEportal = true
            };

            var panels = mPanelService.GetPanels(panelRequest).Panels;
            var peopleList = new List<SelectListItem>();
            foreach (var p in panels)
            {
                var people = mPanelService.GetMemberships(new GetMembershipsRequest
                {
                    PanelId = p.PanelId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now
                }).People;

                peopleList.AddRange(people.Select(pe => new SelectListItem { Text = pe.Name, Value = pe.NAATINumber.ToString() }));
                model.Panels.Add(new SelectListItem { Text = p.Name, Value = p.PanelId.ToString() });
            }

            model.People.AddRange(peopleList.DistinctBy(s => s.Value));

            if (panels.Length == 1)
            {
                model.Panels[0].Selected = true;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult PanelManagement(PanelManagementModel model)
        {
            //  Get the reigistration requests from the ePortal database.
            var criteria = new GetTestsRequest
            {
                NAATINumber = model.NAATINumber,
                PanelId = model.PanelId,
                DateDueFrom = model.DateDueFrom,
                DateDueTo = model.DateDueTo,
                DateAllocatedFrom = model.DateAllocatedFrom,
                DateAllocatedTo = model.DateAllocatedTo,
                TestStatus = model.TestStatus,
                AsChair = true,
                UserId = User.NaatiNumber(),
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner }.ToArray()
            };

            var message = ValidateSearch(criteria);

            if (!string.IsNullOrWhiteSpace(message))
            {
                var noResult = new
                {
                    PageNumber = 0,
                    PageSize = 0,
                    Results = new List<TestContract>(),
                    TotalResultsCount = 0
                };

                return Json(new
                {
                    Data = noResult,
                    IsError = true,
                    Message = message
                });
            }

            var searchResults = mExaminerToolsService.GetTests(criteria);

            //  start return the combined results to the page.
            var data = (from x in searchResults.Tests
                        select new
                        {
                            x.Examiner,
                            x.TestSittingId,
                            x.Description,
                            x.Supplementary,
                            DateAllocated = x.DateAllocated.ToString("d/MM/yyyy"),
                            DueDate = x.DueDate?.ToString("d/MM/yyyy") ?? string.Empty,
                            Status = x.Status == "InProgress" ? "In Progress" : x.Status,
                            Overdue = x.DueDate.HasValue && DateTime.Now > x.DueDate.Value
                        }).ToList();

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = data.Count,
                recordsFiltered = data.Count,
                data
            });
        }

        private const string TestMaterialBeingReviewedByNaati = "TestMaterialBeingReviewedByNaati";
        private const string PaymentBeingProcessedByNAATI = "PaymentBeingProcessedByNAATI";
        private const string Paid = "Paid";

        private const string AwaitingNAATIsApproval = "Awaiting NAATI's Approval";
        private const string Approved = "Approved";
        private const string Rejected = "Rejected";
        private const string Cancelled = "Cancelled";

        public ActionResult TestMaterialCreationPayments()
        {
            var model = new TestMaterialCreationPaymentsModel
            {
                Statuses = new List<SelectListItem> {
                    new SelectListItem { Value = TestMaterialBeingReviewedByNaati, Text = "Test material being reviewed by NAATI" },
                    new SelectListItem { Value = PaymentBeingProcessedByNAATI, Text = "Payment Being processed by NAATI" },
                    new SelectListItem { Value = Paid, Text = "Paid", Selected = true }
                }
            };

            return View(model);
        }
        private static string GetStatusFromTestMaterialRecord(TestMaterialCreationPaymentContract testMaterialPayment)
        {
            if(testMaterialPayment.PaymentApprovedDate != null && testMaterialPayment.PaymentProcessedDate != null && testMaterialPayment.InvoiceNo != null)
            {
                return Paid;
            }
            if ((testMaterialPayment.PaymentApprovedDate != null && testMaterialPayment.PaymentProcessedDate == null)|| testMaterialPayment.CurrentStatus == Approved)
            {
                return PaymentBeingProcessedByNAATI;
            }
            if (testMaterialPayment.CurrentStatus == AwaitingNAATIsApproval)
            {
                return TestMaterialBeingReviewedByNaati;
            }
            return string.Empty;
        }

        [HttpPost]
        public ActionResult TestMaterialCreationPayments(TestMaterialCreationPaymentsModel model)
        {
            var success = false;

            if (ModelState.IsValid)
            {
                var message = ValidateDateFromAndTo(model.SubmittedFrom, model.SubmittedTo, "submitted");

                if (!string.IsNullOrEmpty(message))
                {
                    ModelState.AddModelError("SubmittedFrom", message);
                }
                else
                {
                    success = true;
                }
            }

            if (!success)
            {
                return Json(new
                {
                    draw = Convert.ToInt32(Request.Form.Get("draw")),
                    totalRecords = 0,
                    recordsFiltered = 0,
                    data = new string[] { },
                    IsError = true,
                    Errors = ViewData.ModelState.Values
                        .SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var payments =
                mExaminerToolsService.GettestMaterialCreationPayments(new GetTestMaterialCreationPaymentsRequest()
                {
                    ExaminerNaatiNumber = User.NaatiNumber().ToString()
                }).Payments.ToArray();

            //before filtering, compute total invoice

            //take out cancelled and rejected
            payments = payments.Where(x => x.CurrentStatus != Cancelled).ToArray();
            payments = payments.Where(x => x.CurrentStatus != Rejected).ToArray();

            //total payments
            payments.ForEach(x => x.TotalInvoice = ComputeTotal(payments, x));

            int testMaterialIdId;
            if (model.TestMaterialID != null && int.TryParse(model.TestMaterialID, out testMaterialIdId))
            {
                payments = payments.Where(x => x.TestMaterialId == testMaterialIdId).ToArray();
            }
            if (model.SubmittedFrom != null)
            {
                payments = payments.Where(x => x.MaterialCreationSubmittedDate >= model.SubmittedFrom).ToArray();
            }
            if (model.SubmittedTo != null)
            {
                payments = payments.Where(x => x.MaterialCreationSubmittedDate <= model.SubmittedTo.Value.AddDays(1)).ToArray();
            }
            if (model.PaymentStatus != null)
            {
                payments =
                    payments.Where(x => model.PaymentStatus.Contains(GetStatusFromTestMaterialRecord(x)))
                        .ToArray();
            }

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = payments.Length,
                recordsFiltered = payments.Length,
                data = payments.Select(x =>
                {
                    return new
                    {
                        AmountPaid = x.AmountPaid == 0 ? "" : $"${String.Format("{0:0.00}",x.AmountPaid)}",
                        x.CredentialType,
                        x.CurrentStatus,
                        x.InvoiceNo,
                        MaterialCreationSubmittedDate = x.MaterialCreationSubmittedDate.HasValue ? x.MaterialCreationSubmittedDate.Value.ToShortDateString() : "",
                        PaymentApprovedDate = x.PaymentApprovedDate.HasValue ? x.PaymentApprovedDate.Value.ToShortDateString() : "",
                        PaymentProcessedDate = x.PaymentProcessedDate.HasValue ? x.PaymentProcessedDate.Value.ToShortDateString() : "",
                        x.Skill,
                        x.TestMaterialId,
                        TotalInvoice = $"${String.Format("{0:0.00}", x.TotalInvoice)}"
                    };
                })
            });
        }

        private decimal ComputeTotal(TestMaterialCreationPaymentContract[] payments, TestMaterialCreationPaymentContract payment)
        {
            if(payment.InvoiceNo == null)
            {
                return 0M;
            }
            var total =  payments.Where(x => x.InvoiceNo == payment.InvoiceNo).Select(x => x.AmountPaid).Sum();
            return total;
        }

        public ActionResult MarkingPaymentStatus()
        {
            var model = new MarkingPaymentStatusModel
            {
                Statuses = new List<SelectListItem> {
                    new SelectListItem { Value = "PapersNotReceivedByNAATI", Text = "Papers not received by NAATI" },
                    new SelectListItem { Value = "BeingProcessedByNAATI", Text = "Being processed by NAATI" },
                    new SelectListItem { Value = "Paid", Text = "Paid", Selected = true }
                }
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MarkingPaymentStatus(MarkingPaymentStatusModel model)
        {
            var success = false;

            if (ModelState.IsValid)
            {
                var message = ValidateDateFromAndTo(model.SubmittedFrom, model.SubmittedTo, "submitted");

                if (!string.IsNullOrEmpty(message))
                {
                    ModelState.AddModelError("SubmittedFrom", message);
                }
                else
                {
                    success = true;
                }
            }

            if (!success)
            {
                return Json(new
                {
                    draw = Convert.ToInt32(Request.Form.Get("draw")),
                    totalRecords = 0,
                    recordsFiltered = 0,
                    data = new string[] { },
                    IsError = true,
                    Errors = ViewData.ModelState.Values
                        .SelectMany(m => m.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var markings =
                mExaminerToolsService.GetPayrollHistory(new GetPayrollHistoryRequest
                {
                    ExaminerNaatiNumber = User.NaatiNumber().ToString()
                })
                    .MarkingPayrollItems
                    .OrderByDescending(x => x.ResultReceivedDate)
                    .ToArray();

            const string beingProcessed = "Being processed by NAATI";
            const string paidStatus = "Paid";

            int attendanceId;
            if (model.AttendanceID != null && int.TryParse(model.AttendanceID, out attendanceId))
            {
                markings = markings.Where(x => x.TestAttendanceId == attendanceId).ToArray();
            }
            if (model.SubmittedFrom != null)
            {
                markings = markings.Where(x => x.ResultReceivedDate > model.SubmittedFrom).ToArray();
            }
            if (model.SubmittedTo != null)
            {
                markings = markings.Where(x => x.ResultReceivedDate < model.SubmittedTo).ToArray();
            }
            if (model.PayrollStatus != null)
            {
                markings =
                    markings.Where(x => model.PayrollStatus.Contains(ConvertPayrollStatusToUiForm(x.PayrollStatus)))
                        .ToArray();
            }

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = markings.Length,
                recordsFiltered = markings.Length,
                data = markings.Select(x =>
                {
                    string payrollStatus;
                    bool paid = false;
                    switch (x.PayrollStatus)
                    {
                        case "Received":
                        case "Ready":
                        case "InProgress":
                            payrollStatus = beingProcessed;
                            break;
                        case "Complete":
                            payrollStatus = paidStatus;
                            paid = true;
                            break;
                        default:
                            payrollStatus = "Unknown";
                            break;
                    }

                    return new
                    {
                        SubmittedDate = x.ResultReceivedDate.ToShortDateString(),
                        x.TestAttendanceId,
                        PayrollStatus = payrollStatus,
                        InvoiceNo = paid ? x.AccountingReference : null,
                        ProcessedDate = paid ? x.PayrollModifiedDate?.ToShortDateString() : null,
                        AmountPaid = paid ? x.ExaminerCost.ToString("c") : null,
                        InvoiceTotal = paid ? x.InvoiceTotal.ToString("c") : null,
                        x.Language,
                        x.TestType,
                        x.PaperReceivedDate,
                        x.Supplementary
                    };
                })
            });
        }

        public ActionResult FileTooLarge()
        {
            return View();
        }

        private static string ConvertPayrollStatusToUiForm(string status)
        {
            const string beingProcessed = "BeingProcessedByNAATI";
            const string paidStatus = "Paid";
            const string notReceivedStatus = "PapersNotReceivedByNAATI";
            switch (status)
            {
                case "Received":
                case "Ready":
                case "InProgress":
                    status = beingProcessed;
                    break;
                case "Complete":
                    status = paidStatus;
                    break;
                default:
                    status = notReceivedStatus;
                    break;
            }
            return status;
        }

        private static string ValidateSearch(GetTestsRequest criteria)
        {
            string result = null;

            result += ValidateDateFromAndTo(criteria.DateAllocatedFrom, criteria.DateAllocatedTo, "date allocated");
            result += ValidateDateFromAndTo(criteria.DateDueFrom, criteria.DateDueTo, "date due");

            return result;
        }

        private static string ValidateDateFromAndTo(DateTime? from, DateTime? to, string fieldName)
        {
            if (!(from.HasValue && to.HasValue))
            {
                return null;
            }

            return from.Value > to.Value
                ? $"The {fieldName} from {from.Value:dd/MM/yyyy} must be on or before the to date {to.Value:dd/MM/yyyy}"
                : null;
        }

        private bool ValidateExceededMarks(SubmitTestModel request)
        {
            var hasExceededMark = false;
            for (var i = 0; i < request.Components.Count; i++)
            {
                var c = request.Components[i];
                if (!(c.Mark > c.TotalMarks))
                {
                    continue;
                }

                var key = $"Components[{i}].Mark";
                var errorMessage = $"Exceeds the total mark for {c.Name}.";
                ModelState.AddModelError(key, errorMessage);
                hasExceededMark = true;
            }

            return hasExceededMark;
        }

        private void SubmitTest(SubmitTestModel request)
        {

            if (request.TestType == TestType.Rubric)
            {
                ModelState.Clear();
                var naatiNumber = User.NaatiNumber();
                var rubricContract = new TestRubricMarkingContract
                {
                    NaatiNumber = naatiNumber,
                    TestResultId = request.TestResultID.GetValueOrDefault(),
                    SubmittedDate = DateTime.Now,
                    TestComponents = request.Components.Select(_autoMapperHelper.Mapper.Map<TestMarkingComponentContract>).ToList(),
                    Feedback = request.Feedback,
                };
                var response = mExaminerToolsService.SaveExaminerRubricMarking(rubricContract);
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    ModelState.AddModelError("RubricResults", response.ErrorMessage);
                }
                return;
            }

            var serviceRequest = new SubmitTestRequest
            {
                UserId = User.NaatiNumber(),
                TestResultID = request.TestResultID.GetValueOrDefault(),
                Components = new List<TestComponentContract>(),
                Comments = request.Comments,
                Feedback = request.Feedback,
                ReasonsForPoorPerformance = request.ReasonsForPoorPerformance,
                PrimaryReasonForFailure = request.PrimaryReasonForFailure.GetValueOrDefault()
            };

            foreach (var c in request.Components)
            {
                serviceRequest.Components.Add(_autoMapperHelper.Mapper.Map<TestComponentContract>(c));
            }

            mExaminerToolsService.SubmitTest(serviceRequest);
        }

        [HttpPost]
        public void SaveTest(SubmitTestModel request)
        {
            if (request.TestType == TestType.Rubric)
            {
                var rubricContract = new TestRubricMarkingContract
                {
                    NaatiNumber = User.NaatiNumber(),
                    TestResultId = request.TestResultID.GetValueOrDefault(),
                    TestComponents = request.Components.Select(_autoMapperHelper.Mapper.Map<TestMarkingComponentContract>).ToList(),
                    Feedback = request.Feedback
                };
                var response = mExaminerToolsService.SaveExaminerRubricMarking(rubricContract);
                if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    ModelState.AddModelError("RubricResults", response.ErrorMessage);
                }

                return;
            }

            var naatiNumber = User.NaatiNumber();

            var serviceRequest = new SaveTestRequest
            {
                UserId = naatiNumber,
                TestID = request.TestResultID.GetValueOrDefault(),
                Components = new List<MyNaati.Contracts.Portal.TestComponentContract>(),
                Comments = request.Comments,
                Feedback = request.Feedback,
                ReasonsForPoorPerformance = request.ReasonsForPoorPerformance,
                PrimaryReasonForFailure = request.PrimaryReasonForFailure
            };

            foreach (var c in request.Components)
            {
                serviceRequest.Components.Add(_autoMapperHelper.Mapper.Map<MyNaati.Contracts.Portal.TestComponentContract>(c));
            }

            mExaminerToolsInternalService.SaveTest(serviceRequest);

        }

        private void FixValidationMessages(SubmitTestModel request)
        {
            foreach (var key in ViewData.ModelState.Keys)
            {
                var modelState = ViewData.ModelState[key];
                var toRemove = new List<ModelError>();
                var toAdd = new List<string>();

                foreach (var error in modelState.Errors)
                {
                    if (!key.StartsWith("Components["))
                    {
                        continue;
                    }

                    var index = key.Split(']')[0].Replace("Components[", string.Empty);
                    toRemove.Add(error);
                    toAdd.Add(error.ErrorMessage.Replace("Mark", request.Components[int.Parse(index)].Name));
                }

                foreach (var tr in toRemove)
                {
                    modelState.Errors.Remove(tr);
                }

                foreach (var ta in toAdd)
                {
                    modelState.Errors.Add(ta);
                }
            }
        }

        private void CompleteModel(SubmitTestModel model)
        {
            model.TestList = GetTests(false);
            model.PrimaryReasonsForFailure = GetReasonsForFailure(model);
            model.DocumentTypes = mExaminerToolsService.GetDocumentTypes().DocumentTypes.Select(dt => new DocumentTypeModel { Id = dt.Id, DisplayName = dt.DisplayName }).ToList();
        }

        private static IEnumerable<ReasonForFailureModel> GetReasonsForFailure(SubmitTestModel model)
        {
            return new List<ReasonForFailureModel> {
                new ReasonForFailureModel { Id = 0, Text = "" },
                new ReasonForFailureModel { Id = 1, Text = "Lack of proficiency in English" },
                new ReasonForFailureModel { Id = 2, Text = "Lack of proficiency in LOTE" },
                new ReasonForFailureModel { Id = 3, Text = $"Lack of {(model.TestList.Any(x => x.Category == "Interpreter") ? "interpreting" : "translating")} skills"
                }
            };
        }

        private List<TestModel> GetTests(bool asChair)
        {

            var request = new GetTestsRequest
            {
                UserId = User.NaatiNumber(),
                AsChair = asChair,
                TestStatus = new[] { "InProgress", "Overdue" },
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner }.ToArray()
            };

            var response = mExaminerToolsService.GetTests(request);

            return response.Tests.Select(t =>
            {
                var mapped = _autoMapperHelper.Mapper.Map<TestContract, TestModel>(t);
                if (mapped.Status == "InProgress")
                {
                    mapped.Status = "In Progress";
                }
                return mapped;
            }).ToList();
        }

        private IEnumerable<TestModel> GetTestsMaterial()
        {
            var request = new GetTestsMaterialRequest
            {
                UserId = User.NaatiNumber(),
                RoleCategoryIds = new[] { (int)PanelRoleCategoryName.Examiner }
            };

            var response = mExaminerToolsService.GetTestsMaterial(request);

            return response.Tests.Select(_autoMapperHelper.Mapper.Map<TestMaterialContract, TestModel>).ToList();
        }

        [HttpGet]
        public ActionResult ManageTestMaterials()
        {
            // to be enabled in a later release
            //var mfaConfigurationAndEmailCodeActiveStatus = GetMfaConfigurationAndEmailCodeActiveStatus(CurrentUserNaatiNumber);
            //if (!mfaConfigurationAndEmailCodeActiveStatus.MfaConfiguredAndActiveOrEmailCodeActive)
            //{
            //    return View("_MfaAndAccessCodePartial", new MfaAndAccessCodeModel()
            //    {
            //        MfaConfigured = false,
            //        MfaConfiguredAndActiveOrEmailCodeActive = false,
            //        ReturnController = "ExaminerTools",
            //        ReturnView = "ManageTestMaterials"
            //    });
            //}

            return View(new ManageTestMaterialsModel
            {
                CredentialTypes = mExaminerToolsService.GetMaterialRequestCredentialTypes(CurrentUserNaatiNumber).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.DisplayName, Selected = true }),
                RoundStatusTypes = mExaminerToolsService.GetRoundStatusTypes().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.DisplayName }),
            });
        }

        [HttpGet]
        public ActionResult ManageTestMaterialsData(GetMaterialRequestsRequest request = null)
        {
            request = request ?? new GetMaterialRequestsRequest();
            request.NaatiNumber = User.NaatiNumber();
            var response = mExaminerToolsService.GetMaterialRequests(request);

            return Json(new ManageTestMaterialsModel
            {
                TestMaterialsList = response.MaterialRequests.Select(_autoMapperHelper.Mapper.Map<MaterialRequestSearchResultContract, MaterialRequestSearchResultModel>).ToList(),
                TotalAvailableRows = response.TotalAvailableRows
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ManageTestMaterial(int id)
        {
            return View();
        }

        [HttpGet]
        [Route("managetestmaterialinfo/{id}")]
        public ActionResult ManageTestMaterialInfo(int id)
        {
            var response = mExaminerToolsService.GetMaterialRequest(id, User.NaatiNumber(), false);
            TestMaterialRequestModel model = null;

            if (response != null)
            {
                model = _autoMapperHelper.Mapper.Map<TestMaterialRequestContract, TestMaterialRequestModel>(response.MaterialRequest);
                model.AvailableDocumentTypes = response.DocumentTypes;
                model.MaterialRequestCoordinatorLoadingPercentage = mSystemValueService.GetAll().FirstOrDefault(s => s.Key == "MaterialRequestCoordinatorLoadingPercentage")?.Value;
                model.MaterialRequestTaskTypes = response.MaterialRequestTaskTypes.Select(m => new KeyValuePair<int, string>(m.Id, m.DisplayName));
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("managetestmaterialmembers/{panelId}/{credentialTypeId}/{materialRequestId}")]
        public ActionResult ManageTestMaterialMembers(int panelId, int credentialTypeId, int materialRequestId)
        {
            var members = mExaminerToolsService.GetPanelMembershipLookUp(
                new GetPanelMemberLookupRequestModel()
                {
                    PanelIds = new[] { panelId },
                    ActiveMembersOnly = true,
                    CredentialTypeId = credentialTypeId
                }).Members.ToList();

            return Json(members, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("materialrequestroundattachment")]
        public ActionResult PostMaterialRequestRoundAttachment()
        {
            var ids = new List<int>();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName))
                {
                    continue;
                }

                var roundId = Convert.ToInt32(Request["id"]);
                var type = Convert.ToInt32(Request["type"]);

                string path;

                using (var fileStream = System.IO.File.Create(ConfigurationManager.AppSettings["tempFilePath"] + '\\' + file.FileName))
                {
                    file.InputStream.Seek(0, SeekOrigin.Begin);
                    file.InputStream.CopyTo(fileStream);
                    path = fileStream.Name;
                }

                var request = new SaveMaterialRequestRoundAttachmentRequest
                {
                    NAATINumber = User.NaatiNumber(),
                    FilePath = path,
                    Title = file.FileName,
                    Type = type,
                    MaterialRequestRoundId = roundId
                };

                var response = mExaminerToolsService.SaveMaterialRequestRoundAttachment(request);
                ids.Add(response.StoredFileId);
            }

            return Json(new { success = true, id = ids });
        }

        [HttpPost]
        [Route("materialrequestmembers")]
        public void PostMaterialRequestMembers(PostMaterialRequestMembersRequestModel request)
        {
            var serviceRequest = _autoMapperHelper.Mapper.Map<UpdateMaterialRequestMembersRequest>(request);
            serviceRequest.NaatiNumber = User.NaatiNumber();
            mExaminerToolsService.UpdateMaterialRequestMembers(serviceRequest);
        }

        [HttpGet]
        [Route("materialrequestroundattachments/{id}")]
        public ActionResult GetMaterialRequestRoundAttachments(int id)
        {
            var request = new GetMaterialRequestRoundAttachmentsRequest
            {
                MaterialRequestRoundId = id,
                NAATINumber = User.NaatiNumber(),
                ExaminerAvailable = true,
            };

            var response = mExaminerToolsService.GetMaterialRequestRoundAttachments(request);
            var attachments = response.Attachments.Select(f => _autoMapperHelper.Mapper.Map<MaterialRequestRoundAttachmentDto, MaterialRequestRoundAttachmentModel>(f));

            return Json(new
            {
                draw = Convert.ToInt32(Request.Form.Get("draw")),
                totalRecords = attachments.Count(),
                recordsFiltered = attachments.Count(),
                data = attachments
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("materialrequestdomains/{credentialTypeId}")]
        public ActionResult GetMaterialRequestDomains(int credentialTypeId)
        {
            return Json(mExaminerToolsService.GetTestMaterialDomains(credentialTypeId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("materialrequestroundattachment/{id}/{materialRoundId}")]
        public ActionResult GetMaterialRequestRoundAttachment(int id, int materialRoundId)
        {
            var file = mExaminerToolsService.GetMaterialRequestRoundAttachment(new GetMaterialRequestRoundAttachmentRequest
            {
                MaterialRequestRoundAttachmentId = id,
                TempFileStorePath = ConfigurationManager.AppSettings["tempFilePath"],
                NaatiNumber = User.NaatiNumber(),
                MaterialRequestRoundId = materialRoundId
            });
            Response.BufferOutput = false;
            return File(mMaterialRequestService.ReplaceDocumentTokens(file.FilePaths[0], materialRoundId), MimeMapping.GetMimeMapping(file.FileName), file.FileName);
        }

        [HttpDelete]
        [Route("materialrequestroundattachment/{id}/{materialRoundId}")]
        public void DeleteMaterialRequestRoundAttachment(int id, int materialRoundId)
        {
            var request = new DeleteMaterialRequestRoundAttachmentRequest
            {
                MaterialRequestRoundAttachmentId = id,
                MaterialRequestRoundId = materialRoundId,
                NaatiNumber = User.NaatiNumber()
            };

            mExaminerToolsService.DeleteMaterialRequestRoundAttachment(request);
        }

        [HttpGet]
        [Route("materialrequestlinks/{materialRoundId}")]
        public ActionResult GetMaterialRequestLinks(int materialRoundId)
        {
            var response = mExaminerToolsService.GetMaterialRequestRoundLink(materialRoundId, User.NaatiNumber());
            return Json(response.Results.Select(m => _autoMapperHelper.Mapper.Map<MaterialRequestRoundLinkModel>(m)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("materialrequestlinks")]
        public void PostMaterialRequestLinks(MaterialRequestRoundLinkModel request)
        {
            mExaminerToolsService.SaveMaterialRequestRoundLink(new SaveMaterialRequestRoundLinkRequest
            {
                MaterialRequestRoundId = request.MaterialRequestRoundId,
                NaatiNumber = User.NaatiNumber(),
                Link = request.Link
            });
        }

        [HttpDelete]
        [Route("materialrequestlinks/{materialRoundId}/{linkId}")]
        public void DeleteMaterialRequestLinks(int linkId, int materialRoundId)
        {
            mExaminerToolsService.DeleteMaterialRequestLink(linkId, User.NaatiNumber(), materialRoundId);
        }

        [HttpPost]
        [Route("materialrequestsubmitforapprovalvalidation/{materialRequestId}/{roundId}/{testMaterialDomainId}")]
        public ActionResult PostMaterialRequestSubmitForApprovalValidation(int materialRequestId, int roundId, int testMaterialDomainId)
        {
            List<string> messages = new List<string>();

            var materialRequest = mExaminerToolsService.GetMaterialRequest(materialRequestId, User.NaatiNumber(), true);
            if (materialRequest == null)
            {
                throw new Exception($"User is not authorised to perform an action on material request {materialRequestId}");
            }

                //Check for files
                var attachmentRequest = new GetMaterialRequestRoundAttachmentsRequest
                {
                    MaterialRequestRoundId = roundId,
                    NAATINumber = User.NaatiNumber(),
                    ExaminerAvailable = true,
                };
                var attachmentResponse = mExaminerToolsService.GetMaterialRequestRoundAttachments(attachmentRequest);
                var linkResponse = mExaminerToolsService.GetMaterialRequestRoundLink(roundId, User.NaatiNumber());

                if (attachmentResponse.Attachments.Count(x => x.IsOwner) + linkResponse.Results.Count(x => x.IsOwner) <= 0)
                {
                    messages.Add("You have not uploaded any documents or provided any links.");
                }

                //Check for participants
                var materialRequestResponse =
                    mExaminerToolsService.GetMaterialRequest(materialRequestId, User.NaatiNumber(), false);
                
                if (materialRequestResponse.MaterialRequest.Members.All(x => x.MemberTypeId == (int) MaterialRequestPanelMembershipTypeName.Coordinator))
                {
                    messages.Add("You have not selected any additional participants.");
                }

                //If there are any validation messages:
                if (messages.Any())
                {
                    var finalMessage = string.Join(" ", messages);
                    finalMessage += "\n\n Are you sure you want to submit for approval?";
                    return Json(finalMessage, JsonRequestBehavior.AllowGet);
                }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("materialrequestsubmitforapproval/{materialRequestId}/{roundId}/{testMaterialDomainId}")]
        public ActionResult PostMaterialRequestSubmitForApproval(int materialRequestId, int roundId, int testMaterialDomainId)
        {
            var data = new NcmsMaterialRequestActionRequest
            {
                MaterialRequestId = materialRequestId,
                MaterialRequestRoundId = roundId,
                TestMaterialDomainId = testMaterialDomainId,
                ActionId = (int)SystemActionTypeName.SubmitRoundForApproval
            };

            var materialRequest = mExaminerToolsService.GetMaterialRequest(materialRequestId, User.NaatiNumber(), true);
            if (materialRequest == null)
            {
                throw new Exception($"User is not authorised to perform an action on material request {materialRequestId}");
            }

            var messages = _ncmsService.ValidateMaterialRequest(data);
            if (messages.Any())
            {
                return Json(string.Join(", ", messages), JsonRequestBehavior.AllowGet);
            }

            var message = _ncmsService.ExecuteMaterialRequestAction(data);

            if (message.Any())
            {
                return Json("Failed to submit material development for approval. Please try again or contact NAATI immediately on <a href=\"mailto:info@naati.com.au ? subject = Failed to submit material request\">info@naati.com.au</a> so that they can resolve this issue.", JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [Route("materialrequestnote/{materialRequestId}")]
        public ActionResult GetMaterialRequestPublicNotes(int materialRequestId)
        {
            return Json(mExaminerToolsService.ListMaterialRequestPublicNotes(materialRequestId).Notes, JsonRequestBehavior.AllowGet);
        }

        [Route("materialrequestnote")]
        public void PostMaterialRequestPublicNotes(int materialRequestId, string note)
        {
            mExaminerToolsService.CreateMaterialRequestPublicNote(materialRequestId, note);
        }

        [Route("materialrequestupdatemembers/{materialRequestId}/{roundId}/{testMaterialDomainId}")]
        public ActionResult PostMaterialRequestUpdateMembers(int materialRequestId, int roundId, int testMaterialDomainId, IEnumerable<MaterialRequestPanelMembershipModel> members)
        {
            var data = new NcmsMaterialRequestActionRequest
            {
                MaterialRequestId = materialRequestId,
                MaterialRequestRoundId = roundId,
                TestMaterialDomainId = testMaterialDomainId,
                ActionId = (int)SystemActionTypeName.UpdateMaterialRequestMembers,
                Steps = new[]
                {
                    new {
                        Id = 5,
                        Data = new {
                            SelectedMembers = members
                        }
                    }
                }
            };

            var materialRequest = mExaminerToolsService.GetMaterialRequest(materialRequestId, User.NaatiNumber(), true);
            if (materialRequest == null)
            {
                throw new Exception($"User is not authorised to perfom an action on material request {materialRequestId}");
            }

            var messages = _ncmsService.ValidateMaterialRequest(data);
            if (messages.Any())
            {
                return Json(string.Join(", ", messages), JsonRequestBehavior.AllowGet);
            }


            var message = _ncmsService.ExecuteMaterialRequestAction(data);

            if (message.Any())
            {
                return Json("Failed to update members from this material request. Please try again or contact NAATI immediately on <a href=\"mailto:info@naati.com.au ? subject = Failed to update members from a material request\">info@naati.com.au</a> so that they can resolve this issue.", JsonRequestBehavior.AllowGet);
            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        private class JsonUpdateResponse
        {
            public bool Success { get; set; }
            public IList<string> Errors { get; set; }
        }
    }
}
