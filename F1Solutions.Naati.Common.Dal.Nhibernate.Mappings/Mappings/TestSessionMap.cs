using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestSessionMap : IAutoMappingOverride<TestSession>
    {
        public void Override(AutoMapping<TestSession> mapping)
        {
            mapping.HasMany(x => x.TestSittings)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.TestSessionSkills)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None()
                .Inverse();


            mapping.HasMany(x => x.TestSessionRolePlayers)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.None()
                .Inverse();
        }
    }
}
