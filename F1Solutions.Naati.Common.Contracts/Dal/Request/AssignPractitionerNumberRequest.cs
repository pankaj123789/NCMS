namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class AssignPractitionerNumberRequest
    {
        public int PersonId { get; set; }
        public string PractitionerNumber { get; set; }
    }
}