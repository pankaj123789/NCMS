using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class EmailBatchLetterTemplateMap : IAutoMappingOverride<EmailBatchLetterTemplate>
    {
        public void Override(AutoMapping<EmailBatchLetterTemplate> mapping)
        {
            mapping.Map(x => x.StandardLetterFile).CustomType("BinaryBlob").CustomSqlType("varbinary(max)");
        }
    }
}