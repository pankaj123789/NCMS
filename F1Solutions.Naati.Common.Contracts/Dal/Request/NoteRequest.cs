using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class NoteRequest
    {
        public NoteType NoteType { get; set; }
        public int EntityId { get; set; }
    }
}