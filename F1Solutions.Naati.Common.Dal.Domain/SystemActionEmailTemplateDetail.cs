namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class SystemActionEmailTemplateDetail : EntityBase
    {
        public virtual SystemActionEmailTemplate SystemActionEmailTemplate { get; set; }
        public virtual EmailTemplateDetailType EmailTemplateDetailType { get; set; }
    }
}
