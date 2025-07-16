using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Globalization;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.SystemLifecycle;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;

namespace F1Solutions.Naati.Common.Dal
{
    public class ReportingQueryService : IReportingQueryService
    {
        private readonly string _reportingDbConnectionString;
        private const string MessageTemplate = "Log Date: {LogDate}, Entity Name: {EntityName} : {Message}";
        private readonly string _jobName;
        private readonly string _jobDatabaseName;

        public ReportingQueryService(ISecretsCacheQueryService secretsProvider)
        {
            _reportingDbConnectionString = secretsProvider.Get("ReportingDbConnectionString");
            _jobName =ConfigurationManager.AppSettings["ReportingJobName"];
            _jobDatabaseName = ConfigurationManager.AppSettings["ReportingJobDatabaseName"];
        }
        public void ProcessExecuteNcmsReports()
        {
            if (!IsJobRunning(_jobName))
            {
                ExecuteNonQuery($"{_jobDatabaseName}.dbo.sp_start_job", CommandType.StoredProcedure, new SqlParameter("@job_name", _jobName));
            }
            else
            {
                LoggingHelper.LogWarning(MessageTemplate, DateTime.Now, _jobName, "Already running in the background...");
            }
            
        }
        public void ProcessSyncReportLogs()
        {
            bool dataFound;
            do
            {
                dataFound = false;
                var logsToUpdate = new List<string>();
                using (var reader = ExecuteReader("select top 10 JobExecutionLogId from tblJobExecutionLog where SyncDate is null order by JobExecutionLogId ASC"))
                {
                    while (reader.Read() )
                    {
                        dataFound = true;
                        logsToUpdate.Add(reader[0].ToString());
                    }
                }

                if (dataFound)
                {
                    SyncLogs(logsToUpdate);

                    var parameters = string.Join(",", logsToUpdate.Select(x => $"@p{x}"));
                    var sqlParameters = logsToUpdate.Select(x => new SqlParameter($"@p{x}", x)).ToArray();
                    string commandText = $"Update tblJobExecutionLog set SyncDate = GetDate()  where JobExecutionLogId IN ({parameters})";
                    ExecuteNonQuery(commandText, CommandType.Text, sqlParameters);
                }

            } while (dataFound && SystemLifecycleHelper.SystemStatus == SystemLifecycleStatus.Running);
        }
        private string GetSystemValue(string systemValueKey)
        {
            using (var reader = ExecuteReader("select [Value] from tblSystemValue where ValueKey = @ValueKey", new SqlParameter("@ValueKey", systemValueKey)))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return reader[0].ToString();
                }

                return null;
            }
        }
        private void SyncLogs(List<string> logsToUpdate)
        {
            var logsData = new List<ReportLogDto>();
            var parameters = string.Join(",", logsToUpdate.Select(x => $"@p{x}"));
            var sqlParameters = logsToUpdate.Select(x => new SqlParameter($"@p{x}", x)).ToArray();
            using (var reader = ExecuteReader($"Select  JobExecutionLogId, LogDate, EntityName, Message,JobExecutionLogTypeId from tblJobExecutionLog where JobExecutionLogId in ({parameters})", sqlParameters))
            {
                while (reader.Read())
                {
                    logsData.Add(MapLogDate(reader));
                }
            }

            logsData.ForEach(SyncLogData);

        }
        private void SyncLogData(ReportLogDto dto)
        {
            switch (dto.LogType)
            {
                case ReportLogTypeName.Information:
                    LoggingHelper.LogInfo(MessageTemplate,dto.LogDate.ToString(CultureInfo.InvariantCulture), dto.EntityName, dto.Message);
                    break;
                case ReportLogTypeName.Warning:
                    LoggingHelper.LogWarning(MessageTemplate, dto.LogDate.ToString(CultureInfo.InvariantCulture), dto.EntityName, dto.Message);
                    break;
                case ReportLogTypeName.Error:
                    LoggingHelper.LogError(MessageTemplate, dto.LogDate.ToString(CultureInfo.InvariantCulture), dto.EntityName, dto.Message);
                    break;
                default:
                    LoggingHelper.LogError(MessageTemplate, dto.LogDate.ToString(CultureInfo.InvariantCulture), dto.EntityName, dto.Message);
                    break;
            }
        }
        private ReportLogDto MapLogDate(SqlDataReader reader)
        {
            return new ReportLogDto
            {
                LogDate = Convert.ToDateTime(reader[1]),
                EntityName = reader[2].ToString(),
                LogType = (ReportLogTypeName) Convert.ToInt32(reader[4]),
                Message = reader[3].ToString()
            };
        }
        private SqlCommand GetSqlCommand(string commandText, params SqlParameter[] parameters)
        {
            var command = new SqlCommand(commandText, GetConnection());
            command.Parameters.AddRange(parameters);

            if ((command.Connection.State & ConnectionState.Open) == 0)
            {
                command.Connection.Open();
            }
            return command;
        }
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_reportingDbConnectionString);
        }
        private void ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var sqlCommand = GetSqlCommand(commandText, parameters))
            {
               
                sqlCommand.CommandType = commandType;
                sqlCommand.ExecuteNonQuery();
            }
        }
        private SqlDataReader ExecuteReader(string commandText, params SqlParameter[] parameters)
        {
            var sqlCommand = GetSqlCommand(commandText, parameters);
            SqlDataReader reader = sqlCommand.ExecuteReader();
            return reader;
        }
        public void ClearNcmsReportingCache()
        {
            var logHistoryDays = int.Parse(GetSystemValue("LogHistoryDays"));
            var date = DateTime.Now.AddDays(-logHistoryDays);

            var parameter = new SqlParameter("@date", SqlDbType.Date) { Value = date };

            ExecuteNonQuery("DELETE FROM tblJobExecutionLog where SyncDate < @date", parameters: parameter);
        }

        private bool IsJobRunning(string jobName)
        {
            var commandText = $"SELECT count(*) FROM {_jobDatabaseName}.dbo.sysjobactivity AS sja INNER JOIN msdb.dbo.sysjobs AS sj ON sja.job_id = sj.job_id WHERE sja.start_execution_date IS NOT NULL AND sja.stop_execution_date IS NULL AND sj.name = @JobName";
            var parameters = new SqlParameter("@JobName", jobName);
            using (var sqlCommand = GetSqlCommand(commandText, parameters))
            {
                int numRows = (int) sqlCommand.ExecuteScalar();
                if (numRows > 0 )
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
