using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Bl.FileDeletion;
using Ncms.Contracts.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace Ncms.Bl.BackgroundTasks
{
    public class ProcessFileDeletesPastExpiryDateTask : BaseBackgroundTask, IProcessFileDeletesPastExpiryDateTask
    {
        private readonly IFileDeletionDalService _fileDeletionDalService;
        private readonly IUserQueryService _userQueryService;
        private readonly ISystemQueryService _systemQueryService;
        private FileDeletionRunner FileDeletionRunner;
        private int CurrentUserId = 0;
        
        public ProcessFileDeletesPastExpiryDateTask(IFileDeletionDalService fileDeletionDalService, IUserQueryService userQueryService, ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger,IUtilityQueryService utilityQueryService) 
            : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _fileDeletionDalService = fileDeletionDalService;
            _userQueryService = userQueryService;
            _systemQueryService = systemQueryService;
            TaskLogger.WriteInfo($"Initialising Task...");
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                TaskLogger.WriteInfo($"Executing Task at {DateTime.Now}");
                try
                {
                    var userName = "";

                    if (parameters.TryGetValue("UserName", out userName))
                    {
                        CurrentUserId = _userQueryService.GetUser(userName).Id;
                    }

                    if (CurrentUserId == 0)
                    {
                        throw new FileDeletionException($"User: {userName} does not exist. File deletion cannot continue.");
                    }
                }
                catch (FileDeletionException ex)
                {
                    TaskLogger.WriteError(ex, "FileDeletionException", "FileDeletionException", true);
                    return;
                }
                catch (Exception ex)
                {
                    TaskLogger.WriteError(ex, "Undefined", "Undefined", false);
                    return;
                }

                //WriteMemoryUsageToSeq();

                FileDeletionRunner = new FileDeletionRunner(CurrentUserId, _fileDeletionDalService, TaskLogger, _systemQueryService);

                ProcessFileDeletesPastExpiryDate(startTime);

                TaskLogger.WriteInfo($"Task completed at {DateTime.Now}");
            }
            catch (ThreadAbortException ex)
            {
                _systemQueryService.SetSystemValue(new SetSystemValueRequest() { ValueKey = "ProcessFileDeletesPastExpiryDate", Value = "0" });
                TaskLogger.WriteError(ex, nameof(ThreadAbortException), "Thread was aborted.", false);
            }
        }

        private void WriteMemoryUsageToSeq()
        {
            //var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service WHERE Started = TRUE");
            //get application names from application pool
            string path = string.Format("IIsApplicationPool.Name='W3SVC/APPPOOLS/{0}'", "localhost");
            var scope = new ManagementScope("\\\\JDRP6Z2\\root\\cimv2");

            //        ObjectQuery query = new ObjectQuery(
            //"SELECT * FROM Win32_OperatingSystem");
            //        ManagementObjectSearcher searcher =
            //            new ManagementObjectSearcher(scope, query);

            //        ManagementObjectCollection queryCollection = searcher.Get();
            //        foreach (ManagementObject m in queryCollection)
            //        {
            //            // Display the remote computer information
            //            Console.WriteLine("Computer Name : {0}",
            //                m["csname"]);
            //            Console.WriteLine("Windows Directory : {0}",
            //                m["WindowsDirectory"]);
            //            Console.WriteLine("Operating System: {0}",
            //                m["Caption"]);
            //            Console.WriteLine("Version: {0}", m["Version"]);
            //            Console.WriteLine("Manufacturer : {0}",
            //                m["Manufacturer"]);
            //        }

            scope = new ManagementScope("\\\\JDRP6Z2\\root\\MicrosoftIISV2");

            ManagementPath managementPath = new ManagementPath(path);
            ManagementObject classInstance = new ManagementObject(scope, managementPath, null);
            ManagementBaseObject outParams = classInstance.InvokeMethod("EnumAppsInPool", null, null);


            //get web server names from application names
            IEnumerable<string> nameList = (outParams.Properties["Applications"].Value as string[]) //no null reference exception even there is no application running
                                           .Where(item => !String.IsNullOrEmpty(item)) //but you get empty strings so they are filtered
                                           .ToList() //you get something like /LM/W3SVC/1/ROOT
                                           .Select(item => item); //your WebServer.Name is between 2nd and 4th slahes


            //get server comments from names
            List<string> serverCommentList = new List<string>();
            foreach (string name in nameList)
            {
                ManagementObjectSearcher searcher2 =
                    new ManagementObjectSearcher(scope, new ObjectQuery(string.Format("SELECT ServerComment FROM IIsWebServerSetting WHERE Name = '{0}'", name)));

                serverCommentList.AddRange(from ManagementObject queryObj in searcher2.Get() select queryObj["ServerComment"].ToString());
            }


            //for service in c.Win32_Service(Name = "W3SVC"):
            //    result, = service.StopService()
        }

        private void ProcessFileDeletesPastExpiryDate(DateTime startTime)
        {
            // get the file deletion generators based off priority
            var fileDeletionGenerators = FileDeletionRunner.GetFileDeletionGenerators();
            // get all the file deletion detail lists for each entity type and document type
            var fileDeletionDetailsList = FileDeletionRunner.GetFileDeletionDetails(fileDeletionGenerators);

            foreach(var fileDeletionDetails in fileDeletionDetailsList)
            {
                // Person Images are handled differently than stored files
                if (fileDeletionDetails.EntityType == "tblPersonImage")
                {
                    // if no person images to delete then continue
                    if (fileDeletionDetails.CurrentStatusCount == 0)
                    {
                        continue;
                    }
                    // log the count of person images selected
                    TaskLogger.WriteInfo($"{fileDeletionDetails.CurrentStatusCount} person images selected for " +
                    $"Entity Type: {fileDeletionDetails.EntityType} and Document Type: {fileDeletionDetails.DocumentType}");

                    continue;
                }

                // if no stored files selected for deletion then continue
                if (fileDeletionDetails.CurrentStatusCount == 0 && fileDeletionDetails.QueuedStatusCount == 0)
                {
                    continue;
                }
                // log the count of stored files selected of current status and queued status
                TaskLogger.WriteInfo($"Current Status: {fileDeletionDetails.CurrentStatusCount} and Queued Status: {fileDeletionDetails.QueuedStatusCount} files selected for " +
                    $"Entity Type: {fileDeletionDetails.EntityType} and Document Type: {fileDeletionDetails.DocumentType}");
            }

            // FOR TESTING PURPOSES, CREATE STORED FILES FOR TEST ENVIRONMENTS BECAUSE THEY WILL NOT EXIST, CONFIGURABLE IN SYSTEM VALUE
            var createFiles = false;
            int.TryParse(GetSystemValue("ProcessFileDeletesPastExpiryDateCreateStoredFiles"), out var createFilesSystemVal);
            if (createFilesSystemVal == 1)
            {
                createFiles = true;
            }
            if (createFiles)
            {
                FileDeletionRunner.CreateStoredFiles(fileDeletionDetailsList);
            }
            // END OF CREATION TESTING

            // process soft deletes for the file deletion details,
            // NOTE: person images are hard deleted not soft deleted
            FileDeletionRunner.ProcessSoftDeletes(fileDeletionDetailsList, startTime);
            // log todays and the previous 7 days amount of files deleted per day
            FileDeletionRunner.LogPreviousWeeksFileDeletionCounts();
        }
    }
}