using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class ProfessionalDevelopmentActivityMap : IAutoMappingOverride<ProfessionalDevelopmentActivity>
    {
        public void Override(AutoMapping<ProfessionalDevelopmentActivity> mapping)
        {
            mapping.HasMany(m => m.ProfessionalDevelopmentActivityAttachments)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();
        }
    }
}