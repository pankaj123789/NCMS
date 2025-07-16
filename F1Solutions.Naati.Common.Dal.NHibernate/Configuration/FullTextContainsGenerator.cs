namespace F1Solutions.Naati.Common.Dal.NHibernate.Configuration
{
    //public class FullTextContainsGenerator : BaseHqlGeneratorForMethod
    //{
    //    public FullTextContainsGenerator()
    //    {
    //        SupportedMethods = new[] { ReflectionHelper.GetMethodDefinition(() => DialectExtensions.FullTextContains(null, null)) };
    //    }

    //    public override HqlTreeNode BuildHql(MethodInfo method,
    //      System.Linq.Expressions.Expression targetObject,
    //      ReadOnlyCollection<System.Linq.Expressions.Expression> arguments,
    //      HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
    //    {
    //        HqlExpression[] args = new HqlExpression[2] {
    //            visitor.Visit(arguments[0]).AsExpression(),
    //            visitor.Visit(arguments[1]).AsExpression()
    //        };
    //        return treeBuilder.BooleanMethodCall("contains", args);
    //    }
    //}
}
