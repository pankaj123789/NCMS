using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;

namespace F1Solutions.Naati.Common.Contracts.Dal.Request
{
    public class UpsertSessionRolePlayerRequest
    {
        public TestSessionRolePlayerDto TestSessionRolePlayer { get; set; }
        public IList<PersonNoteData> PersonNotes { get; set; }
    }
}