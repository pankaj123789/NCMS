using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RubricAssessementCriterionResultMap :  IAutoMappingOverride<RubricAssessementCriterionResult>
    {
        public void Override(AutoMapping<RubricAssessementCriterionResult> mapping)
        {
            mapping.Map(x => x.Comments).CustomType("StringClob").CustomSqlType("nvarchar(4000)");
        }
    }
}
