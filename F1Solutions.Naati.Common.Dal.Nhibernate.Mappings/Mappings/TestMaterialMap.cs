using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class TestMaterialMap : IAutoMappingOverride<TestMaterial>
    {
        public void Override(AutoMapping<TestMaterial> mapping)
        {
            mapping.HasMany(x => x.TestMaterialAttachments)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.TestSittingTestMaterials)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();
            mapping.References(prop => prop.TestMaterialLastUsed).Column("TestMaterialId").ReadOnly().NotFound.Ignore();

            mapping.HasMany(x => x.MaterialRequests)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .KeyColumn(nameof(MaterialRequest.OutputMaterial) + "Id")
                .Cascade.All()
           
                .Inverse();

        }
    }
}
