namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class UserCheckResponse
    {
        public bool IsUserExist { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}