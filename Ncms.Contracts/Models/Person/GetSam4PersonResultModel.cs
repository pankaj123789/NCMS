namespace Ncms.Contracts.Models.Person
{
    public class GetSam4PersonResultModel
    {
        public Sam4PersonModel Details { get; set; }
        public string FailMessage { get; set; }

        public string WarnMessage { get; set; }
    }
}
