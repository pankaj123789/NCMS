using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class EmailBatchAttachmentMap : IAutoMappingOverride<EmailBatchAttachment>
    {
        public void Override(AutoMapping<EmailBatchAttachment> mapping)
        {
            mapping.Map(x => x.Attachment).CustomType("BinaryBlob").CustomSqlType("varbinary(max)");
        }
    }
}