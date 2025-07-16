using System;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestMaterialLastUsed : EntityBase
    {
        public virtual TestMaterial TestMaterial { get; set; }
        public virtual DateTime? LastUsedDate { get; set; }
    }
}
