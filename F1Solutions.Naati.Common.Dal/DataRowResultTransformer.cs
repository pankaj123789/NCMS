using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NHibernate.Transform;

namespace F1Solutions.Naati.Common.Dal
{
    public class DataRowResultTransformer<T> : IResultTransformer
        where T : class, new()
    {
        public bool IgnoreUnmatchedColumns { get; set; }
        public bool IgnoreCase { get; set; }

        public DataRowResultTransformer(bool ignoreUnmatchedColumns = true, bool ignoreCase = true)
        {
            IgnoreUnmatchedColumns = ignoreUnmatchedColumns;
            IgnoreCase = ignoreCase;
        }

        public object TransformTuple(object[] tuple, string[] aliases)
        {
            var result = new T();
            var props = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);


            for (int i = 0; i < aliases.Length; i++)
            {
                var alias = aliases[i];
                var prop = props.SingleOrDefault(x => String.Equals(
                    x.Name, alias,
                    IgnoreCase
                        ? StringComparison.OrdinalIgnoreCase
                        : StringComparison.Ordinal));

                if (prop != null)
                {
                    prop.SetValue(result, tuple[i], null);
                }
                else
                {
                    if (!IgnoreUnmatchedColumns)
                    {
                        throw new Exception(String.Format(
                            "DataRowResultTransformer: no property of the target type matches column '{0}'. If you want to ignore unmatched columns, set IgnoreUnmatchedColumns to true.",
                            alias));
                    }
                }
            }
            return result;
        }

        public IList TransformList(IList collection)
        {
            return collection;
        }
    }
}