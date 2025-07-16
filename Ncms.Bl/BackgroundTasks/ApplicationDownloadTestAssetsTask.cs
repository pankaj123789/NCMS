//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using F1Solutions.Global.Common.Mapping;
//using F1Solutions.Naati.Common.Contracts.Bl;
//using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
//using F1Solutions.Naati.Common.Contracts.Dal;
//using F1Solutions.Naati.Common.Contracts.Dal.DTO;
//using F1Solutions.Naati.Common.Contracts.Dal.Enum;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using F1Solutions.Naati.Common.Contracts.Dal.Request;
//using F1Solutions.Naati.Common.Contracts.Dal.Services.Televic;
//using Ncms.Contracts;
//using Ncms.Contracts.BackgroundTask;
//using Ncms.Contracts.Models.Application;

//namespace Ncms.Bl.BackgroundTasks
//{
//    public class ApplicationDownloadTestAssetsTask : ApplicationBackgroundTask, IApplicationDownloadTestAssetsTask
//    {
//        private readonly ITelevicIntegrationService _televicIntegrationService;
//        private readonly ITestSessionQueryService _testSessionQueryService;
//        private readonly ITestResultQueryService _testResultQueryService;
//        private readonly IUserService _userService;

//        public ApplicationDownloadTestAssetsTask(ISystemQueryService systemQueryService, 
//            IBackgroundTaskLogger backgroundTaskLogger,
//            IUtilityQueryService utilityQueryService,
//            ITelevicIntegrationService televicIntegrationService,
//            ITestSessionQueryService testSessionQueryService,
//            ITestResultQueryService testResultQueryService,
//            IUserService userService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
//        {
//            _televicIntegrationService = televicIntegrationService;
//            _testSessionQueryService = testSessionQueryService;
//            _testResultQueryService = testResultQueryService;
//            _userService = userService;
//        }

//        public override void Execute(IDictionary<string, string> parameters)
//        {
//            //DownloadTestSessionFiles();
//        }

//        public void DownloadTestSessionFiles()
//        {
//            //var sessions = _televicIntegrationService.GetFinishedSessions();

//            var dtos = new List<dynamic>();

//            //get credentialrequestid, app id, test sitting id, test session id, naati number. From our DB. in batches (config value for batch size)
//            //where testsession is now - some configurable time and credentialrequeststatus is testesssionaccepted, testsitting.rejected = false. start with testsitting
//            //keep looping untill page returns empty list (or less that page amount)

//            int pageSize = Convert.ToInt32(GetSystemValue("DownloadForTelevicPageSize"));
//            DateTime startDate = DateTime.Now.AddHours(Convert.ToInt32(GetSystemValue("DownloadForTelevicRelativeStartTime")));
//            DateTime endDate = DateTime.Now.AddHours(Convert.ToInt32(GetSystemValue("DownloadForTelevicRelativeEndTime")));
//            int passedPages = 0;

//            TaskLogger.WriteInfo("Getting test sessions for Televic Download...");

//            //first page
//            GetTestSessionsForTelevicDownloadRequest request = new GetTestSessionsForTelevicDownloadRequest
//            {
//                Page = passedPages,
//                PageSize = pageSize,
//                StartDate = startDate,
//                EndDate = endDate
//            };
            
//            var testSessionModels = _testSessionQueryService.GetTestSessionsForTelevicDownload(request).Result.ToList();

//            TaskLogger.WriteInfo("Page " + passedPages + " retrieved...");

//            while (testSessionModels.Count > 0)
//            {
//                passedPages += 1;

//                var testSessionGroupedSitting = testSessionModels.GroupBy(x => x.TestSessionId).ToList();

//                foreach (var testSessionGroup in testSessionGroupedSitting)
//                {
//                    CallTelevic(testSessionGroup);
//                }

//                //Create new request
//                request.Page = passedPages;
//                request.PageSize = pageSize;
//                request.StartDate = startDate;
//                request.EndDate = endDate;

//                TaskLogger.WriteInfo("Retrieving next page...");
//                testSessionModels = _testSessionQueryService.GetTestSessionsForTelevicDownload(request).Result.ToList();
//                TaskLogger.WriteInfo("Page " + passedPages + " retrieved...");
//            }
//            TaskLogger.WriteInfo("Page " + passedPages + " is empty...");
//        }

//        private void CallTelevic(IGrouping<int, GetTestSessionsForTelevicDownloadDto> group)
//        {
//            TaskLogger.WriteInfo("Calling Televic...");

//            int pageSize = Convert.ToInt32(GetSystemValue("TelevicDownloadPageSize"));

