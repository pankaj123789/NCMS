using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using F1Solutions.Naati.Common.Dal.NHibernate.DataAccess;
using NHibernate.Persister.Entity;

namespace F1Solutions.Naati.Common.Dal
{

    public class UtilityQueryService : IUtilityQueryService
    {
        private readonly IApplicationQueryService _applicationQueryService;

        public UtilityQueryService(IApplicationQueryService applicationQueryService)
        {
            _applicationQueryService = applicationQueryService;
        }

        public EntityRecordsResponse GetEntityRecords(string entityName)
        {
            var foundMetadata = NHibernateSession.Current.SessionFactory.GetAllClassMetadata().Single(x => x.Key.ToUpper().EndsWith("." + entityName.ToUpper()));
            var entityMetadata = (AbstractEntityPersister)NHibernateSession.Current.SessionFactory.GetClassMetadata(foundMetadata.Value.MappedClass);

            var propertyNames = entityMetadata.PropertyNames.Select(x => entityMetadata.GetPropertyColumnNames(x)[0])
                .Union(entityMetadata.KeyColumnNames).ToList();
            var tableName = entityMetadata.TableName;

            var query = $"SELECT {string.Join(",", propertyNames)} FROM {tableName}";

            var result = NHibernateSession.Current.CreateSQLQuery(query).List<object[]>();

            var values = result.AsParallel().Select(x => x.Select(y => y?.ToString()).ToList()).ToList();
            return new EntityRecordsResponse
            {
                PropertyNames = propertyNames,
                PropertyValues = values
            };
        }

        public LookupTypeResponse GetLookupType(LookupType lookupType)
        {
            // Todo: Move implementation to this service
            return _applicationQueryService.GetLookupType(lookupType.ToString());
        }

        public void LogSystemTerminationDate()
        {
            var machineName = GetMachineName();

            var podHistory = NHibernateSession.Current.Query<PodHistory>().FirstOrDefault(x => x.PodName == machineName);
            if (podHistory == null)
            {
                LoggingHelper.LogWarning($"Pod: {machineName} was not found in the pod history");
                return;
            }
            podHistory.TerminationDate = DateTime.Now;

            NHibernateSession.Current.SaveOrUpdate(podHistory);

            LoggingHelper.LogInfo($"System Termination date logged for Pod: {machineName}");
            //for (var i = 0; i < 70 ; i++)
            //{
            //    Thread.Sleep(5000);
            //    LoggingHelper.LogInfo($"System Termination date logged for Pod: {machineName} {DateTime.Now.Second}");
            //}
        }

        public void LogSystemStartDate()
        {
            var machineName = GetMachineName();

            var podHistory = NHibernateSession.Current.Query<PodHistory>().FirstOrDefault(x => x.PodName == machineName);

            if (podHistory != null)
            {
                LoggingHelper.LogWarning($"Pod:{machineName} already exists in history. Started date is: '{podHistory.StartedDate}' and termination Date is : '{podHistory.TerminationDate?.ToString()}'. Pod Registry will be updated");
            }
            else
            {
                podHistory = new PodHistory { PodName = GetMachineName() };
            }

            var applicationFolder = Path.Combine(ConfigurationManager.AppSettings["applicationData"], machineName);
            podHistory.FolderPath = applicationFolder;
            podHistory.TerminationDate = null;
            podHistory.StartedDate = DateTime.Now;
            podHistory.DeletionError = null;
            NHibernateSession.Current.SaveOrUpdate(podHistory);
            LoggingHelper.LogInfo($"System Start date logged for Pod: {machineName}");
        }

        //private string GetLocalIp()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    var localIp = string.Empty;
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            localIp = ip.ToString();
        //        }
        //    }

        //    var addessList = string.Join(";", host.AddressList.Select(x=> x.MapToIPv4().ToString()));
        //    var ipAddessList = string.Join(";", host.AddressList.Select(x=> x.ToString()));
        //    LoggingHelper.LogInfo($"LOCAL IP HostName {host.HostName}");
        //    LoggingHelper.LogInfo($"LOCAL IPs AddressList IPV4 {addessList}");
        //    LoggingHelper.LogInfo($"LOCAL IPs AddressList toSTRING {ipAddessList}");
        //    return localIp;
        //}

        //public bool IsLocalIp(string ipAddress)
        //{
        //    var address = IPAddress.Parse(ipAddress);
        //    var ipv4Ip = address.MapToIPv4().ToString();
        //    LoggingHelper.LogInfo($"User ip {ipv4Ip}, Local Ip {IPAddress.Parse("127.0.0.1").MapToIPv4()}");
        //    return ipv4Ip == IPAddress.Parse("127.0.0.1").MapToIPv4().ToString();
        //}

        public IEnumerable<PodHistoryDto> GetTerminatedPods(GetTerminatedPodsRequest request)
        {
            return NHibernateSession.Current.Query<PodHistory>()
                .Where(x => (x.TerminationDate != null) && (x.DeletionError == null) && (x.TerminationDate < request.MaxTerminationDate))
                .Take(request.Take)
                .Select(
                    r => new PodHistoryDto
                    {
                        FolderPath = r.FolderPath,
                        PodName = r.PodName,
                        StartedDate = r.StartedDate,
                        TerminationDate = r.TerminationDate,
                        PodHistoryId = r.Id
                    }).ToList();
        }

        public void DeletePodHistory(IEnumerable<int> podHistoryIds)
        {
            var dataIds = podHistoryIds.Concat(new[] { 0 }).ToList();

            var dataToDelete = NHibernateSession.Current.Query<PodHistory>().Where(x => dataIds.Contains(x.Id));
            foreach (var podHistory in dataToDelete)
            {
                NHibernateSession.Current.Delete(podHistory);
            }
        }

        public void LogPodDHistoryDeletionError(int podHistoryId, string errorMessage)
        {
            var dataToDelete = NHibernateSession.Current.Load<PodHistory>(podHistoryId);
            dataToDelete.DeletionError = errorMessage;
            NHibernateSession.Current.SaveOrUpdate(dataToDelete);
        }

        public string GetMachineName()
        {
            var machineName = System.Net.Dns.GetHostEntry("").HostName;
            return machineName;
        }

        public string GetSystemIp()
        {
            var podIp = ConfigurationManager.AppSettings["PodIp"];
            return podIp;
        }

        public bool RequestJobToken<TJobType>(TJobType jobTypeName)
        {
            return JobManager.RequestJobToken(jobTypeName.ToString());
        }

        public bool ReleaseJobToken<TJobType>(TJobType jobTypeName)
        {
            return JobManager.ReleaseJobToken(jobTypeName.ToString());
        }
    }
}
