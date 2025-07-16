using System;
using System.Collections.Generic;

namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class WorkPractice : EntityBase
    {
        public virtual DateTime Date { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Points{ get; set; }
        public virtual Credential Credential { get; set; }

        private IList<WorkPracticeAttachment> mWorkPracticeAttachments;
        public virtual IEnumerable<WorkPracticeAttachment> WorkPracticeAttachments
        {
            get { return mWorkPracticeAttachments; }
        }

        public WorkPractice()
        {
            mWorkPracticeAttachments = new List<WorkPracticeAttachment>();
        }
    }
}
