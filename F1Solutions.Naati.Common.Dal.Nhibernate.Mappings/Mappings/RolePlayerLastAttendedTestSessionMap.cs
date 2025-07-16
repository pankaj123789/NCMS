using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class RolePlayerLastAttendedTestSessionMap : IAutoMappingOverride<RolePlayerLastAttendedTestSession>
    {
        public void Override(AutoMapping<RolePlayerLastAttendedTestSession> mapping)
        {
            mapping.Table("vwRolePlayerLastTestSession");
        }
    }
}
