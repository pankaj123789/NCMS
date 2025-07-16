using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using NAATI.Domain;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class ApplicationMap : IAutoMappingOverride<Application>
    {
        public void Override(AutoMapping<Application> mapping)
        {
            mapping.HasMany(m => m.CourseAttendances)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();

            mapping.HasMany(m => m.AccreditationResults)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(m => m.TestInvitations)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            mapping.HasMany(m => m.ApplicationRevalidations)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
            
            mapping.Map(m => m.SkillAssessmentNotes).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}