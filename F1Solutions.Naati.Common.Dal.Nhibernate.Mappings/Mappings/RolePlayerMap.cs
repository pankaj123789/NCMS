using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RolePlayerMap : IAutoMappingOverride<RolePlayer>
    {
        public void Override(AutoMapping<RolePlayer> mapping)
		{
			mapping.HasMany(x => x.Locations)
				.Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
				.Cascade.All()
				.Inverse();

			mapping.HasMany(x => x.Sessions)
				.Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
				.Cascade.All()
				.Inverse();

		    mapping.HasMany(x => x.LastAttendedTestSessions)
		        .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
		        .Cascade.All()
		        .Inverse();
        }
    }
}
