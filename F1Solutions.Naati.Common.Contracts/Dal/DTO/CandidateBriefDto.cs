using System;

namespace F1Solutions.Naati.Common.Contracts.Dal.DTO
{
    public class CandidateBriefDto
    {
        public int CandidateBriefId { get; set; }
        public int TestMaterialAttachmentId { get; set; }
        public int TestSittingId { get; set; }
        public DateTime? EmailedDate { get; set; }
    }
}