using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestSessionRolePlayerMap: IAutoMappingOverride<TestSessionRolePlayer>

    {
        public void Override(AutoMapping<TestSessionRolePlayer> mapping)
        {
            mapping.HasMany(x => x.Details)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None()
                .Inverse();
        }
    }
}
