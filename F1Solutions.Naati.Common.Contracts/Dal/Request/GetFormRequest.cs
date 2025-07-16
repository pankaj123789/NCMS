namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class GetFormRequest
    {
        public bool Public { get; set; }
        public bool Practitioner { get; set; }
        public bool Private { get; set; }
        public bool Recertification { get; set; }
        public bool NonPractitioner { get; set; }
    }
}