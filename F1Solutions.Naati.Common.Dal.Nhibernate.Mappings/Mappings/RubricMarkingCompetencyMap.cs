using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class RubricMarkingCompetencyMap : IAutoMappingOverride<RubricMarkingCompetency>
    {
        public void Override(AutoMapping<RubricMarkingCompetency> mapping)
        {
            mapping.HasMany(x => x.RubricMarkingAssessmentCriteria)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");

        }
    }
    
}
