using F1Solutions.Naati.Common.Dal.Domain;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Conventions
{
    public class LookupTableNameConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table("tlu" + instance.EntityType.Name);
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => typeof (ILookupType).IsAssignableFrom(x.EntityType));
        }
    }
}