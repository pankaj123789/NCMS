using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class MaterialRequestRoundDto
    {
        public int MaterialRoundId { get; set; }
        public int RoundNumber { get; set; }
        public int StatusTypeId { get; set; }
        public string StatusTypeDisplayName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime StatusChangeDate { get; set; }
        public DateTime DueDate { get; set; }

        public int ModifiedUserId { get; set; }
        public DateTime RequestedDate { get; set; }

        public IList<MaterialRequestDocumentInfoDto> Documents { get; set; }
        public IList<MaterialRequestRoundLinkDto> Links { get; set; }
    }
}
