using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class MailingListMap : IAutoMappingOverride<MailingList>
    {
        public void Override(AutoMapping<MailingList> mapping)
        {
            mapping.HasMany(ml => ml.MailingListAddresses)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}

