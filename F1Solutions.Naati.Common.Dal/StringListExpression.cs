using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;

namespace F1Solutions.Naati.Common.Dal
{
 
    [Serializable]
    public class StringListExpression : PropertyExpression
    {

        private string mValues { get; set; }

        private const string separator = ",";


        /// <summary>
        /// Initializes a new instance of the <see cref="StringListExpression"/> class.
        /// </summary>
        /// <param name="lhsPropertyName">Name of the LHS property.</param>
        /// <param name="values">The RHS projection.</param>
        public StringListExpression(string lhsPropertyName, IEnumerable<int> values) : base(lhsPropertyName, Projections.Constant(1, NHibernateUtil.Int32))
        {
            mValues = string.Join(separator, values.ToArray());
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringListExpression"/> class.
        /// </summary>
        /// <param name="lhsProjection">The LHS projection.</param>
        /// <param name="values">The RHS projection.</param>
        public StringListExpression(IProjection lhsProjection, IEnumerable<int> values) : base(lhsProjection, Projections.Constant(1, NHibernateUtil.Int32))
        {
            mValues = string.Join(separator, values.ToArray());
        }
      
        /// <summary>
        /// Get the Sql operator to use for the <see cref="LtPropertyExpression"/>.
        /// </summary>
        /// <value>The string "<c> &lt; </c>"</value>
        protected override string Op
        {
            get { return $" IN (SELECT VALUE FROM STRING_SPLIT(\'{mValues}\', \'{separator}\')) AND 1 = "; }
        }
    }
}
