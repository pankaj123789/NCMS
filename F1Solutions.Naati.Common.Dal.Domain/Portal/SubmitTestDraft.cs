using System;

namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class SubmitTestDraft : EntityBase
    {
        public virtual int TestAttendanceID { get; set; }
        public virtual int NAATINumber { get; set; }
        public virtual string Comments { get; set; }
        public virtual DateTime Updated { get; set; }
        public virtual string Letters { get; set; }
        public virtual int? PrimaryReasonForFailure { get; set; }
        public virtual string Feedback { get; set; }
    }
}
