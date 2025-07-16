using System.Net.Http.Headers;

namespace MyNaati.Contracts.BackOffice.Common
{
    public static class Extensions
    {
        public static MediaTypeHeaderValue GetHeaderValue(this FileType fileType)
        {
            return new MediaTypeHeaderValue(fileType.MediaType);
        }

        public static bool IsInteger(this string s)
        {
            int _;
            return int.TryParse(s, out _);
        }
    }
}
