using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RubricTestComponentResultMap : IAutoMappingOverride<RubricTestComponentResult>
    {
        public void Override(AutoMapping<RubricTestComponentResult> mapping)
        {
            mapping.HasMany(x => x.RubricAssessementCriterionResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
