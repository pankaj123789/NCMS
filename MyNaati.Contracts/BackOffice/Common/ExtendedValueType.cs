using System;
using System.Collections.Generic;

namespace MyNaati.Contracts.BackOffice.Common
{
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
