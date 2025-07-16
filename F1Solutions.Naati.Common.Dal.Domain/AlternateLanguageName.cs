namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class AlternateLanguageName : EntityBase
    {
        public virtual Language Language { get; set; }
        public virtual string AlternateName { get; set; }
    }
}