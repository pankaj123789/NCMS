using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace F1Solutions.Naati.Common.Dal.Nhibernate.Mappings.Conventions
{
    public class TableNameConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table("tbl" + instance.EntityType.Name);
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.OppositeOf<LookupTableNameConvention>();
        }
    }
}
