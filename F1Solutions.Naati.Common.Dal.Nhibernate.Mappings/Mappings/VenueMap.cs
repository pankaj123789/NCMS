using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class VenueMap : IAutoMappingOverride<Venue>
    {
        public void Override(AutoMapping<Venue> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
            mapping.HasMany(x => x.TestSessions)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
