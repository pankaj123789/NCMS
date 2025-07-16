using System.Data;
using System.Data.SqlClient;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;

namespace F1Solutions.Naati.Common.Dal.NHibernate.DataAccess
{
    public static class KeyAllocation
    {
        public static int GetSingleKey(string tableName)
        {
            return GetSingleKey((SqlConnection)NHibernateSession.Current.Connection, NHibernateSession.Current.Transaction, tableName);
        }

        public static int GetSingleKey(SqlConnection connection, string tableName)
        {
            return GetSingleKey(connection, null, tableName);
        }

        private static int GetSingleKey(SqlConnection connection, global::NHibernate.ITransaction transaction, string tableName)
        {
            SqlParameter tableNameParam = new SqlParameter("@TableName", tableName);
            SqlParameter keyParam = new SqlParameter("@NextKey", SqlDbType.Int);
            keyParam.Direction = ParameterDirection.Output;

            bool openedConnection = false;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                openedConnection = true;
            }

            SqlCommand command = new SqlCommand("GetSingleKey", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(tableNameParam);
            command.Parameters.Add(keyParam);

            if (transaction != null)
            {
                transaction.Enlist(command);
            }

            command.ExecuteNonQuery();

            if (openedConnection == true)
            {
                connection.Close();
            }

            return (int)keyParam.Value;
        }
    }
}