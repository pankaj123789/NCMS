using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Conventions
{
    public class PrimaryKeyColumnConvention : IIdConvention
    {
        public void Apply(IIdentityInstance identity)
        {
            identity.Column(identity.EntityType.Name + "Id");
        }
    }
}