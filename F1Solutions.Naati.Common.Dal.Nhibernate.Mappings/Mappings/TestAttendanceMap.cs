using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;
using FluentNHibernate.Mapping;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class TestAttendanceMap : IAutoMappingOverride<TestAttendance>
    {
        public void Override(AutoMapping<TestAttendance> mapping)
        {
            mapping.Id(x => x.Id);
            mapping.References(x => x.TestInvitation)
                .Unique();

            mapping.HasMany(m => m.TestResults)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();

            mapping.HasMany(m => m.TestAttendanceDocuments)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();
       
            mapping.HasMany(m => m.TestNotes)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
