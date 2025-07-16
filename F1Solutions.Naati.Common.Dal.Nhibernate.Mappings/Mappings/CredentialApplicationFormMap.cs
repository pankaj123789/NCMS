using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    class CredentialApplicationFormMap : IAutoMappingOverride<CredentialApplicationForm>
    {
        public void Override(AutoMapping<CredentialApplicationForm> mapping)
		{
			mapping.HasMany(x => x.Sections)
				.Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
				.Cascade.All()
				.Inverse();

		    mapping.HasMany(x => x.FormCredentialTypes)
		        .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
		        .Cascade.All()
		        .Inverse();
        }
    }
}
