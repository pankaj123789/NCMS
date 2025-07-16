namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetSystemValueRequest
    {
        public string ValueKey { get; set; }
        public bool ForceRefresh { get; set; }
    }
}