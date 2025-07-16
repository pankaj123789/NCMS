using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Ncms.Bl.FileDeletion;
using Ncms.Contracts.BackgroundTask;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ncms.Bl.BackgroundTasks
{
    public class ProcessFileDeletesHardDeleteTask : BaseBackgroundTask, IProcessFileDeletesHardDeleteTask
    {
        private readonly IFileDeletionDalService _fileDeletionDalService;
        private readonly IUserQueryService _userQueryService;
        private readonly ISystemQueryService _systemQueryService;

        private FileDeletionRunner FileDeletionRunner;
        private int CurrentUserId = 0;

        protected readonly int BatchSize;

        public ProcessFileDeletesHardDeleteTask(IFileDeletionDalService fileDeletionDalService, IUserQueryService userQueryService, ISystemQueryService systemQueryService, IBackgroundTaskLogger backgroundTaskLogger,IUtilityQueryService utilityQueryService) 
            : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
        {
            _fileDeletionDalService = fileDeletionDalService;
            _userQueryService = userQueryService;
            _systemQueryService = systemQueryService;
        }

        public override void Execute(IDictionary<string, string> parameters)
        {
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

            FileDeletionRunner = new FileDeletionRunner(CurrentUserId, _fileDeletionDalService, TaskLogger, _systemQueryService);

            DeleteFilesInRecycleBin();
        }

        private void DeleteFilesInRecycleBin()
        {
            int.TryParse(GetSystemValue("ProcessFileDeletesPastExpiryDateRetentionDays"), out var retentionDays);

             FileDeletionRunner.DeleteFilesInRecycleBin(retentionDays);
        }

    }
}