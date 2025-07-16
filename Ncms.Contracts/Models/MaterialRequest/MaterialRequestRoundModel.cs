using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl.Attributes;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;

namespace Ncms.Contracts.Models.MaterialRequest
{
    public class MaterialRequestRoundModel
    {
        public int MaterialRoundId { get; set; }
        public int RoundNumber { get; set; }

        [LookupDisplay(LookupType.MaterialRequestRoundStatus, "StatusTypeName")]
        public int StatusTypeId { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public DateTime DueDate { get; set; }

        public int ModifiedUserId { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public DateTime RequestedDate { get; set; }

        public IList<MaterialRequestDocumentInfoModel> Documents { get; set; }
        public IList<MaterialRequestRoundLinkModel> Links { get; set; }

    }
}
