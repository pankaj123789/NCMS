namespace F1Solutions.Naati.Common.Dal.NHibernate.Configuration
{
    public static class DialectExtensions
    {
        public static bool FullTextContains(this string source, string pattern)
        {
            return false;
        }
    }
}
