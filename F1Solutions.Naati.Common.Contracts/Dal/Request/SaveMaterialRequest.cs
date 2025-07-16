using System.ServiceModel;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class SaveMaterialRequest
    {
        [MessageHeader]
        public int TestMaterialId { get; set; }
        [MessageHeader]
        public int NAATINumber { get; set; }
        [MessageHeader]
        public string Title { get; set; }
        public string FilePath { get; set; }
    }
}