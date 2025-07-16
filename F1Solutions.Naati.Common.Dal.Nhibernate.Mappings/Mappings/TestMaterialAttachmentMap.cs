using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestMaterialAttachmentMap : IAutoMappingOverride<TestMaterialAttachment>
    {
        public void Override(AutoMapping<TestMaterialAttachment> mapping)
        {
            mapping.HasMany(x => x.CandidateBriefs)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
