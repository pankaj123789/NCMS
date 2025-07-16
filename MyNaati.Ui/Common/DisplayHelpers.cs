using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MyNaati.Ui.Common
{
    public static class DisplayHelpers
    {
        public static string GetFriendlyName<T>(T value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enum");

            string name = Enum.GetName(typeof(T), value);
            name = Regex.Replace(name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
            //replace " To " and " From "
            name = name.Replace(" To ", " to ");
            name = name.Replace(" From ", " from ");
            //not the ideal place for this, but it'll do.
            // Maybe we should allow registration of additional processing rules for [specific?] enums (ie we pass in the type and our string)
            name = name.Replace(" A N Z Only", " (Australia/NZ only)");
            return name;
        }

        public static string GetDisplayNameAttribute<TModel>(Expression<Func<TModel, object>> expr)
        {
            var operand = expr.Body;
            while (operand is UnaryExpression)
            {
                var unaryOperand = (UnaryExpression)operand;
                if (unaryOperand.NodeType == System.Linq.Expressions.ExpressionType.Convert)
                    operand = (unaryOperand.Operand);
                if (unaryOperand.NodeType == System.Linq.Expressions.ExpressionType.MemberAccess)
                    break;               
            }
            MemberExpression memberExpression = (MemberExpression)operand;
            if (!(memberExpression.Member is PropertyInfo))
                throw new InvalidOperationException();
            string propertyName = memberExpression.Member.Name;

            var foundProperties = typeof(TModel).GetProperty(propertyName).GetCustomAttributes(typeof(DisplayNameAttribute), true);
            if (foundProperties.Any())
                return ((DisplayNameAttribute) foundProperties[0]).DisplayName;

            return string.Empty;
        }
    }
}