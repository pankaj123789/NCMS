using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class DeleteObjectResponse
    {
        public bool Success { get; set; }
        public KeyValuePair<string, string> FlowMessage { get; set; }
    }
}