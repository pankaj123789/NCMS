namespace F1Solutions.Naati.Common.Contracts.Dal
{
    public class Sam4PhoneData
    {
        public string LocalNumber { get; set; }
        public string Note { get; set; }
        public bool PrimaryContact { get; set; }
        public bool IncludeInPd { get; set; }
        public bool AllowSmsNotification { get; set; }
        public bool Invalid { get; set; }
    }
}