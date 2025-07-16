using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace F1Solutions.Global.Common
{
    // todo: move these exceptions to the relevant area and develop them further
    [Serializable]
    public class F1Exception : Exception
    {
        public F1Exception(string message, Exception innerException = null)
            : base(message, innerException) {}
    }

    [Serializable]
    public class F1SanityException : F1Exception
    {
        public F1SanityException(string message, Exception innerException = null)
            : base(message, innerException) {}
    }

    /// <summary>
    /// Validation helper. Provides common sanity checks and throws exception when they fail. 
    /// <remarks>
    /// Public methods should only call private methods. If a public method calls another public
    /// methods, the error message will contain the wrong calling class/method names.
    /// </remarks>
    /// </summary>
    public static class SanityHelper
    {
        #region private

        /// <summary>
        /// Used to keep track of the calling method. A stack is needed to allow for the possibility of nested
        /// calls (e.g. NotNull() calls Util.MemberNameFrom() which calls ArgNotNull(). Or Requires() may be
        /// given a delegate which also uses SanityHelper)
        /// </summary>
        [ThreadStatic]
        private static Stack<MethodBase> mCallers;

        // for per-thread initialization
        private static Stack<MethodBase> Callers
        {
            get { return mCallers ?? (mCallers = new Stack<MethodBase>(100)); }
        }

        /// <summary>
        /// Returns general validation failure message which contains the name of the original calling class
        /// and method outside of SanityHelper.
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static string GetTraceInfoMessage()
        {
            var method = Callers.Peek();

            return String.Format("Sanity failure in {0}.{1}():",
                method.DeclaringType != null ? method.DeclaringType.Name : "Global", method.Name);
        }

        /// <summary>
        /// The private method that performs the validation by evaluating an expression passed from one of the public 
        /// methods.
        /// 
        /// If validation fails, an F1SanityException exception is raised with a message containing the name of the 
        /// original calling class and member name, plus the message passed in from the public method.
        /// 
        /// If validation succeeds, the value being validated is returned (however the public method may or may not
        /// return the value to the caller.)
        /// </summary>
        private static T CoreValidate<T>(Func<T, bool> validate, T value, String message = "")
        {
            if (!validate(value))
            {
                var trace = GetTraceInfoMessage();
                var msg = String.Format("{0} {1}", trace, (message ?? String.Empty)).Trim();
                throw new F1SanityException(msg);
            }

            return value;
        }

        private static T PrivateNotNull<T>(this T value, string name, string message = null)
            where T : class
        {
            string msg;

            if (!String.IsNullOrEmpty(name))
                msg = String.Format("`{0}` is null. {1}", name, message);
            else
                msg = message;

            return CoreValidate(x => x != null, value, msg);
        }

        private static T PrivateRequires<T>(this T obj, Expression<Func<T, bool>> conditionExpression, string message = "")
        {
            var condition = conditionExpression.Compile();

            string msg = String.Format("Validation condition not met: {0}. {1}", conditionExpression, message);
            CoreValidate(condition, obj, msg);
            return obj;
        }

        #endregion private

        /// <summary>
        /// Throws an exception if the given string is null, empty, or contains only whitespace. Otherwise returns the string.
        /// </summary>
        public static string NotNullOrWhiteSpace(this string value, string name = "Parameter", string message = null)
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                Func<string, bool> test = x => !String.IsNullOrWhiteSpace(x);
                var msg = String.Format("{0} is null, empty, or entirely whitespace. {1}", name, message);
                return CoreValidate(test, value, msg);
            }
            finally
            {
                Callers.Pop();
            }
		}

		/// <summary>
		/// Throws an exception if the given object is null, or is numeric but less than 1.
		/// </summary>
		public static T NotNullOrZero<T>(this T value, string message)
		where T : class
		{
			Callers.Push(new StackFrame(1).GetMethod());
			try
			{
				value.PrivateNotNull(null, message);

				int i;

				if (int.TryParse(value.ToString(), out i))
					i.PrivateRequires(x => x > 0, "Numeric value must be greater than zero");

				return value;
			}
			finally
			{
				Callers.Pop();
			}
		}

		/// <summary>
		/// Throws an exception if the given object is null, or is numeric but less than 1.
		/// </summary>
		public static int NotZero(this int value, string message)
		{
			Callers.Push(new StackFrame(1).GetMethod());
			try
			{
				value.PrivateRequires(x => x > 0, "Numeric value must be greater than zero");
				return value;
			}
			finally
			{
				Callers.Pop();
			}
		}

		public static Guid NotEmpty(this Guid value, string message)
		{
			Callers.Push(new StackFrame(1).GetMethod());
			try
			{
				if (value == Guid.Empty)
					value.PrivateRequires(x => x != Guid.Empty, "Guid value cannot be zero");

				return value;
			}
			finally
			{
				Callers.Pop();
			}
		}

		/// <summary>
		/// Use this to throw an exception when an object is null that shouldn't be, and you want the exception message to 
		/// be more informative than a basic NullReferenceException. Similar to FailIfNull() but you have to specify the message
		/// and the name of the object being validated is not automatically included in the message.
		/// 
		/// If the value is not null, it is returned to allow chaining.
		/// </summary>
		public static T NotNull<T>(this T value, string message)
            where T : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                return value.PrivateNotNull(null, message);
            }
            finally
            {
                Callers.Pop();
            }
        }

        public static T Exists<T>(this T value, object id)
            where T : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                return value.PrivateNotNull(null, String.Format("{0} with ID '{1}' does not exist.", typeof(T).Name, id));
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// For lazy (as in slack) validation. Throws an exception if the given value is null. Use this only 
        /// during development, or for very-unlikely-to-fail checks. Otherwise use one of the methods where 
        /// you can provide a helpful exception message.
        /// </summary>
        public static void FailIfNull<T>(this T value)
            where T : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                value.PrivateNotNull("An object");
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// Use this to validate a method argument. Throws an exception if the given value is null. 
        /// Otherwise returns the value. You must provide the name of the argument you are validating. 
        /// Providing a custom error message is optional.
        /// </summary>
        public static T ArgNotNull<T>(this T value, string name, string message = null)
            where T : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                return value.PrivateNotNull(String.Format("Method argument `{0}`", name), message);
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// Use this to test if the value of an object member is null without having to use the literal member name.
        /// 
        /// Throws an exception if the given member expression returns null when the given object is passed to it. Otherwise 
        /// returns the value returned by the expression. The good thing about this is that the name of the member is obtained 
        /// from the expression and used in the exception message if necessary.
        /// </summary>
        public static TVal NotNull<TObj, TVal>(this TObj obj, Expression<Func<TObj, TVal>> memberExpression, string message = "")
            where TObj : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                string memberName;
                object value;

                try
                {
                    // get the name of the member that will yield the value to validate; this is used in the exception message
                    memberName = Util.MemberNameFrom(memberExpression);

                    // compile and execute the member expression to get the value to validate
                    Func<TObj, TVal> valueDelegate = memberExpression.Compile();
                    value = valueDelegate(obj);
                }
                catch (Exception e)
                {
                    throw new F1SanityException("Error compiling or evaluating expression for validation. See inner exception.", e);
                }

                // perform the validation
                return (TVal)PrivateNotNull(value, String.Format("An instance of {0}.{1}", obj.GetType().Name, memberName), message);
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// Throws an exception if the given list is empty. Otherwise returns the list.
        /// </summary>
        public static IEnumerable<T> NotEmpty<T>(this IEnumerable<T> value, string name = "List", string message = null)
            where T : class
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                var msg = String.Format("List of {0}, \"{1}\", is empty. {2}", typeof(T), name, message);
                return CoreValidate(x => x.Any(), value, msg);
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// Throws an exception if the given expression returns False when the given object is passed to it. 
        /// Otherwise returns the given object.
        /// </summary>
        public static T Requires<T>(this T obj, Expression<Func<T, bool>> conditionExpression, string message = "")
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                return obj.PrivateRequires(conditionExpression, message);
            }
            finally
            {
                Callers.Pop();
            }
        }

        /// <summary>
        /// Checks for an existing, non-empty attribute on an xElement, and returns it. If the attribute
        /// does not exist or is empty, an exception is thrown.
        /// </summary>
        public static string AttributeValueNotEmpty(this XmlNode xmlNode, string name)
        {
            Callers.Push(new StackFrame(1).GetMethod());
            try
            {
                CoreValidate(x => x.Attributes != null, xmlNode,
                    String.Format("XML element '{0}' is missing the '{1}' attribute.", xmlNode.Name, name));
                
                var attribute = xmlNode.Attributes[name];

                CoreValidate(x => x != null, attribute,
                    String.Format("XML element '{0}' is missing the '{1}' attribute.", xmlNode.Name, name));

                var value = attribute.Value;

                CoreValidate(x => !String.IsNullOrWhiteSpace(x), value,
                    String.Format("XML element '{0}' attribute '{1}' does not have a value.", xmlNode.Name, name));

                return value;
            }
            finally
            {
                Callers.Pop();
            }
        }
    }
}
