using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Mappings
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.HasMany(x => x.UserRoles)
              .Access.ReadOnlyPropertyThroughPascalCaseField(Prefix.m)
              .Cascade.AllDeleteOrphan()
              .Inverse();
        }
    }
}




