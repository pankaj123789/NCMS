using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class RubricMarkingAssessmentCriterionMap : IAutoMappingOverride<RubricMarkingAssessmentCriterion>
    {
        public void Override(AutoMapping<RubricMarkingAssessmentCriterion> mapping)
        {
            mapping.HasMany(x => x.RubricMarkingBands)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");

        }
    }
}
