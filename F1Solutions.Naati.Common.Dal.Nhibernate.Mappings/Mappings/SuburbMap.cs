using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class SuburbMap : IAutoMappingOverride<Suburb>
    {
        public void Override(AutoMapping<Suburb> mapping)
        {
            mapping.Map(x => x.Name).Column("Suburb");

            mapping.HasMany(x => x.Postcodes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
