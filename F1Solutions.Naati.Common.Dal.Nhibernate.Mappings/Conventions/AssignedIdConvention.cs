using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Conventions
{
    public class AssignedIdConvention : IIdConvention, IIdConventionAcceptance
    {
        public void Apply(IIdentityInstance instance)
        {
            instance.GeneratedBy.Assigned();
        }

        public void Accept(IAcceptanceCriteria<IIdentityInspector> criteria)
        {
            criteria.Expect(x =>
            {
                var accept = typeof(LegacyEntityBase).IsAssignableFrom(x.EntityType);
                return accept;
            });
        }
    }
}