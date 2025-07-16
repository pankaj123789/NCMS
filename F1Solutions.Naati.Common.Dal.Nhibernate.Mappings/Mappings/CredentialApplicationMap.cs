using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
   public class CredentialApplicationMap : IAutoMappingOverride<CredentialApplication>
    {
        public void Override(AutoMapping<CredentialApplication> mapping)
        {
            mapping.HasMany(x => x.CredentialRequests)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.CredentialApplicationNotes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.CredentialApplicationFieldsData)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.CredentialWorkflowFees)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.Map(x => x.Reference).Formula($"CONCAT('{CredentialApplication.ApplicationReferencePrefix}', CAST(CredentialApplicationId AS VARCHAR))")
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);
        }
    }
}
