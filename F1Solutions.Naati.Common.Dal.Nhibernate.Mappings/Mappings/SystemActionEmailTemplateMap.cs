using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class SystemActionEmailTemplateMap : IAutoMappingOverride<SystemActionEmailTemplate>
    {
        public void Override(AutoMapping<SystemActionEmailTemplate> mapping)
        {
            mapping.HasMany(x => x.CredentialWorkflowActionEmailTemplates)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.ActionEmailTemplateDetails)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
