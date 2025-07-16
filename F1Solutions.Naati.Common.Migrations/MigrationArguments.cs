using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Migrations
{
    public static class MigrationArguments
    {
        private static Dictionary<string, string> mMigrationArguments = new Dictionary<string, string>();

        public static void Reset()
        {
            mMigrationArguments = new Dictionary<string, string>();
        }

        public static void AcceptCommandLineArgs(string[] args)
        {
            for (int iArg = 0; iArg < args.Length; iArg++)
            {
                var currentArg = args[iArg];
                if (currentArg.StartsWith("-") == false)
                {
                    //not a key
                    continue;
                }

                if ((iArg + 1) >= args.Length)
                {
                    //last arg; can't possibly have value
                    continue;
                }

                var nextArg = args[iArg + 1];
                if (nextArg.StartsWith("-"))
                {
                    //next arg is a key; can't be a value for this arg
                    continue;
                }

                var key = currentArg.Substring(1);
                var value = nextArg;

                AddCommandLineArg(key, value);
            }
        }

        public static void AddCommandLineArg(string key, string value)
        {
            mMigrationArguments.Add(key.ToLower(), value);
        }

        public static bool ArgumentExists(string key)
        {
            return mMigrationArguments.ContainsKey(key.ToLower());
        }

        public static string GetArgument(string key, bool throwException = true)
        {
            if (!ArgumentExists(key.ToLower()))
            {
                if (!throwException)
                {
                    return null;
                }

                var error = string.Format("Expected argument for key '{0}'. You can pass it in the command line like '-{0} \"Some argument\"'", key);

                throw new Exception(error);
            }

            return mMigrationArguments[key.ToLower()];
        }
    }
}
