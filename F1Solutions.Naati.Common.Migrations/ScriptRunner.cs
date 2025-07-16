using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace F1Solutions.Naati.Common.Migrations
{
    public class ScriptRunner : IDisposable
    {
        private bool _disposed;

        private const string SystemValueTable = "tblSystemValue";
        public string BuildVersionKey => "BuildVersion";

        protected readonly string ConnectionString;

        protected IDictionary<string, string> DbNameTokenMappings;

        protected SqlConnection Connection;

        public ScriptRunner(string connectionString, IDictionary<string, string> dbNameTokenMappings)
        {
            ConnectionString = connectionString;
            DbNameTokenMappings = dbNameTokenMappings;

            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

        public void RunScript(string script)
        {
            try
            {
                using (var command = Connection.CreateCommand())
                {
                    command.CommandTimeout = 600;
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception($"Exception running: {script.Substring(0, Math.Min(script.Length, 200))}", ex);
            }
        }

        public virtual void RunPostMigrationScripts() { }
        protected virtual void RunVersionInfoMaintenanceScripts() { }
        protected virtual void RunSystemValueMaintenanceScripts() { }

        public bool DbObjectExists(string objectName, string objectOwner)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[{objectOwner}].[{objectName}]')";

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        public bool TableRowExists(string tableName, string columnName, string columnValue)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = '{columnValue}'";

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        public bool IsIdentityColumn(string tableName, string columnName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT IS_IDENTITY FROM SYS.COLUMNS WHERE[OBJECT_ID] = OBJECT_ID('{tableName}')  and name = '{columnName}'";

                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Creates or updates a database object (requires the object to support the ALTER statement)
        /// </summary>
        protected void CreateOrUpdateDbObject(string objectName, string objectOwner, string script)
        {
            RunScript(GetCreateOrUpdateDbObjectScript(objectName, objectOwner, script));
        }

        /// <summary>
        /// Get the Create or update script for a database object (requires the object to support the ALTER statement)
        /// </summary>
        public string GetCreateOrUpdateDbObjectScript(string objectName, string objectOwner, string script)
        {
            script = script.Trim();
            script = RemoveFromStart(script, "CREATE", StringComparison.InvariantCultureIgnoreCase);
            script = RemoveFromStart(script, "ALTER", StringComparison.InvariantCultureIgnoreCase);
            script = script.Trim();

            script = (DbObjectExists(objectName, objectOwner) ? "ALTER " : "CREATE ") + script;

            return DbNameTokenMappings.Aggregate(script, (current, dbNameTokenMapping) => current.Replace("[" + dbNameTokenMapping.Key + "]", "[" + dbNameTokenMapping.Value + "]"));
        }

        private static string RemoveFromStart(string s, string stringToRemove, StringComparison options)
        {
            return s.StartsWith(stringToRemove, options)
                ? s.Remove(0, stringToRemove.Length)
                : s;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            Connection.Close();
            Connection.Dispose();
        }

        public string Escape(string s)
        {
            return s.Replace("'", "''");
        }

        ~ScriptRunner()
        {
            Dispose();
        }
        
        public void UpdateDbVersion(string assemblyFileVersion)
        {
            if (!TableRowExists($"{SystemValueTable}", "VALUEKEY", BuildVersionKey))
            {
               throw  new  Exception($"System value key {BuildVersionKey} does not exists" );
            }

            RunScript($"UPDATE {SystemValueTable} SET value = '{assemblyFileVersion}' where ValueKey = '{BuildVersionKey}'");
            SetLastRunDate();
        }

        private void SetLastRunDate()
        {
            RunScript($"UPDATE {SystemValueTable} SET Value = CONVERT(Varchar, GETDATE(), 20), ModifiedDate = GETDATE() WHERE ValueKey = 'DatabaseUpdaterLastRun'");
        }

        public string GetLastRunVersion()
        {
            if (!TableRowExists($"{SystemValueTable}", "VALUEKEY", BuildVersionKey))
            {
                throw new Exception($"System value key {BuildVersionKey} does not exists");
            }

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = $"SELECT VALUE FROM {SystemValueTable} WHERE VALUEKEY = '{BuildVersionKey}'";
                using (var dataReader = command.ExecuteReader())
                {
                    dataReader.Read();
                    return dataReader[0].ToString();
                }
            }
        }
    }

  

    public enum ScriptSEnvironmentName
    {
        Dev,
        Test,
        Uat,
        Prod
    }
}
