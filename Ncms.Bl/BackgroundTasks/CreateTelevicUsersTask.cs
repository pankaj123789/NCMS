//using System.Collections.Generic;
//using System.Linq;
//using F1Solutions.Naati.Common.Bl.BackgroundTask;
//using F1Solutions.Naati.Common.Contracts.Bl.BackgroundTask;
//using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
//using F1Solutions.Naati.Common.Contracts.Dal.Services.Televic;
//using Ncms.Contracts.BackgroundTask;

//namespace Ncms.Bl.BackgroundTasks
//{
//    public class CreateTelevicUsersTask : BaseBackgroundTask, ICreateTelevicUsersTask
//    {
//        private ITelevicIntegrationService _televicIntegrationService;
//        private readonly ITestSessionQueryService _testSessionQueryService;

//        public CreateTelevicUsersTask(ISystemQueryService systemQueryService,
//            IBackgroundTaskLogger backgroundTaskLogger,
//            IUtilityQueryService utilityQueryService,
//            ITelevicIntegrationService televicIntegrationService,
//            ITestSessionQueryService testSessionQueryService) : base(systemQueryService, backgroundTaskLogger, utilityQueryService)
//        {
//            _televicIntegrationService = televicIntegrationService;
//            _testSessionQueryService = testSessionQueryService;
//        }

//        public override void Execute(IDictionary<string, string> parameters)
//        {
//            CreateTelevicUsers();
//        }

//        private void CreateTelevicUsers()
//        {
//            var sessionDateHoursText = GetSystemValue("CreateTelevicUserSessionHours");
//            var sessionHours = int.Parse(sessionDateHoursText);
//            var testSessions = _testSessionQueryService.GetTestSittingsWithUsersProj(sessionHours
//                ).GroupBy(x => x.TestSessionId);


//            foreach (var testSession in testSessions)
//            {
//                var testGropBySkill = testSession.GroupBy(x => x.SkillId);

//                foreach(var session in testGropBySkill)
//                {
//                    string groupName = $"{session.First().TestSessionDate.ToString("dd/MM/yyyy")} {session.First().TestSessionName} {session.First().SkillName}"; 
//                    //upsert users for each test session
//                    _televicIntegrationService.UpsertUsers(session, groupName);                
//                }
//                //update synced flag
//                _testSessionQueryService.MarkAsSynced(testSession.Key);
//            }
//        }
//    }
//}
