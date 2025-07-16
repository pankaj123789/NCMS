using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace TestSpecImporter.DataAccessLayer.Mappings
{
    public class CredentialTypeMap: IAutoMappingOverride<CredentialType>
    {
        public void Override(AutoMapping<CredentialType> mapping)
        {
            mapping.HasMany(x => x.CredentialApplicationTypeCredentialTypes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.TestSpecifications)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.CredentialTypeTemplates)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.DowngradePaths).KeyColumn(nameof(CredentialTypeDowngradePath.CredentialTypeFrom)+ "Id")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

 
            mapping.IgnoreProperty(x => x.DisplayName);
        }
    }
}
