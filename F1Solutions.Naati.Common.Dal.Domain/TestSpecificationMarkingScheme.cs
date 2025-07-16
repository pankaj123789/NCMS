namespace F1Solutions.Naati.Common.Dal.Domain
{
    public class TestSpecificationMarkingScheme : EntityBase
    {
        public virtual CredentialType CredentialType { get; set; }
        public virtual TestSpecification TestSpecification { get; set; }
    }
}
