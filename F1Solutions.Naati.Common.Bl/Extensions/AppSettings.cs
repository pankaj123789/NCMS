using System.Configuration;
using System.Globalization;
using System.Linq;

namespace F1Solutions.Naati.Common.Bl.Extensions
{
    public static class AppSettings
    {
        public const int DefaultSearchResultLimit = 500;

        public static int SearchResultLimit => SettingOrDefault("SearchResultLimit", DefaultSearchResultLimit);

        public static T SettingOrDefault<T>(string key)
        {
            return SettingOrDefault(key, default(T));
        }

        public static T SettingOrDefault<T>(string key, T defaultValue)
        {
            T value;

            try
            {
                value = Setting<T>(key);
            }
            catch (ConfigurationErrorsException)
            {
                value = defaultValue;
            }

            return value;
        }

        public static T[] SettingList<T>(string key, char seperator = ',')
        {
            return Setting<string>(key).Split(seperator).Select(x => Convert<T>(x.Trim())).ToArray();
        }

        public static T Setting<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[$"sam:{key}"];

            if (value == null)
            {
                throw new ConfigurationErrorsException($"Could not find app setting '{key}',");
            }

            return Convert<T>(value);
        }

        private static T Convert<T>(object value)
        {
            return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
