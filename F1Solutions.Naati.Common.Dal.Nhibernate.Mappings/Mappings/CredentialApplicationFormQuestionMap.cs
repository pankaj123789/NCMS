using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class CredentialApplicationFormQuestionMap: IAutoMappingOverride<CredentialApplicationFormQuestion>
    {
        public void Override(AutoMapping<CredentialApplicationFormQuestion> mapping)
        {
            mapping.HasMany(x => x.QuestionAnswerOptions)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.QuestionLogics)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
