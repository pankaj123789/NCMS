using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Util;
using F1Solutions.Global.Common.Logging;
using Ncms.Ui;
using Swashbuckle.Application;
using Swashbuckle.Swagger;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Ncms.Ui
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            LoggingHelper.LogInfo("Configuring swagger...");
            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSwagger"]))
            {
                LoggingHelper.LogInfo("Swagger is disabled.");
                return;
            }

            IEnumerable<string> schemas = new List<string>();

            GlobalConfiguration.Configuration
              .EnableSwagger(c =>
              {
                  c.RootUrl(req => ResolveBasePath(req));
                  c.SingleApiVersion("v1", "Naati Public Api Specification");
                  c.PrettyPrint();

                  //auth
                  // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                  //c.IgnoreObsoleteActions();

                  c.GroupActionsBy(apiDesc => {
                      var attr = apiDesc
                          .GetControllerAndActionAttributes<DisplayNameAttribute>()
                          .FirstOrDefault();

                      // use controller name if the attribute isn't specified
                      return attr?.DisplayName ?? apiDesc.ToString();
                  });

                  //c.DocumentFilter<RemoveVerbsFilter>();

                  // You can also specify a custom sort order for groups (as defined by "GroupActionsBy") to dictate
                  // the order in which operations are listed. For example, if the default grouping is in place
                  // (controller name) and you specify a descending alphabetic sort order, then actions from a
                  // ProductsController will be listed before those from a CustomersController. This is typically
                  // used to customize the order of groupings in the swagger-ui.
                  //
                  //c.OrderActionGroupsBy(new DescendingAlphabeticComparer());

                  //c.IncludeXmlComments(GetXmlCommentsPath());

                  // Swashbuckle makes a best attempt at generating Swagger compliant JSON schemas for the various types
                  // exposed in your API. However, there may be occasions when more control of the output is needed.
                  // This is supported through the "MapType" and "SchemaFilter" options:
                  //
                  // Use the "MapType" option to override the Schema generation for a specific type.
                  // It should be noted that the resulting Schema will be placed "inline" for any applicable Operations.
                  // While Swagger 2.0 supports inline definitions for "all" Schema types, the swagger-ui tool does not.
                  // It expects "complex" Schemas to be defined separately and referenced. For this reason, you should only
                  // use the "MapType" option when the resulting Schema is a primitive or array type. If you need to alter a
                  // complex Schema, use a Schema filter.
                  //
                  //c.MapType<ProductType>(() => new Schema { type = "integer", format = "int32" });

                  // If you want to post-modify "complex" Schemas once they've been generated, across the board or for a
                  // specific type, you can wire up one or more Schema filters.
                  //
                  //c.SchemaFilter<ApplyCustomSchemaFilters>();

                  // In a Swagger 2.0 document, complex types are typically declared globally and referenced by unique
                  // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids. In most cases, this
                  // works well because it prevents the "implementation detail" of type namespaces from leaking into your
                  // Swagger docs and UI. However, if you have multiple types in your API with the same class name, you'll
                  // need to opt out of this behavior to avoid Schema Id conflicts.
                  //
                  //c.UseFullTypeNameInSchemaIds();

                  // Alternatively, you can provide your own custom strategy for inferring SchemaId's for
                  // describing "complex" types in your API.
                  //
                  //c.SchemaId(x => x.FullName);
                  //c.UseFullTypeNameInSchemaIds();
                  ////c.SchemaId(t => t.FullName != null && t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);
                  //c.Schemes(schemas);

                  // Set this flag to omit schema property descriptions for any type properties decorated with the
                  // Obsolete attribute
                  //c.IgnoreObsoleteProperties();

                  // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                  // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                  // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                  // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                  //
                  //c.DescribeAllEnumsAsStrings();

                  // Similar to Schema filters, Swashbuckle also supports Operation and Document filters:
                  //
                  // Post-modify Operation descriptions once they've been generated by wiring up one or more
                  // Operation filters.

                  // c.OperationFilter<SwaggerOperationFilter>();
                  // c.DocumentFilter<SwaggerDocumentFilter>();

                  //c.OperationFilter<AddDefaultResponse>();

                  // Enable Swagger examples
                  //c.OperationFilter<ExamplesOperationFilter>();

                  //// Enable swagger response descriptions
                  //c.OperationFilter<DescriptionOperationFilter>();
                  //c.OperationFilter<AddResponseHeadersFilter>();
                  //c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                  //c.OperationFilter<ParameterOperationFilter>();
                  //c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();

                  // If you've defined an OAuth2 flow as described above, you could use a custom filter
                  // to inspect some attribute on each action and infer which (if any) OAuth2 scopes are required
                  // to execute the operation

                  //c.OperationFilter<AssignOAuth2SecurityRequirements>();

                  // Post-modify the entire Swagger document by wiring up one or more Document filters.
                  // This gives full control to modify the final SwaggerDocument. You should have a good understanding of
                  // the Swagger 2.0 spec. - https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md
                  // before using this option.

                  //c.DocumentFilter<ApplyDocumentVendorExtensions>();
                  //c.DocumentFilter<RouteDocumentFilter>();


                  // In contrast to WebApi, Swagger 2.0 does not include the query string component when mapping a URL
                  // to an action. As a result, Swashbuckle will raise an exception if it encounters multiple actions
                  // with the same path (sans query string) and HTTP method. You can workaround this by providing a
                  // custom strategy to pick a winner or merge the descriptions for the purposes of the Swagger docs
                  //
                  //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                  // Wrap the default SwaggerGenerator with additional behavior (e.g. caching) or provide an
                  // alternative implementation for ISwaggerProvider with the CustomProvider option.
                  //
                  //c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));

                  //c.SchemaFilter<DefaultExcludeFilter>();

                  //c.ApiKey("apiKey")
                  //    .Description("Filling bearer token here")
                  //    .Name("X-Naati")
                  //    .In("header");
                  //c.ApiKey("apiKey")
                  //    .Description("API Key Authentication")
                  //    .Name("apiKey")
                  //    .In("header");


              })
              //https://stackoverflow.com/questions/45857289/unable-to-change-swagger-ui-path
              .EnableSwaggerUi("apiDocs/{*assetPath}", c =>
              {
                  //"apiDocs/{*assetPath}"
                  // Use the "DocumentTitle" option to change the Document title.
                  // Very helpful when you have multiple Swagger pages open, to tell them apart.
                  //
                  c.DocumentTitle("Naati Swagger UI");

                  // Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
                  // The file must be included in your project as an "Embedded Resource", and then the resource's
                  // "Logical Name" is passed to the method as shown below.
                  //
                  //c.InjectStylesheet(containingAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testStyles1.css");

                  // Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
                  // has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
                  // "Logical Name" is passed to the method as shown above.
                  //
                  //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testScript1.js");

                  // The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
                  // strings as the possible choices. You can use this option to change these to something else,
                  // for example 0 and 1.
                  //
                  //c.BooleanValues(new[] { "0", "1" });

                  // By default, swagger-ui will validate specs against swagger.io's online validator and display the result
                  // in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
                  // feature entirely.
                  //c.SetValidatorUrl("http://localhost/validator");
                  //c.DisableValidator();

                  // Use this option to control how the Operation listing is displayed.
                  // It can be set to "None" (default), "List" (shows operations for each resource),
                  // or "Full" (fully expanded: shows operations and their details).
                  //
                  c.DocExpansion(DocExpansion.List);

                  // Specify which HTTP operations will have the 'Try it out!' option. An empty paramter list disables
                  // it for all operations.
                  //
                  //c.SupportedSubmitMethods("GET", "HEAD");

                  // Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
                  // It's typically used to instruct Swashbuckle to return your version instead of the default
                  // when a request is made for "index.html". As with all custom content, the file must be included
                  // in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
                  // the method as shown below.
                  //
                  //c.CustomAsset("index", containingAssembly, "YourWebApiProject.SwaggerExtensions.index.html");

                  // If your API has multiple versions and you've applied the MultipleApiVersions setting
                  // as described above, you can also enable a select box in the swagger-ui, that displays
                  // a discovery URL for each version. This provides a convenient way for users to browse documentation
                  // for different API versions.
                  //
                  //c.EnableDiscoveryUrlSelector();

                  // If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
                  // the Swagger 2.0 specification, you can enable UI support as shown below.
                  //
                  //c.EnableOAuth2Support(
                  //    clientId: "test-client-id",
                  //    clientSecret: null,
                  //    realm: "test-realm",
                  //    appName: "Swagger UI"
                  //    //additionalQueryStringParams: new Dictionary<string, string>() { { "foo", "bar" } }
                  //);


                  // If your API supports ApiKey, you can override the default values.
                  // "apiKeyIn" can either be "query" or "header"
                  //
                  //c.EnableApiKeySupport("apiKey", "header");
                  //c.EnableApiKeySupport("X-Naati", "header");

              });
        }

        //public static string GetXmlCommentsPath()
        //{
        //    return System.AppDomain.CurrentDomain.BaseDirectory + @"\bin\MyNaati.Ui.xml";
        //}

        private static string ResolveBasePath(HttpRequestMessage message)
        {
            var virtualPathRoot = message.GetRequestContext().VirtualPathRoot;
            var scheme = message.RequestUri.Host.Contains("localhost") ? "http" : "https";
            var schemeAndHost = $"{scheme}://{message.RequestUri.Host}";
            return new Uri(new Uri(schemeAndHost, UriKind.Absolute), virtualPathRoot).AbsoluteUri;
        }
    }

}
