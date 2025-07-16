using System;
using System.Collections.Generic;
using System.Configuration;

namespace MyNaati.Bl.BackOffice.Helpers
{
    public class ApplicationSettingsHelper
    {
        private const string ExcludedNamesListKey = "ExcludedNamesList";
        private static readonly List<string> DefaultExcludedNameList = new List<string>();
        private const string IncludedFileExtensionsListKey = "IncludedFileExtensionsList";
        private static readonly List<string> DefaultIncludedFileExtensionsList = new List<string>();

        public static List<string> ExcludedNamesList
        {
            get
            {
                var value = GetAppSetting<string>(ExcludedNamesListKey);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return DefaultExcludedNameList;
                }

                try
                {
                    var list = new List<string>();
                    list.AddRange(value.Split(','));
                    return list;
                }
                catch (Exception)
                {
                    return DefaultExcludedNameList;
                }
            }
        }

        public static List<string> IncludedFileExtensionsList
        {
            get
            {
                var value = GetAppSetting<string>(IncludedFileExtensionsListKey);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return DefaultIncludedFileExtensionsList;
                }

                try
                {
                    var list = new List<string>();
                    list.AddRange(value.Split(','));
                    return list;
                }
                catch (Exception)
                {
                    return DefaultIncludedFileExtensionsList;
                }
            }
        }

        private static T GetAppSetting<T>(string key, bool throwException = false)
        {
            try
            {
                var setting = ConfigurationManager.AppSettings[key];
                return (T)Convert.ChangeType(setting, typeof(T));
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }

                return default(T);
            }
        }
    }
}