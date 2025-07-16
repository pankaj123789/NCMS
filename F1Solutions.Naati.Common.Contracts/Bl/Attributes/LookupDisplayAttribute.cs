using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using F1Solutions.Naati.Common.Contracts.Dal.BackOffice.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace F1Solutions.Naati.Common.Contracts.Bl.Attributes
{
    public class LookupDisplayAttribute : Attribute
    {
        /// <summary>
        /// Creates a new property with the display name when object is serialized to json
        /// </summary>
        /// <param name="lookupType">The lookup t</param>
        /// <param name="displayProperty">The new property name. If no property name is specified then new property will be formed be [Property]+ 'DisplayName' </param>
        public LookupDisplayAttribute(LookupType lookupType, string displayProperty = null)
        {
            LookupType = lookupType;
            DisplayProperty = displayProperty;
         
        }

        public LookupType LookupType { get; }
        public string DisplayProperty { get; }
    }

    //public interface ILookupTypeConverterHelper 
    //{
    //    string GetDisplayName(LookupType lookupType, int value);
    //}

    //public class LookupTypeConverterHelper : ILookupTypeConverterHelper
    //{
    //    private readonly IDictionary<LookupType, IDictionary<int, string>> _lookups;
    //    private readonly IUtilityQueryService _utilityQueryService;

    //    public LookupTypeConverterHelper(IUtilityQueryService utilityQueryService)
    //    {
    //        _utilityQueryService = utilityQueryService;
    //        _lookups = new ConcurrentDictionary<LookupType, IDictionary<int, string>>();
    //    }

    //    public string GetDisplayName(LookupType lookupType, int value)
    //    {
    //        if (value == 0)
    //        {
    //            return string.Empty;
    //        }

    //        IDictionary<int, string> values;
    //        if (!_lookups.TryGetValue(lookupType, out values))
    //        {
    //            values = _utilityQueryService.GetLookupType(lookupType)
    //                .Results.ToDictionary(x => x.Id, y => y.DisplayName);
    //            _lookups[lookupType] = values;
    //            return values[value];
    //        }

    //        var displayName = string.Empty;
    //        if (!values.TryGetValue(value, out displayName))
    //        {
    //            values = _utilityQueryService.GetLookupType(lookupType)
    //                .Results.ToDictionary(x => x.Id, y => y.DisplayName);
    //            _lookups[lookupType] = values;

    //            displayName = values[value];
    //        }
    //        return displayName;
    //    }

    //    public void ClearCache()
    //    {
    //       _lookups.Clear();
    //    }
    //}

    public class LookupTypeConverter : JsonConverter
    {
        private readonly ILookupTypeConverterHelper _converterHelper;
        private readonly string _displayProperty;
        private readonly LookupType _lookupType;


        public LookupTypeConverter(LookupType lookupType, string displayProperty,ILookupTypeConverterHelper converterHelper)
        {
            _converterHelper = converterHelper;
            _lookupType = lookupType;
            _displayProperty = displayProperty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);

            writer.WritePropertyName(_displayProperty);
            var lookupId = Convert.ToInt32(value);
            var displayName = _converterHelper.GetDisplayName(_lookupType, lookupId);
            serializer.Serialize(writer, displayName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,JsonSerializer serializer)
        {
            return serializer.Deserialize(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }

    public class CustomContractResolver : DefaultContractResolver
    {
        private readonly Func<ILookupTypeConverterHelper> _converterResolver;

        public CustomContractResolver(Func<ILookupTypeConverterHelper> depencencyResolver)
        {
            _converterResolver = depencencyResolver;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var displayAttribute = member.GetCustomAttribute<LookupDisplayAttribute>(true);
            if (displayAttribute != null)
            {
                var propertyName = displayAttribute.DisplayProperty;

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    propertyName = property.PropertyName;
                    var endingId = "Id";
                    var newEnding = "DisplayName";
                    if (propertyName.EndsWith(endingId))
                        propertyName = propertyName.Substring(0, propertyName.Length - endingId.Length);

                    propertyName = propertyName + newEnding;
                }


                property.Converter = new LookupTypeConverter(displayAttribute.LookupType, propertyName, _converterResolver());
            }

            return property;
        }
    }
}