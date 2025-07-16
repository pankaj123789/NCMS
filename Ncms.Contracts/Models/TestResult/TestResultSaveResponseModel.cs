namespace Ncms.Contracts.Models.TestResult
{
    public class TestResultSaveResponseModel
    {
        public string ReprocessingFlow { get; set; }
        public TestResultSaveMessageModel BlockMessage { get; set; }
        public bool Success { get; set; }
    }

    public class TestResultSaveMessageModel
    {
        public string Caption { get; set; }
        public string Message { get; set; }
        public TestResultSaveMessageTypeModel Type { get; set; }
    }

    public enum TestResultSaveMessageTypeModel
    {
        Confirm,
        Alert
    }

}
