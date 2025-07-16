using System;
using System.Collections.Generic;

namespace Ncms.Contracts.Models.Common
{
    /// <summary>	
    /// Provides a base class for the implementation of ExtendedValueTypes.
    /// 
    /// To implement an ExtendedValueType, define a class that inherits from ExtendedValueType, passing the derived class as the type
    /// argument T. The derived class must implement a private parameterless constructor, a private constructor that takes a string
    /// argument and passes it to the base constructor, and the abstract LoadType() method. The LoadType() implementation should call 
    /// AddDefinition() once for each different value, passing in a new instance each time. Define a string constant and a static getter 
    /// for each different value avaliable on the derived type. The static getter calls the Parse method, passing in the appropriate 
    /// string constant.
    /// </summary>	
    /// <remarks>
    /// ExtendedValueType is designed to provide developers with an alternative to enumerations, with two distinct advantages.
    /// Firstly, ExtendedValueTypes have an underlying string value that is displayed when ToString() is called. This allows
    /// the types to have a display friendly value including spaces and other characters that are illegal when defining an enumeration.
    /// Secondly, it is easier to further extend ExtendedValueTypes because they are defined as classes as opposed to enumerations.
    /// </remarks>
    /// <example>
    /// public class PersonType : ExtendedValueType -PersonType-
    /// {
    ///     private const string PROGRAMMER = "Programmer";
    ///     private const string LESSER_LIFE_FORM = "Lesser life form";
    ///
    ///     private PersonType()
    ///     {
    ///
    ///     }
    ///
    ///     private PersonType(string data)
    ///         : base(data)
    ///     {
    ///
    ///     }
    ///
    ///     public static PersonType Programmer
    ///     {
    ///         get
    ///         {
    ///             return Parse(PROGRAMMER);
    ///         }
    ///     }
    ///
    ///     public static PersonType LesserLifeForm
    ///     {
    ///         get
    ///         {
    ///             return Parse(LESSER_LIFE_FORM);
    ///         }
    ///     }
    ///
    ///     protected override void LoadType()
    ///     {
    ///         AddDefinition(new PersonType(PROGRAMMER));
    ///         AddDefinition(new PersonType(LESSER_LIFE_FORM));
    ///     }
    /// }	
    /// </example>
    /// <typeparam name="T">The type of the class deriving from ExtendedValueType</typeparam>
    public abstract class ExtendedValueType<T>
        where T : ExtendedValueType<T>
    {
        protected readonly string _data;
        private static readonly IDictionary<string, T> _definitions;

        protected ExtendedValueType() { }

        protected ExtendedValueType(string data)
        {
            this._data = data;
        }

        static ExtendedValueType()
        {
            _definitions = new Dictionary<string, T>();

            try
            {
                ((T)Activator.CreateInstance(typeof(T), true)).LoadType();
            }
            catch (MissingMethodException ex)
            {
                throw new NotImplementedException(
                    $"The type '{typeof(T).Name}' must define a private parameterless constructor.", ex);
            }
        }

        public override string ToString()
        {
            return _data;
        }

        /// <summary>
        /// Returns an ICollection containing an instance of each different value this ExtendedValueType supports.
        /// </summary>
        public static ICollection<T> Values => _definitions.Values;

        /// <summary>
        /// Returns the ExtendedValueType instance that correspondes to the passed string. Similar to enum.Parse().
        /// </summary>		
        /// <param name="data">The string value to return an ExtendedValueType instance for.</param>
        /// <returns>An ExtendedValueType instance.</returns>
        public static T Parse(string data)
        {
            if (_definitions.ContainsKey(data.Trim()))
                return _definitions[data.Trim()];
            else
                throw new ArgumentException($"'{data}' is not a valid {typeof(T).Name} type.", "data");
        }

        /// <summary>
        /// LoadType is an initialization method. The implementation must call AddDefinition() once for each different value 
        /// supported by the derived type.
        /// </summary>
        protected abstract void LoadType();

        /// <summary>
        /// Intended only for use when implementing the LoadType() method. Must be called once (and once only) for each different 
        /// value supported by the derived type.
        /// </summary>
        /// <param name="instance"></param>
        protected static void AddDefinition(T instance)
        {
            _definitions.Add(instance._data, instance);
        }
    }
}