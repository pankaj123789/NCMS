using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class TemporaryFileDeletionBackgroundTask : BaseBackgroundTask, ITemporaryFileDeletionBackgroundTask
    {
        public TemporaryFileDeletionBackgroundTask(ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            DeleteTemporaryFiles();
        }

        private void DeleteTemporaryFiles()
        {
            TaskLogger.WriteInfo("Getting temporary file..");
            var temporaryLocation = ConfigurationManager.AppSettings["tempFilePath"];

            if (temporaryLocation.StartsWith("~"))
            {
                temporaryLocation = System.Web.HttpContext.Current.Server.MapPath(temporaryLocation);
            }

            TaskLogger.WriteInfo($"Temporary file:{temporaryLocation}");

            var fileRetentionHours = GetSystemValue("TemporaryFilesRetentionHours");

            TaskLogger.WriteInfo($"Removing older than {fileRetentionHours} hours");

            var folder = new DirectoryInfo(temporaryLocation);
            if (!folder.Exists)
            {
                TaskLogger.WriteError($"Folder: {temporaryLocation} not found", "undefined", "reference", false);
                return;
            }

            var minRetentionDate = DateTime.Now.AddHours(-Convert.ToInt32(fileRetentionHours)).ToUniversalTime();
            TaskLogger.WriteInfo($"Removing files older than UTC: {minRetentionDate}. Current UTC time: { DateTime.Now.ToUniversalTime()}");

            var files = folder.EnumerateFiles("*", SearchOption.AllDirectories);

            ExecuteIfRunning(files, f => DeleteFile(f, minRetentionDate));

            var subFolders = folder.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            ExecuteIfRunning(subFolders, f => DeleteFolder(f, minRetentionDate));
        }
        public void DeleteFile(FileInfo file, DateTime minRetentionDate)
        {
            try
            {
                TaskLogger.WriteInfo($"Checking file {file.FullName} with creation UTC  time: {file.CreationTimeUtc}");
                if (file.CreationTimeUtc < minRetentionDate)
                {
                    TaskLogger.WriteInfo($"Deleting File {file.FullName}");
                    file.Delete();
                }
                else
                {
                    TaskLogger.WriteInfo($"File {file.FullName} has been skipped because creation time UTC IS : {file.CreationTimeUtc}");
                }
            }
            catch (Exception ex)
            {
                TaskLogger.WriteError(ex, "undefined", "undefined", false);
                TaskLogger.WriteError($"Error deleting temporary files {file.FullName}", "undefined", "undefined", false);
            }
        }

        public void DeleteFolder(DirectoryInfo directory, DateTime minRetentionDate)
        {
            try
            {
                if (!directory.EnumerateFiles("*", SearchOption.AllDirectories).Any() && (directory.CreationTimeUtc < minRetentionDate))
                {
                    directory.Delete(true);
                }
            }
            catch (Exception ex)
            {
                TaskLogger.WriteError(ex, "undefined", "undefined", false);
                TaskLogger.WriteError($"Error deleting temporary directory {directory.FullName}", "undefined", "undefined", false);
            }
        }
    }
}
