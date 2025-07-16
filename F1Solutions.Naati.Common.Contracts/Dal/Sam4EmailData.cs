namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class Sam4EmailData
    {
        public bool IsPreferredEmail { get; set; }
        public bool IncludeInPd { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public bool Invalid { get; set; }
    }
}