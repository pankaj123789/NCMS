namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class SubmitTestDraftComponent : EntityBase
    {
        public virtual int SubmitTestDraftID { get; set; }
        public virtual int ComponentID { get; set; }
        public virtual double? Mark { get; set; }
    }
}
