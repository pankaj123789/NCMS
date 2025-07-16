using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class InstitutionMap : IAutoMappingOverride<Institution>
    {
        public void Override(AutoMapping<Institution> mapping)
        {
            mapping.References(prop => prop.LatestInstitutionName).Column("InstitutionId").ReadOnly()
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasMany(x => x.InstitutionNames)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.InstitutionContactPersons)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.ContactPersons)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.IgnoreProperty(x => x.CurrentName);

            mapping.Map(x => x.InstitutionName).Formula("(SELECT TOP 1 tblInstitutionName.Name FROM tblInstitutionName WHERE tblInstitutionName.InstitutionId = InstitutionId ORDER BY tblInstitutionName.EffectiveDate DESC)");
            mapping.Map(x => x.TradingName).Formula("(SELECT TOP 1 tblInstitutionName.TradingName FROM tblInstitutionName WHERE tblInstitutionName.InstitutionId = InstitutionId ORDER BY tblInstitutionName.EffectiveDate DESC)");
            mapping.Map(x => x.InstitutionAbberviation).Formula("(SELECT TOP 1 tblInstitutionName.Abbreviation FROM tblInstitutionName WHERE tblInstitutionName.InstitutionId = InstitutionId ORDER BY tblInstitutionName.EffectiveDate DESC)");
        }
    }
}