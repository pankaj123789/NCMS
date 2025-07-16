using F1Solutions.Naati.Common.Wiise.HttpOperations;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Linq;
using System.Net;

namespace F1Solutions.Naati.Common.Wiise.EntityPublicOperations
{
    internal class BaseEntityOperation
    {
        protected IAsynchronousClient AsynchronousClient { get; set; }
        protected RequestOptions RequestOptions { get; set; }
        protected String[] @contentTypes = new String[] { "application/json" };
        protected String[] @accepts = new String[] {"application/json"};

        internal BaseEntityOperation(IAsynchronousClient asynchronousClient)
        {
            AsynchronousClient = asynchronousClient;
            RequestOptions = new RequestOptions();
            foreach (var accept in @accepts)
                RequestOptions.HeaderParameters.Add("Accept", accept);
            foreach (var cType in @contentTypes)
                RequestOptions.HeaderParameters.Add("Content-Type", cType);
        }

        protected void AddRequestOption(string key,string value)
        {
            RequestOptions.HeaderParameters.Add(key, value);
        }

        protected IReadableConfiguration Configuration { get; set; }

        protected HttpStatusCode GetStausCode(HttpStatusCode combinedStatusCode, BaseModel result)
        {
            if (result.HasValidationErrors)
            {
                if (!result.ValidationErrors.Any())
                {
                    combinedStatusCode = 0;
                    result.ValidationErrors.Add(new ValidationError { ErrorCode = 0, Message = "Error unspecified" });
                }
                else
                {
                    combinedStatusCode = (HttpStatusCode)result.ValidationErrors.First().ErrorCode;
                }
            }
            return combinedStatusCode;
        }

    }
}
