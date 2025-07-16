using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace DataAccessLayer.Mappings
{
    public class TestSpecificationStandardMarkingSchemeMap : IAutoMappingOverride<TestSpecificationStandardMarkingScheme>
    {
        public void Override(AutoMapping<TestSpecificationStandardMarkingScheme> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
