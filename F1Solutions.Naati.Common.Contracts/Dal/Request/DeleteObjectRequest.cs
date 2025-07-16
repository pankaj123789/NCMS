using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class DeleteObjectRequest
    {
        public int ObjectId { get; set; }
        public List<KeyValuePair<string, bool>> FlowAnswers { get; set; }
    }
}