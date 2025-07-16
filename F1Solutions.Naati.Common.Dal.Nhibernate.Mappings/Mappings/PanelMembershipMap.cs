using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Prefix = FluentNHibernate.Mapping.Prefix;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class PanelMembershipMap : IAutoMappingOverride<PanelMembership>
    {

        public void Override(AutoMapping<PanelMembership> mapping)
        {
            mapping.HasMany(p => p.PanelMembershipCredentialTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.DeleteOrphan()
                .Inverse();

            mapping.HasMany(p => p.PanelMembershipMaterialCredentialTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.DeleteOrphan()
                .Inverse();

            mapping.HasMany(p => p.PanelMembershipCoordinatorCredentialTypes)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.DeleteOrphan()
               .Inverse();

            mapping.HasMany(p => p.JobExaminers)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.DeleteOrphan()
                .Inverse();
        }


    }
}
