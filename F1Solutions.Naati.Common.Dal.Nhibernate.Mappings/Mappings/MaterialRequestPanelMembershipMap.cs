using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Prefix = FluentNHibernate.Mapping.Prefix;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class MaterialRequestPanelMembershipMap : IAutoMappingOverride<MaterialRequestPanelMembership>
    {

        public void Override(AutoMapping<MaterialRequestPanelMembership> mapping)
        {
            mapping.HasMany(p => p.MaterialRequestPanelMembershipTasks)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.DeleteOrphan()
                .Inverse();
        }


    }
}
