using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Transaction;

namespace F1Solutions.Naati.Common.Dal.NHibernate.Extensions
{
    public static class DataAccessExtensions
    {
        private static readonly FieldInfo mTransInfo = typeof(AdoTransaction).GetField("trans", BindingFlags.NonPublic | BindingFlags.Instance);

        public static SqlTransaction GetSqlTransaction(this ITransaction nhTransaction)
        {
            return (SqlTransaction)mTransInfo.GetValue(nhTransaction);
        }

        public static T ParseRelation<T>(this object value)
        {
            if (value == DBNull.Value || value == null)
                return default(T);

            return NHibernateSession.Current.Get<T>(value);
        }

        public static T ParseDBNull<T>(this object value) 
        {
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T) value;
        }

        public static DateTime? ParseNullableDate(this object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            return (DateTime)value;
        }

        public static int? ParseNullableInteger(this object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            return (int)value;
        }

        public static decimal? ParseNullableDecimal(this object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            return (decimal)value;
        }

        public static void DeleteList<T>(this ISession session, List<T> objects)
        {
            foreach (object item in objects)
                session.Delete(item);
        }

        public static void SaveList<T>(this ISession session, List<T> objects)
        {
            if (objects == null)
                return;

            foreach (object item in objects)
                session.Save(item);
        }
    }
}