using System.IO;

namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class GetFileResponseMapped
    {
        public string FileName { get; set; }
        public Stream FileData { get; set; }
    }
}