using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class CandidateBrief : EntityBase
    {
        public virtual TestSitting TestSitting { get; set; }
        public virtual TestMaterialAttachment TestMaterialAttachment { get; set; }
        public virtual DateTime? EmailedDate { get; set; }
    }
}
