using System.IO;

namespace F1Solutions.Naati.Common.Azure.Storage
{
    internal class StorageAccountConfiguration
    {
        public string AccountName { get; }

        public string AccountKey { get; }

        public string ShareName { get; }

        public DirectoryInfo MappedDirectory { get; }

        public StorageAccountConfiguration(string accountName, string accountKey, string shareName, string mappedDirectory)
        {
            AccountName = accountName;
            AccountKey = accountKey;
            ShareName = shareName;
            MappedDirectory = new DirectoryInfo(mappedDirectory);
        }
    }
}
