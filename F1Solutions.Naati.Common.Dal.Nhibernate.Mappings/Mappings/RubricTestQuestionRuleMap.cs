using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RubricTestQuestionRuleMap : IAutoMappingOverride<RubricTestQuestionRule>
    {
        public void Override(AutoMapping<RubricTestQuestionRule> mapping)
        {
            mapping.References(x => x.ModifiedUser).Column("ModifiedUser");
        }
    }
}