//            //GetTestSessionsForTelevicDownloadRequest request = new GetTestSessionsForTelevicDownloadRequest
//            //{
//            //    Page = passedPages,
//            //    PageSize = pageSize,
//            //    StartDate = startDate,
//            //    EndDate = endDate
//            //};

//            /// var testSessionModels = _televicIntegrationService.GetFinishedSessions(group.Key);
//            /// 

//            var testSessionNaatiNumbers = new List<int>();  // assuming this are naatinumbers

//            //********************************TESTING**********************************
//            testSessionNaatiNumbers.Add(92747);
//            //********************************TESTING**********************************

//            TaskLogger.WriteInfo("Retrieved test session users...");

//            while (testSessionNaatiNumbers.Count > 0)
//            {
//                // passedPages += 1;             

//                foreach (var testSessionNaatiNumber in testSessionNaatiNumbers)
//                {
//                    TaskLogger.WriteInfo("Retrieving file paths...");
//                    var filePaths = DownloadAssets(testSessionNaatiNumber);
//                    TaskLogger.WriteInfo(filePaths.Count + " file paths retrieved...");

//                    TaskLogger.WriteInfo("Uploading assets...");
//                    UploadAssets(filePaths, testSessionNaatiNumber);

//                    TaskLogger.WriteInfo("Mark test sitting as sat...");
//                    ExecuteAction(SystemActionTypeName.MarkAsSat, group.FirstOrDefault(x => x.NaatiNumber == testSessionNaatiNumber));
//                }

//                //Create new request
//                //request.Page = passedPages;
//                //request.PageSize = pageSize;
//                //request.StartDate = startDate;
//                //request.EndDate = endDate;

//                testSessionNaatiNumbers = new List<int>(); // call televic _testSessionQueryService.GetTestSessionsForTelevicDownload(request).Result.ToList();
//            }
//        }

//        private IList<string> DownloadAssets(int naatinumber) 
//        {
//            /// call televice to get the files for the user
//            /// 

//            List<string> documentPaths = new List<string>();

//            //********************************TESTING**********************************
//            documentPaths.Add(@"C:\tempStorage\TemporaryFiles\test\CP Date Test 1.csv");
//            documentPaths.Add(@"C:\tempStorage\TemporaryFiles\test\CP Date Test 2.csv");
//            documentPaths.Add(@"C:\tempStorage\TemporaryFiles\test\CP Date Test 3.csv");
//            documentPaths.Add(@"C:\tempStorage\TemporaryFiles\test\CP Date Test 4.csv");
//            //********************************TESTING**********************************

//            // return path of the files
//            return documentPaths;
//        }

//        private void UploadAssets(IEnumerable<string> paths, int naatiNumber)
//        {
//            foreach (string path in paths)
//            {
//                TaskLogger.WriteInfo("Uploading file: "  + path + " ...");

//                CreateOrReplaceTestSittingDocumentDto document = new CreateOrReplaceTestSittingDocumentDto
//                {
//                    StoragePath = path,
//                    FilePath = path,
//                    Title = path,
//                    Type = StoredFileType.MarkedTestAsset.ToString(),
//                    TestSittingId = 15488
//                };

//                document.UploadedByUserId = _userService.Get()?.Id ?? 0;

//                var response = _testResultQueryService.CreateOrUpdateDocument(new CreateOrUpdateDocumentRequest
//                {
//                    Document = document
//                });

//                TaskLogger.WriteInfo("File uploaded  ...");
//            }
//        }
        
//        private bool ExecuteAction(SystemActionTypeName action, GetTestSessionsForTelevicDownloadDto dto)
//        {
//            var applicationId = dto.CredentialApplicationId;
//            TaskLogger.WriteInfo("Executing action: {Action}, ApplicationId: {ApplicationId}", action, applicationId);
            
//            var model = new ApplicationActionWizardModel
//            {
//                ApplicationId = dto.CredentialApplicationId,
//                CredentialRequestId = dto.CredentialRequestId,
//                ActionType = (int)action
//            };
//            model.SetBackGroundAction(true);
//            try
//            {
//                var response = ServiceLocator.Resolve<IApplicationService>().PerformAction(model);
//                LogResponse(response, "DownloadTestAssets", applicationId);

//                TaskLogger.AdProcessedApplication($"ApplicationId : {applicationId}");

//                return true;
//            }

//            catch (UserFriendlySamException ex)
//            {
//                TaskLogger.WriteApplicationError(ex, applicationId, "DownloadTestAssets", string.Empty, true);
//            }
//            catch (Exception ex)
//            {
//                TaskLogger.WriteApplicationError(ex, applicationId, "DownloadTestAssets", string.Empty, false);
//            }

//            return false;
//        }
//    }
//}
