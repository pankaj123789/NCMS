namespace F1Solutions.Naati.Common.Dal.Domain.Portal
{
    public class File : EntityBase
    {
        public virtual string FileName { get; set; }
        public virtual byte[] FileBytes { get; set; }
    }
}
