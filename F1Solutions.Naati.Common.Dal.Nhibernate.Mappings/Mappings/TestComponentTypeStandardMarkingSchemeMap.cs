using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace DataAccessLayer.Mappings
{
    public class TestComponentTypeStandardMarkingSchemeMap : IAutoMappingOverride<TestComponentTypeStandardMarkingScheme>
    {
            public void Override(AutoMapping<TestComponentTypeStandardMarkingScheme> mapping)
            {
                mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
            }
    }
}
