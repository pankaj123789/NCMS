using System;
using System.Linq;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace MyNaati.Ui.Swashbuckle.Filters
{
    public class RouteDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            if (swaggerDoc == null)
            {
                throw new ArgumentNullException(nameof(swaggerDoc));
            }

            var pathsToIgnore = swaggerDoc.paths.Keys.Where(x => x.Contains("/api/{version}") || x.Contains("/api/ApiV2")).ToList();
            pathsToIgnore.ForEach(x => { swaggerDoc.paths[x].get = null; });
        }
    }

}