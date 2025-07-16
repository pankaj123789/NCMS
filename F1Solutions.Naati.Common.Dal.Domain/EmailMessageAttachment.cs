namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class EmailMessageAttachment : EntityBase
    {
        public virtual EmailMessage EmailMessage { get; set; }

        public virtual StoredFile StoredFile { get; set; }

        public virtual string Description { get; set; }
        public virtual string FileName { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
