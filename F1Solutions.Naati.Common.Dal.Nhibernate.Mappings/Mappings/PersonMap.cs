// F1Solutions.Naati.Common.Dal.Nhibernate.Mappings\Mappings\PersonMap.cs

using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class PersonMap : IAutoMappingOverride<Person>
    {
        private const int RecognitionMethodId = 6;

        public void Override(AutoMapping<Person> mapping)
        {
            mapping.References(prop => prop.LatestPersonName).Column("PersonId").ReadOnly()
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m);

            mapping.HasMany(x => x.PersonNames)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.CredentialApplications)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.All()
                .Inverse();

            mapping.HasMany(x => x.PersonImages)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.CertificationPeriods)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.PanelMemberships)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(x => x.ExaminerUnavailable)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            // --- OPTIONAL: set explicit table/id if you want (automapper may already do this)
            mapping.Table("tblPerson");
            mapping.Id(x => x.Id).GeneratedBy.Identity();

            // 1) map the soft-delete columns (must exist in Person/BaseEntity)
            mapping.Map(x => x.IsDeleted).Column("IsDeleted").Not.Nullable();
            mapping.Map(x => x.DeletedOn).Column("DeletedOn").Nullable();
            mapping.Map(x => x.DeletedBy).Column("DeletedBy").Length(256).Nullable();

            // 2) apply default WHERE so soft-deleted rows are excluded
            mapping.Where("IsDeleted = 0");

            // --- existing formula maps follow ---
            mapping.Map(x => x.HasPhoto).Formula("(isnull((select top 1 (case when tblPersonImage.Photo Is null then 0 else 1 end) from tblPersonImage where tblPersonImage.PersonId = PersonId order by tblPersonImage.PhotoDate desc),0))");
            mapping.Map(x => x.PhotoDate).Formula("(select top 1 tblPersonImage.PhotoDate from tblPersonImage where tblPersonImage.PersonId = PersonId order by tblPersonImage.PhotoDate desc)");
            mapping.Map(x => x.Surname).Formula("(select top 1 tblPersonName.Surname from tblPersonName where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.AlternativeSurname).Formula("(select top 1 tblPersonName.AlternativeSurname from tblPersonName where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.OtherNames).Formula("(select top 1 tblPersonName.OtherNames from tblPersonName where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.GivenName).Formula("(select top 1 tblPersonName.GivenName from tblPersonName where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.AlternativeGivenName).Formula("(select top 1 tblPersonName.AlternativeGivenName from tblPersonName where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.Title).Formula("(select top 1 tluTitle.Title from tblPersonName left join tluTitle on tblPersonName.TitleId = tluTitle.TitleId where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");
            mapping.Map(x => x.TitleId).Formula("(select top 1 tluTitle.TitleId from tblPersonName left join tluTitle on tblPersonName.TitleId = tluTitle.TitleId where tblPersonName.PersonId = PersonId order by tblPersonName.EffectiveDate desc)");


            // these are not loaded because of the way that they're used - filters on other queries.
            //  they also kill performance on person searches, so they're removed.
            mapping.Map(x => x.AccreditationCountNonPopulated).Formula(BuildCertificationCountSql(false)).LazyLoad().Access.None();
            mapping.Map(x => x.RecognitionCountNonPopulated).Formula(BuildCertificationCountSql(true)).LazyLoad().Access.None();

            mapping.Map(x => x.SuburbOrCountry).Formula(
                @"(SELECT TOP 1 CASE WHEN (tblCountry.Name = 'Australia')
  THEN 
	tblSuburb.Suburb + ' ' +
	tluState.State + ' ' +
	tblPostcode.Postcode
  ELSE tblCountry.Name
  END
  FROM tblAddress
  left join tblCountry on tblAddress.CountryId = tblCountry.CountryId
  left join tblPostcode on tblAddress.PostcodeId = tblPostcode.PostcodeId
  left join tblSuburb on tblSuburb.SuburbId = tblPostcode.SuburbId
  left join tluState on tluState.StateId = tblSuburb.StateId
  where tblAddress.PrimaryContact = '1' and tblAddress.EntityId = EntityId)").LazyLoad().Access.None();

            mapping.IgnoreProperty(x => x.Photo);
        }

        private string BuildCertificationCountSql(bool recognition)
        {
            return string.Format(
                    @"(select COUNT(*) 
                       from tblPerson 
                       inner join tblLanguageExperience on tblPerson.PersonId = tblLanguageExperience.PersonId 
                       inner join tblApplication on tblLanguageExperience.LanguageExperienceId = tblApplication.LanguageExperienceId 
                       inner join tblAccreditationResult on tblApplication.ApplicationId = tblAccreditationResult.ApplicationId 
                       where 
                       (tblAccreditationResult.ExpiryDate is null or tblAccreditationResult.ExpiryDate >= cast(GetDate() as Date)) and 
                       tblAccreditationResult.FailureReasonId is null and 
                       tblApplication.AccreditationMethodId {0} {1} and 
                       tblPerson.PersonId = PersonId)",
                       GetOperator(recognition),
                       RecognitionMethodId);
        }

        private string GetOperator(bool recognition)
        {
            if (recognition)
            {
                return "=";
            }

            return "<>";
        }
    }
}