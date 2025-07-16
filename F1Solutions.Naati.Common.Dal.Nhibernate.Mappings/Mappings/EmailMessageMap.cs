using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class EmailMessageMap : IAutoMappingOverride<EmailMessage>
    {
        public void Override(AutoMapping<EmailMessage> mapping)
        {
            mapping.Id(x => x.Id);
            mapping.Map(x => x.Body).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            mapping.HasMany(x => x.Attachments)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}