using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class DocumentTypeCategoryMap : IAutoMappingOverride<DocumentTypeCategory>
    {
        public void Override(AutoMapping<DocumentTypeCategory> mapping)
        {
            mapping.HasMany(x => x.DocumentTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
        }
    }
}
