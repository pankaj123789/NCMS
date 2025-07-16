using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class EmailTemplateMap : IAutoMappingOverride<EmailTemplate>
    {
        public void Override(AutoMapping<EmailTemplate> mapping)
        {
            mapping.Map(x => x.Content).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            mapping.HasMany(x => x.SystemActionEmailTemplates)
               .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
               .Cascade.All()
               .Inverse();
        }
    }
}