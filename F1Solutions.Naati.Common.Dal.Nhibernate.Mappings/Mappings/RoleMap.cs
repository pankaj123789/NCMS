using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RoleMap : IAutoMappingOverride<SecurityRole>
    {
        public void Override(AutoMapping<SecurityRole> mapping)
        {
            mapping.HasMany(x => x.SecurityRules)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    
    }
}
