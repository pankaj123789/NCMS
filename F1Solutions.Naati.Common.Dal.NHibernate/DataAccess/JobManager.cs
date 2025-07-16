using System.Data;
using System.Data.SqlClient;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal.NHibernate.DataAccess
{
    public static class JobManager
    {

        public static bool RequestJobToken(string jobName)
        {
            return ExecuteAction((SqlConnection)NHibernateSession.Current.Connection, NHibernateSession.Current.Transaction, jobName, "RequestJobToken");
        }

        public static bool ReleaseJobToken(string jobName)
        {
            return ExecuteAction((SqlConnection)NHibernateSession.Current.Connection, NHibernateSession.Current.Transaction, jobName, "ReleaseJobToken");
        }
        
        private static bool ExecuteAction(SqlConnection connection, global::NHibernate.ITransaction transaction, string jobName, string storeProcedureName)
        {
            SqlParameter tableNameParam = new SqlParameter("@JobName", jobName);
            SqlParameter keyParam = new SqlParameter("@Success", SqlDbType.Bit);
            keyParam.Direction = ParameterDirection.Output;

            bool openedConnection = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                openedConnection = true;
            }

            SqlCommand command = new SqlCommand(storeProcedureName, connection) {CommandType = CommandType.StoredProcedure};

            command.Parameters.Add(tableNameParam);
            command.Parameters.Add(keyParam);

            transaction?.Enlist(command);

            command.ExecuteNonQuery();

            if (openedConnection == true)
            {
                connection.Close();
            }

            return (bool)keyParam.Value;
        }
    }
}
