using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class EmailBatchMap : IAutoMappingOverride<EmailBatch>
    {
        public void Override(AutoMapping<EmailBatch> mapping)
        {
            mapping.HasMany(x => x.Recipients)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.Attachments)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.LetterTemplates)
                .Cascade.All()
                .Inverse();

            mapping.Map(x => x.Content).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}