using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using MyNaati.Ui.Swashbuckle.Helpers;
using MyNaati.Ui.Swashbuckle.Models;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace MyNaati.Ui.Swashbuckle.Filters
{
    public class ParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {

            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            var customExampleAttributes = apiDescription.GetControllerAndActionAttributes<RequestParameterExampleAttribute>();

            foreach (var attr in customExampleAttributes)
            {
                var parameter = operation.parameters.FirstOrDefault(x => x.name == attr.Name);
                if (parameter == null)
                {
                    parameter = new Parameter();

                    operation.parameters.Add(parameter);
                }
                var provider = (IParameterExampleProvider)Activator.CreateInstance(attr.ParameterRequestExampleProviderType);
                var example = provider.GetExample();
                parameter.name = attr.Name;
                var seriealizedExample = JsonConvert.SerializeObject(example);

                parameter.@in = attr.ParameterType;
                parameter.type = attr.DataType;
                parameter.description = attr.Description;
                parameter.required = attr.Required;
                parameter.@default = seriealizedExample;
                //parameter.maximum = 1000;
                //parameter.multipleOf = 5;
                //parameter.multipleOf = 10;
                parameter.format = "text";
            }
        }
    }

    public class AddAuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            operation.parameters.Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                description = "HMac token",
                required = true,
                type = "string"
            });
        }
    }

    public class ParameterOperationV2Filter : IOperationFilter
    {

        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            ValidateInput(operation, schemaRegistry, apiDescription);
            if (IsMethodWithHttpGetAttribute(apiDescription))
            {
                operation.parameters.Add(new Parameter
                {
                    name = "extra",
                    @in = "query",
                    description = "This is an extra querystring parameter",
                    required = false,
                    type = "string"
                });

                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "Used for certain authorization policies such as Bearer token authentication",
                    required = false,
                    type = "string"
                });
            }
        }

        private bool IsMethodWithHttpGetAttribute(ApiDescription apiDescription)
        {
            var attributes = apiDescription.ActionDescriptor.GetCustomAttributes<HttpGetAttribute>();
            return attributes != null && attributes.Any();
        }

        private void ValidateInput(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (schemaRegistry == null)
            {
                throw new ArgumentNullException(nameof(schemaRegistry));
            }

            if (apiDescription == null)
            {
                throw new ArgumentNullException(nameof(apiDescription));
            }

            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }
        }
    }
}