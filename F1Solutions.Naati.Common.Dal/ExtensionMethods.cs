using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NHibernate;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    internal static class ExtensionMethods
    {
        public static string GetValueOrDefault(this NameValueCollection collection, string key, string @default)
        {
            var value = collection[key];

            return string.IsNullOrEmpty(value)
                ? @default
                : value;
        }

        public static bool IsNullOrEmpty(this Guid? guid)
        {
            return guid?.Equals(Guid.Empty) ?? true;
        }

        public static T GetRelated<T>(this int id)
            where T : Entity
        {
            return NHibernateSession.Current.Query<T>().Single(o => o.Id == id);
        }

        public static T GetRelated<T>(this int? id)
            where T : Entity
        {
            if (id.HasValue == false)
                return null;

            if (id.Value == 0)
                return null;

            return id.Value.GetRelated<T>();
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T ToIdenticalEnum<T>(this object input)
            where T : new()
        {
            var outputType = typeof(T);
            var outputTypeIsNullable = outputType.IsGenericType && outputType.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (input == null)
            {
                if (outputTypeIsNullable)
                {
                    return new T();
                }

                //This is almost certainly indicative of an accidentally omitted '?'.
                var error = $"The input to this is nullable. The generic parameter T should be as well, but it's actually '{outputType.FullName}'";
                throw new Exception(error);
            }

            var enumType = outputTypeIsNullable ? Nullable.GetUnderlyingType(outputType) : outputType;

            return (T)Enum.Parse(enumType, input.ToString());
        }

        /// <summary>
        /// Performs a string.Format and returns blank if result is identical to one with null arguments
        /// </summary>
        /// <remarks>
        /// This overload is here because NHibernate.Linq doesn't seem to do Select() with the params version.
        /// </remarks>
        public static string FormatAndCheckBlank(this string formatString, object arg1, object arg2)
        {
            return formatString.FormatAndCheckBlank(new[] { arg1, arg2 });
        }

        /// <summary>
        /// Performs a string.Format and returns blank if result is identical to one with null arguments
        /// </summary>
        /// <remarks>
        /// This is to avoid getting weird items like " - " from format strings of "{0} - {1}".
        /// </remarks>
        public static string FormatAndCheckBlank(this string formatString, params object[] formatArgs)
        {
            var blankArgs = new object[formatArgs.Length];
            var formatResult = string.Format(formatString, formatArgs);
            var blankResult = string.Format(formatString, blankArgs);

            return formatResult == blankResult ? "" : formatResult;
        }

        public delegate bool TryParse<T>(string s, out T result) where T : struct;

        public static IEnumerable<T?> Parse<T>(this IEnumerable<string> source, TryParse<T> tryParse) where T : struct
        {
            return source.Select(x =>
            {
                T result;
                return tryParse(x, out result) ? result : (T?)null;
            });
        }

        public static IEnumerable<int?> ParseInt(this IEnumerable<string> source)
        {
            return source.Parse<int>(int.TryParse);
        }

        public static IEnumerable<T> WhereHasValue<T>(this IEnumerable<T?> source) where T : struct
        {
            return source.Where(x => x.HasValue).Select(x => x.Value);
        }

        public static IEnumerable<int> WhereParseInt(this IEnumerable<string> source)
        {
            return source.ParseInt().WhereHasValue();
        }

        public static IList<T> TakeIf<T>(this IList<T> source, int count, bool take, out int originalCount)
        {
            originalCount = source.Count;
            return take ? source.Take(count).ToList() : source;
        }

        public static IList<T> LimitSearchResults<T>(this IList<T> source, out bool searchLimited, out int originalCount)
        {
            var searchResultLimit = int.Parse(ConfigurationManager.AppSettings["SearchResultLimit"]);
            searchLimited = source.Count > searchResultLimit;

            return source.TakeIf(searchResultLimit, searchLimited, out originalCount).ToArray();
        }

        public static IList<T> TransformSqlQueryResult<T>(this ISession session, IResultTransformer resultTransformer, string sqlQuery)
        {
            return session.CreateSQLQuery(sqlQuery).SetResultTransformer(resultTransformer).List<T>();
        }

        public static IList<T> TransformSqlQueryDataRowResult<T>(this ISession session, string sqlQuery) where T : class, new()
        {
            return session.TransformSqlQueryResult<T>(new DataRowResultTransformer<T>(), sqlQuery);
        }

        public static IList<T> TransformSqlQueryAliasToBeanResult<T>(this ISession session, string sqlQuery)
        {
            return session.TransformSqlQueryResult<T>(Transformers.AliasToBean(typeof(T)), sqlQuery);
        }
    }
}
