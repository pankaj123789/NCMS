using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.Response
{
    public class UpsertApplicationResponse
    {
        public int CredentialApplicationId { get; set; }
        public IEnumerable<int> CredentialRequestIds { get; set; }
        public IEnumerable<int> ApplicationFieldDataIds { get; set; }
        public IEnumerable<int> NoteIds { get; set; }
    }
}