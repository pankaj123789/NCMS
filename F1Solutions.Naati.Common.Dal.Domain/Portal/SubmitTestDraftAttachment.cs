namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class SubmitTestDraftAttachment : EntityBase
    {
        public virtual int SubmitTestDraftID { get; set; }
        public virtual string FileName { get; set; }
        public virtual byte[] FileBytes { get; set; }
    }
}
