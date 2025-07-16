using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace F1Solutions.Naati.Common.Dal
{
    public class CustomGroupConjuction : Conjunction
    {
        // Custom implmentation to support HAVING clause in group query
        // Nhibernate for any group or agregate projections. 
        // Since the agregations and group projections are nested in the conjunction, Nhibernate doesnt find the group or agregate projection and therefore having clause is not created
        public override IProjection[] GetProjections()
        {
            var projections = base.GetProjections() ?? new IProjection[0];
            return projections.Concat(new []{ Projections.Count(Projections.Constant(1, NHibernateUtil.Int32))}).ToArray();
        }

    }

    public class CustomGroupDisjunction : Disjunction
    {
        // Custom implmentation to support HAVING clause in group query
        // Nhibernate for any group or agregate projections. 
        // Since the agregations and group projections are nested in the conjunction, Nhibernate doesnt find the group or agregate projection and therefore having clause is not created
        public override IProjection[] GetProjections()
        {
            var projections = base.GetProjections() ?? new IProjection[0];
            return projections.Concat(new[] { Projections.Count(Projections.Constant(1, NHibernateUtil.Int32)) }).ToArray();
        }

    }
}
