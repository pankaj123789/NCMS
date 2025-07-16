using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using NAATI.Domain;
using FluentNHibernate.Mapping;

namespace F1Solutions.NAATI.WebService.NHibernate.Mappings.Mappings
{
    public class TestInvitationMap : IAutoMappingOverride<TestInvitation>
    {
        public void Override(AutoMapping<TestInvitation> mapping)
        {
            mapping.Id(x => x.Id);
            mapping.HasMany(p => p.TestAttendances)
                .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
                .Cascade.AllDeleteOrphan()
                .Inverse();

            //mapping.IgnoreProperty(x => x.CalculatedStatus);
        }
    }
}