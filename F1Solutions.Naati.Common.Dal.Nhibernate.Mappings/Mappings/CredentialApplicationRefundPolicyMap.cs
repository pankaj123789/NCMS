using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class CredentialApplicationRefundPolicyMap : IAutoMappingOverride<CredentialApplicationRefundPolicy>
    {
        public void Override(AutoMapping<CredentialApplicationRefundPolicy> mapping)
        {
            mapping.HasMany(x => x.RefundPolicyParameters)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
