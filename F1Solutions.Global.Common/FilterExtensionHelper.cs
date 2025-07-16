using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace F1Solutions.Global.Common
{
    public static class FilterExtensionHelper
    {

        private const string IntegerFilterEnding = "Int";
        private const string DoubleFilterEnding = "Double";
        private const string BooleanFilterEnding = "Boolean";
        private const string StringFilterEnding = "String";
        private const string ListFilterEnding = "List";

        public static IEnumerable<TS> ToFilterList<TS, T>(this string json) where TS : ISearchCriteria<T>, new()
        {
            if (string.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<TS>();
            }

           var filterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
           var dictionary = filterDictionary.Where(x => x.Value != null).ToDictionary(x => (T)Enum.Parse(typeof(T), x.Key.ToString()), y => y.Value);

            var convertedObjects = new List<TS>();
            foreach (var keyValue in dictionary)
            {
                var filterObject = (TS)Activator.CreateInstance(typeof(TS));
                filterObject.Filter = keyValue.Key;
                filterObject.Values = GetFilterValueFrom(keyValue);
                convertedObjects.Add(filterObject);
            }

            return convertedObjects;
        }

        public static bool CanConvertToListOf<T>(this ISearchCriteria<T> filterCriteria, Type destinationType)
        {
            var validationDictionary = new Dictionary<Type, Func<string, bool>>
            {
                {typeof(double), CanConvertToDouble},
                {typeof(bool), CanConvertToBoolean},
                {typeof(int), CanConverToInt},
                {typeof(string), filterName => true},
            };

            Func<string, bool> validator;
            if (validationDictionary.TryGetValue(destinationType, out validator))
            {
                return validator(filterCriteria.Filter.ToString());
            }
            return false;
        }

        private static bool CanConvertToDouble(string filterName)
        {
            return filterName.EndsWith(IntegerFilterEnding) || filterName.EndsWith(IntegerFilterEnding + ListFilterEnding)
                   || filterName.EndsWith(DoubleFilterEnding) || filterName.EndsWith(BooleanFilterEnding + ListFilterEnding);
        }

        public static bool CanConvertToBoolean(string filterName)
        {
            return filterName.EndsWith(BooleanFilterEnding) || filterName.EndsWith(BooleanFilterEnding + ListFilterEnding);
        }

        public static bool CanConverToInt(string filterName)
        {
            return filterName.EndsWith(IntegerFilterEnding) || filterName.EndsWith(IntegerFilterEnding + ListFilterEnding);
        }

        public static List<TR> ToList<T, TR>(this ISearchCriteria<T> filterCriteria)
        {
            if (!filterCriteria.CanConvertToListOf(typeof(TR)))
            {
                throw new Exception($"Filter Criteria: {filterCriteria.Filter} can't be converted to {typeof(TR)} list!");
            }

            var convertionDictioanry = new Dictionary<Type, Func<string, object>>
            {
                {typeof(int), value => Convert.ToInt32(value)},
                {typeof(double), value => Convert.ToDouble(value)},
                {typeof(bool), value => Convert.ToBoolean(value)},
            };

            var conversionMethod = convertionDictioanry[typeof(TR)];
            return filterCriteria.Values.Select(x => (TR)conversionMethod(x)).ToList();
        }

        private static IList<string> GetFilterValueFrom<T>(KeyValuePair<T, object> filter)
        {
            var filterKey = filter.Key.ToString();
            if (filterKey.EndsWith(ListFilterEnding))
            {
                return ParseStringList(filter.Value);
            }

            if (filter.Value is DateTime)
            {
               return new List<string> { ((DateTime)filter.Value).ToFilterString()}; 
            }

            return new List<string> { filter.Value.ToString() };
        }

        private static List<string> ParseStringList(object value)
        {
            return JsonConvert.DeserializeObject<List<string>>(value.ToString());
        }

        public static T CreateCriteria<T, S>(S filter, IEnumerable<string> values) where T : ISearchCriteria<S>, new()
        {
            var criteria = (T)Activator.CreateInstance(typeof(T));
            criteria.Filter = filter;
            criteria.Values = values;
            return criteria;
        }
        public static string ToFilterString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd hh:mm:ss");
        }
    
    }
}
