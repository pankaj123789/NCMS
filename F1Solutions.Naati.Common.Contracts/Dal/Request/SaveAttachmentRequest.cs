using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveAttachmentRequest
    {
        [MessageHeader]
        public int TestResultId { get; set; }
        [MessageHeader]
        public int NAATINumber { get; set; }
        [MessageHeader]
        public string Title { get; set; }
        public string FilePath { get; set; }
        public int Type { get; set; }
    }
}