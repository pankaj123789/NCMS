using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;

namespace Ncms.Bl.BackgroundTasks
{
    public class DeleteOldSystemDataBackgroundTask : BaseBackgroundTask, IDeleteOldSystemDataBackgroundTask
    {
        private readonly ILifecycleService _lifecycleService;

        public DeleteOldSystemDataBackgroundTask(ISystemQueryService systemQueryService, ILifecycleService lifecycleService, IBackgroundTaskLogger backgroundTaskLogger, IUtilityQueryService utilityQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _lifecycleService = lifecycleService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
            DeleteOldSystemData();
        }

        private void DeleteOldSystemData()
        {
            TaskLogger.WriteInfo("Getting MaxPodHistoryRetentionDays...");
            var maxPodHistoryRetentionDays = Convert.ToInt32(GetSystemValue("MaxPodHistoryRetentionDays"));
            var maxPodHistoryLength = Convert.ToInt32(GetSystemValue("MaxPodHistoryLength"));
            TaskLogger.WriteInfo($"MaxPodHistoryDeletionDays: {maxPodHistoryRetentionDays}");

            var maxTerminationDate = DateTime.Now.AddDays(-maxPodHistoryRetentionDays).ToUniversalTime();
            TaskLogger.WriteInfo($"MaxPodHistoryDeletionDays: {maxPodHistoryRetentionDays}");

            var foldersFound = false;
            do
            {
                ThrowIfNotRunning();
                TaskLogger.WriteInfo($"Getting pods terminated before: {maxTerminationDate}");
                var terminatedPods = _lifecycleService.GetTerminatedPods(
                    new GetTerminatedPodsRequest()
                    {
                        Take = maxPodHistoryLength,
                        MaxTerminationDate = maxTerminationDate
                    });

                foldersFound = terminatedPods.Any();

                TaskLogger.WriteInfo(
                    $"Deleting top {maxPodHistoryLength} application data for pods terminated before: {maxTerminationDate}");

                ExecuteIfRunning(terminatedPods, p => DeletePodFolder(p, maxTerminationDate));
            }
            while (foldersFound);
        }

        private void DeletePodFolder(PodHistoryModel podHistory, DateTime maxTerminationDate)
        {
            TaskLogger.WriteInfo($"Trying to delete folder {podHistory.FolderPath}");
            try
            {
                var folderInfo = new DirectoryInfo(podHistory.FolderPath);
                if (!folderInfo.Exists)
                {
                    var message =$"Application folder '{podHistory.FolderPath}' was not deleted because it was not found";
                    TaskLogger.WriteWarning(message);

                    _lifecycleService.LogPodDHistoryDeletionError(podHistory.PodHistoryId, message);
                    return;
                }

                var modifiedFolders = folderInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Any(x => (x.CreationTime.ToUniversalTime() > maxTerminationDate) || (x.LastWriteTime.ToUniversalTime() > maxTerminationDate));

                if (modifiedFolders)
                {
                    var message =$"Application folder '{podHistory.FolderPath}' was not deleted because folder has been modified after {maxTerminationDate}";
                    TaskLogger.WriteWarning(message);
                    _lifecycleService.LogPodDHistoryDeletionError(podHistory.PodHistoryId, message);
                    return;
                }

                var modifiedFiles = folderInfo.EnumerateFiles("*", SearchOption.AllDirectories).Any(x => (x.CreationTime.ToUniversalTime() > maxTerminationDate) || (x.LastWriteTime.ToUniversalTime() > maxTerminationDate));
                if (modifiedFiles)
                {
                    var message =$"Application folder '{podHistory.FolderPath}' was not deleted because folder contains files  modified after {maxTerminationDate}";
                    TaskLogger.WriteWarning(message);
                    _lifecycleService.LogPodDHistoryDeletionError(podHistory.PodHistoryId, message);
                    return;
                }
                TaskLogger.WriteInfo($"Not recent files found. Deleting folder: {podHistory.FolderPath}");

                folderInfo.Delete(true);
                TaskLogger.WriteInfo($"folder: {podHistory.FolderPath} deleted. Deleting record from  history...");
                _lifecycleService.DeletePodHistory(new []{ podHistory.PodHistoryId });
                TaskLogger.WriteInfo($"folder: {podHistory.FolderPath} deleted from history");
            }
            catch (Exception ex)
            {
                TaskLogger.WriteError(ex, "undefined", "undefined", false);
                try
                {
                    _lifecycleService.LogPodDHistoryDeletionError(podHistory.PodHistoryId, ex.Message);
                }
                catch(Exception ex2)
                {
                    TaskLogger.WriteError(ex2, "undefined", "undefined", false);
                }
            }
        }
    }
}
