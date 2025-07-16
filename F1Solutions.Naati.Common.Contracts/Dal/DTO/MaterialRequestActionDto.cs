using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestActionDto
    {
        public MaterialRequestInfoDto MaterialRequestInfo { get; set; }
        public IList<MaterialRequestRoundDto> Rounds { get; set; }

        public IList<MaterialRequestPersonNoteDto> PersonNotes { get; set; }
        public IList<MaterialRequestActionNoteDto> Notes { get; set; }
        public IList<MaterialRequestActionPublicNoteDto> PublicNotes { get; set; }

        public IList<OutputTestMaterialDocumentInfoDto> Documents { get; set; }
    }
}
