using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class TestSessionRolePlayerMap : IAutoMappingOverride<TestSessionRolePlayer>
    {
        public void Override(AutoMapping<TestSessionRolePlayer> mapping)
		{
			mapping.HasMany(x => x.Details)
				.Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
				.Cascade.All()
				.Inverse();
		}
    }
}
