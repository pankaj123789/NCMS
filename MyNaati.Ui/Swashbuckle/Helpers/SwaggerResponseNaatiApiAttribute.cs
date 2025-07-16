using System;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Swashbuckle.Swagger.Annotations;

namespace MyNaati.Ui.Swashbuckle.Helpers
{
    public class SwaggerResponseNaatiApiAttribute : SwaggerResponseAttribute
    {
        public SwaggerResponseNaatiApiAttribute(ApiPublicErrorCode errorCode, string description, Type type = null) :base((int)errorCode, description, type)
        {
            
        }
    }
}