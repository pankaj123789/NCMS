using System;
using MyNaati.Ui.Swashbuckle.Models;

namespace MyNaati.Ui.Swashbuckle.Helpers
{


    public class RequestParameterExampleAttribute : Attribute
    {
        public Type ParameterRequestExampleProviderType { get; }
        public string Name { get; }
        public string Description { get; }
        public string DataType { get; }
        public string ParameterType { get; }
        public bool Required { get; }

        public RequestParameterExampleAttribute(Type parameterRequestExampleProviderType, string name, string description, string dataType = "string", string parameterType ="query", bool required = true)
        {
            if (!typeof(IParameterExampleProvider).IsAssignableFrom(parameterRequestExampleProviderType))
            {
                throw  new Exception($"Invalid provider type {parameterRequestExampleProviderType}");
            }

            ParameterRequestExampleProviderType = parameterRequestExampleProviderType;
            Name = name;
            Description = description;
            DataType = dataType;
            ParameterType = parameterType;
            Required = required;
        }
    }

    public class HMacParameterExampleAttribute : RequestParameterExampleAttribute
    {
        public HMacParameterExampleAttribute() : base(typeof(HMacParameterExampleProvider), "NAATI", "HMAC Token Value", "string", "header")
        {
        }
    }
    
}
