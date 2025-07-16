using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace F1Solutions.Global.Common
{
    /// <summary>
    /// This class is a collection of miscellaneous helper methods which do not depend on any assemblies. 
    /// You should not add any methods which require a reference to another assembly, unless it is a very *very*
    /// general assembly itself (nothing product-specific).
    /// </summary>
    public static class Util
    {
        private static readonly Regex SplitPascalCaseWordRegex = new Regex("([A-Z][a-z])");
        private static readonly Regex SplitPascalCaseAcronymRegex = new Regex("([A-Z]{2,})");
        private static readonly Regex MemberPrefixRegex = new Regex("^[m|_][A-Z0-9]+");
        private static readonly Regex CapitalLetterRegex = new Regex(@"([A-Z][a-z]+)");

        private static TimeZoneInfo _desiredTimeInfo;

        public static TimeZoneInfo DesiredTimeInfo => _desiredTimeInfo ?? (_desiredTimeInfo = TimeZoneInfo.FindSystemTimeZoneById("asdfasdf"));

        /// <summary>
        /// Gets the name of a class member from an expression. Good for when you need to use a member name but
        /// want to keep your code runtime-error-resistant and visible to rename-refactoring. Works for Properties, 
        /// Methods and Fields.
        /// </summary>
        /// <typeparam name="TSource">The type to which the member belongs (you don't need to specify this as the compiler will infer it)</typeparam>
        /// <param name="type">Any object of the type to which the member belongs</param>
        /// <param name="expression">An expression which calls the member that you want the name of</param>
        /// <returns>The name of the member</returns>
        [DebuggerStepThrough]
        public static string MemberNameFrom<TSource>(this TSource type, Expression<Func<TSource, object>> expression)
        {
            return MemberNameFrom<TSource, object>(expression);
        }

        /// <summary>
        /// Gets the name of a class member from an expression. Good for when you need to use a member name but
        /// want to keep your code runtime-error-resistant and visible to rename-refactoring. Works for Properties, 
        /// Methods and Fields.
        /// </summary>
        /// <typeparam name="TSource">The type to which the member belongs</typeparam>
        /// <param name="expression">An expression which calls the member that you want the name of</param>
        /// <returns>The name of the member</returns>
        [DebuggerStepThrough]
        public static string MemberNameFrom<TSource>(Expression<Func<TSource, object>> expression)
        {
            return MemberNameFrom<TSource, object>(expression);
        }

        [DebuggerStepThrough]
        public static string MemberNameFrom<TSource, U>(Expression<Func<TSource, U>> expression)
        {
            expression.ArgNotNull("expression");
            try
            {
                string memberName = null;
                switch (expression.Body.NodeType)
                {
                    // method calls
                    case ExpressionType.Call:
                        {
                            var methodCall = expression.Body as MethodCallExpression;

                            if (methodCall != null)
                                memberName = methodCall.Method.Name;

                            break;
                        }

                    // property calls
                    case ExpressionType.Convert:
                        {
                            UnaryExpression unary = expression.Body as UnaryExpression;
                            if (unary != null)
                            {
                                var convertCall = unary.Operand as MemberExpression;
                                if (convertCall != null)
                                    memberName = convertCall.Member.Name;
                            }
                            break;
                        }

                    // field calls
                    case ExpressionType.MemberAccess:
                        {
                            MemberExpression fieldCall = expression.Body as MemberExpression;

                            if (fieldCall != null)
                                memberName = fieldCall.Member.Name;

                            break;
                        }

                    default:
                        throw new Exception(String.Format("Expression type not supported: {0}", expression.Body.NodeType));
                }

                if (memberName == null)
                    throw new Exception("Couldn't figure out given expression.");

                return memberName;
            }
            catch (Exception e)
            {
                throw new Exception(
                    String.Format("Failed to get property name from given expression. Source type: {0}. Error: {1}", typeof(TSource).Name,
                                  e.Message), e);

            }
        }

        /// <summary>
        /// Gets a Type instance by namespace-qualified name from the given assembly. 
        /// Wraps exceptions with message.
        /// </summary>
        public static Type CreateTypeByName(string typeName, Assembly assembly)
        {
            typeName.NotNullOrWhiteSpace("typeName");

            try
            {

                return assembly.GetType(typeName);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(
                        "An error occurred while trying to create a Type instance for type '{0}'. See inner exception.",
                        typeName), e);
            }
        }

        /// <summary>
        /// Create an instance of a given type by name, from the given assembly.
        /// </summary>
        public static T CreateInstance<T>(string typeName, Assembly assembly, params object[] args)
            where T : class
        {
            Type type = CreateTypeByName(typeName, assembly);

            type.NotNull(String.Format("The type `{0}` could not be loaded.", typeName));

            return CreateInstance<T>(type, args) as T;
        }

        /// <summary>
        /// Create an instance of a given type. 
        /// Wraps exceptions with message.
        /// </summary>
        public static T CreateInstance<T>(Type type, params object[] args)
            where T : class
        {
            try
            {
                T instance = Activator.CreateInstance(type, args) as T;

                if (instance == null)
                    throw new InvalidOperationException(String.Format("Type '{0}' does not seem to be a {1}.",
                                                                      type.Name, typeof(T).Name));
                return instance;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(

                        "An error occurred while trying to create an instance of '{0}'. See inner exception.",
                        type.Name), e);
            }
        }

        /// <summary>
        /// Performs an action over an IEnumerable(of T), where the index of each item is used
        /// in the action. 
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            source.NotNull("Source list must not be null");

            IEnumerator<T> enumerator = source.GetEnumerator();

            int i = 0;
            while (enumerator.MoveNext())
            {
                action(i, enumerator.Current);
                i++;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                return;
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        public static Type GetItemType<T>(this IEnumerable<T> enumerable)
        {
            return typeof(T);
        }

        public static string RemoveFromStart(this string s, string stringToRemove, StringComparison options)
        {
            if (s.StartsWith(stringToRemove, StringComparison.InvariantCultureIgnoreCase))
                return s.Remove(0, stringToRemove.Length);
            return s;
        }

        [DebuggerStepThrough]
        public static bool IsValidEmailAddress(string emailAddress)
        {
            const string emailExpr = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";

            Regex regEx = new Regex(emailExpr);
            if (!regEx.IsMatch(emailAddress))
                return false;

            try
            {
                new MailAddress(emailAddress);
                return true;
            }
            catch { return false; }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            const string phoneExpr = @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$";
            Regex regEx = new Regex(phoneExpr);
            return regEx.IsMatch(phoneNumber);
        }

        public static object TryGetPropertyValue(object source, string propName)
        {
            source.ArgNotNull("source");
            propName.NotNullOrWhiteSpace("propName");

            var prop = source.GetType().GetProperty(propName);
            if (prop == null)
                return null;
            return prop.GetValue(source, null);
        }

        public static object GetPropertyValue(object source, string propName)
        {
            source.ArgNotNull("source");
            propName.NotNullOrWhiteSpace("propName");

            var prop = source.GetType().GetProperty(propName);
            prop.NotNull("Source object does not have a property named " + propName);
            return prop.GetValue(source, null);
        }

        public static void SetPropertyValue(object src, string propName, object value)
        {
            src.GetType().GetProperty(propName).SetValue(src, value, null);
        }

        /// <summary>
        /// Takes a CamelCase string and returns it as human read able
        /// eg. "ProjectApplicationStatus" will be returned as "Project Application Status"
        /// </summary>
        public static string GetFriendlyName(string input)
        {
            //replace all items in caps with space plus the rest of the string ($1 is a placeholder for the rest of the string)
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        /// <summary>
        /// Splits a PascalCase/camelCase string into a string of component words, without changing capitalisation.
        /// </summary>
        public static string SplitPascalCase(string input)
        {
            // put spaces before normal words
            string words = SplitPascalCaseWordRegex.Replace(input, " $1");
            // put spaces before acronyms 
            return SplitPascalCaseAcronymRegex.Replace(words, " $1").Trim();
        }

        /// <summary>
        /// Removes m or _ from the start of a member name in the form of mMember or _Member.
        /// </summary>
        public static string SanitiseMemberName(string memberName)
        {
            if (MemberPrefixRegex.Match(memberName).Success)
                return memberName.Substring(1);
            return memberName;
        }

        /// <summary>
        /// Ensures that a string looks like a token (&lt;&lt;tokenName&gt;&gt;)
        /// </summary>
        public static string ToToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return token;
            }

            if (!token.StartsWith("<<") || !token.EndsWith(">>"))
            {
                return "<<" + token + ">>";
            }

            return token;
        }

        /// <summary>
        /// Ensures that a string does not look like a token (&lt;&lt;tokenName&gt;&gt;)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string FromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return token;
            }

            return token.Trim(new char[] { '<', '>' });
        }

        public static string ReplaceToken(string source, string token, string replacement)
        {
            if (replacement == null)
            {
                // null replacement value will throw an exception in Regex.Replace
                replacement = string.Empty;
            }
            return Regex.Replace(source, "(<|&lt;){2}" + Regex.Escape(FromToken(token)) + "(>|&gt;){2}", replacement, RegexOptions.IgnoreCase);
        }

        public static bool TextSearch(string token, params string[] searchFields)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            token = token.ToLower();
            return searchFields.Any(x => x != null && x.ToLower().Contains(token));
        }

        public static string FormatDate(DateTime? date, string nullValue = "")
        {
            return date.HasValue
                ? date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                : nullValue;
        }

        public static DateTime? Date(this DateTime? date)
        {
            return date.HasValue ? (DateTime?)date.Value.Date : null;
        }

        public static string SanitizePath(string path)
        {
            string result = path;
            foreach (char pathChar in Path.GetInvalidPathChars())
                result = result.Replace(pathChar, '_');
            return result;
        }

        public static string SanitizeFilename(string filename)
        {
            string result = filename;
            foreach (char nameChar in Path.GetInvalidFileNameChars())
                result = result.Replace(nameChar, '_');
            return result;
        }

        public static IEnumerable<KeyValuePair<string, string>> GetUrlEncodings()
        {
            var encodings = new Dictionary<string, string>();
            encodings.Add("_sub0", "_sub00");
            encodings.Add("%", "_sub01");
            encodings.Add(@"/", "_sub02");
            return encodings;
        }

        public static string DecodeUrlString(string encoded)
        {
            var decoded = encoded;
            if (!String.IsNullOrWhiteSpace(encoded))
            {
                var encodings = GetUrlEncodings().Reverse();

                foreach (var encoding in encodings)
                {
                    decoded = decoded.Replace(encoding.Value, encoding.Key);
                }
            }
            return decoded;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string With(this string str, params object[] args)
        {
            try
            {
                return string.Format(str, args);
            }
            catch (FormatException)
            {
                return str;
            }
        }

        /// <summary>
        /// Constant time string comparison
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool SecureEquals(string secret, string attackerKnown)
        {
            int sLength = secret.Length;
            int aLength = attackerKnown.Length;
            int diff = sLength ^ aLength;
            string dummy = new string(' ', aLength);

            // CJM paranoia: secret length info may be exposed from the switch between secret and dummy
            for (var i = 0; i < aLength; i++)
            {
                diff |= i < sLength ? secret[i] ^ attackerKnown[i] : dummy[i] ^ attackerKnown[i];
            }

            return diff == 0;
        }

        /// <summary>
        /// Constant time byte array comparison
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool SecureEquals(byte[] secret, byte[] attackerKnown)
        {
            int sLength = secret.Length;
            int aLength = attackerKnown.Length;
            int diff = sLength ^ aLength;
            byte[] dummy = new byte[aLength];

            // CJM paranoia: secret length info may be exposed from the switch between secret and dummy
            for (var i = 0; i < aLength; i++)
            {
                diff |= i < sLength ? secret[i] ^ attackerKnown[i] : dummy[i] ^ attackerKnown[i];
            }

            return diff == 0;
        }

        public static object Default(Type t)
        {
            // slightly different to activator.CreateInstance, in ways that probably don't matter.
            return typeof(Util).GetMethod("DefaultGeneric", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(t).Invoke(null, null);
        }

        // ReSharper disable once UnusedMember.Local
        // used by Default method
        private static T DefaultGeneric<T>()
        {
            return default(T);
        }

        public static double RoundOffForMidWay(this double value)
        {
            return Math.Round(2 * value, MidpointRounding.AwayFromZero) / 2;
        }

        /// <summary>
        /// Replaces tokens similar to String.Format() but tokens can be non-numeric (like a serilog template).
        /// Does not support formatting of tokens. 
        /// </summary>
        public static string FormatTemplate(this string input, params object[] tokens)
        {
            var results = Regex.Matches(input, @"({[^{}]*})");
            var replaceCount = Math.Min(tokens.Length, results.Count);
            for (int i = 0; i < replaceCount; i++)
            {
                input = input.Replace(results[i].Value, tokens[i].ToString());
            }
            return input;
        }

        /// <summary>
        /// Returns the last N chars of a string.
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="tailLength">Number of chars to return from end of string</param>
        public static string Tail(this string source, int tailLength)
        {
            if (source == null || tailLength >= source.Length)
            {
                return source;
            }
            return source.Substring(source.Length - tailLength);
        }


        public static bool IsNullOrEmpty(this Guid? guid)
        {
            return guid?.Equals(Guid.Empty) ?? true;
        }

        public static T ParseEnum<T>(this string value) where T : struct
        {
            T result;
            return Enum.TryParse(value, out result)
                ? result
                : default(T);
        }

        /// <summary>
        /// Gets the type name of an object. Includes type names of generic type arguments. No namespace or other junk.
        /// </summary>
        public static string GetBriefTypeName(object thing)
        {
            if (thing == null)
            {
                return "null";
            }

            var type = thing.GetType();
            return type.IsGenericType
                ? $"{type.Name}[{String.Join(", ", type.GetGenericArguments().Select(x => x.Name))}]"
                : type.Name;
        }

        public static T? GetValueOrNull<S,T>(S source, Func<S, T> member) where T : struct
        {
            if (source == null)
            {
                return null;
            }

            return  member(source);
        }

        public static string ToDisplayName(this string value)
        {
            var displayName = CapitalLetterRegex.Matches(value).Cast<Match>().Select(m => m.Value);
            var result = string.Join(" ", displayName);
            return result;
        }

        public static string ToSentence(this string input)
        {
            return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new char[] { ' ', c } : new char[] { c }).ToArray());
        }

        public static string TakeWords(this string input, int numberOfWordsToTake)
        {
            if (String.IsNullOrWhiteSpace(input))
            {
                return String.Empty;
            }

            return String.Join(" ", input.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Take(numberOfWordsToTake));
        }

        public static string GetDescription(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            MemberInfo[] memberInfo = enumType.GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attribs.Any())
                {
                    return ((DescriptionAttribute) attribs.ElementAt(0)).Description;
                }
            }
            return enumValue.ToString();
        }

        public static DateTime CurrentAesTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, DesiredTimeInfo);
        }

        
    }
}
