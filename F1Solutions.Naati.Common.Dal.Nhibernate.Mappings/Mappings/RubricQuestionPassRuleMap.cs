using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RubricQuestionPassRuleMap : IAutoMappingOverride<RubricQuestionPassRule>
    {
        public void Override(AutoMapping<RubricQuestionPassRule> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
