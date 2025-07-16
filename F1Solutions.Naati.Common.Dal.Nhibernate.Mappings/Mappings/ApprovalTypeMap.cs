using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class ApprovalTypeMap : IAutoMappingOverride<ApprovalType>
    {
        public void Override(AutoMapping<ApprovalType> mapping)
        {
            mapping.Table("tluApprovalType");
        }
    }
}
