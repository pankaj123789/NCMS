using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestMaterialAttachment : EntityBase
    {
        private IList<CandidateBrief> mCandidateBriefs = new List<CandidateBrief>();
        public virtual StoredFile StoredFile { get; set; }
        public virtual TestMaterial TestMaterial { get; set; }
        public virtual string Title { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual bool ExaminerToolsDownload { get; set; }
        public virtual bool MergeDocument { get; set; }

        public virtual IEnumerable<CandidateBrief> CandidateBriefs => mCandidateBriefs;

    }
}
