namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class ServiceResponse
    {
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string StackTrace { get; set; }
    }
    
    public class ServiceResponse<T> : ServiceResponse
    {
        public T Data { get; set; }
    }
}
