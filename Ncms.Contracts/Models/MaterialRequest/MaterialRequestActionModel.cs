using System.Collections.Generic;
using Ncms.Contracts.Models.System;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestActionModel : SystemActionModel
    {
        public IList<MaterialRequestRoundModel> Rounds { get; set; }

        public MaterialRequestInfoModel MaterialRequestInfo { get; set; }
        public IList<MaterialRequestPersonNoteModel> PersonNotes { get; set; }
        public IList<MaterialRequestActionNoteModel> Notes { get; set; }
        public IList<MaterialRequestActionPublicNoteModel> PublicNotes { get; set; }

        public IList<OutputTestMaterialDocumentInfoModel> Documents { get; set; }
    }
}
