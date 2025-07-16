using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Wiise.HttpOperations;
using System;
using System.Threading.Tasks;

namespace F1Solutions.Naati.Common.Wiise.EntityNativeOperations
{
    internal class BaseEntityOperation
    {
        private ExceptionFactory _exceptionFactory = null;

        protected IAsynchronousClient AsynchronousClient { get; set; }
        protected RequestOptions RequestOptions { get; set; }
        protected string[] @contentTypes = new string[] { "application/json" };
        protected string[] @accepts = new string[] {"application/json"};

        internal BaseEntityOperation(IAsynchronousClient asynchronousClient)
        {
            AsynchronousClient = asynchronousClient;
            RequestOptions = new RequestOptions();
            foreach (var accept in @accepts)
                RequestOptions.HeaderParameters.Add("Accept", accept);
            foreach (var cType in @contentTypes)
                RequestOptions.HeaderParameters.Add("Content-Type", cType);
            _exceptionFactory = HttpOperations.Configuration.DefaultExceptionFactory;
        }

        protected void AddRequestOption(string key,string value)
        {
            RequestOptions.HeaderParameters.Add(key, value);
        }

        protected void AddTenant(string tenantId)
        {
            if (tenantId != null)
            {
                RequestOptions.HeaderParameters.Remove("wiise-tenant-id");
                RequestOptions.HeaderParameters.Add("wiise-tenant-id", ClientUtils.ParameterToString(tenantId));
            }
        }

        protected void AddAccessToken(string accessToken)
        {
            if (accessToken != null)
            {
                RequestOptions.HeaderParameters.Remove("Authorization");
                RequestOptions.HeaderParameters.Add("Authorization", "Bearer " + accessToken);
            }
        }

        protected ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        protected IReadableConfiguration BaseConfiguration { get; set; }

        internal async Task<ApiResponse<T>> GetAsyncWithHttpInfo<T>(string accessToken, string wiiseTenantId, string path, string filter = null, string expand = null, int? pageSize = null, int? pageNumber = null)
        {
            if (wiiseTenantId == null)
            {
                throw new ApiException(400, "Missing required parameter 'tenantId' when calling AccountingApi " + path);
            }

            if (accessToken == null)
            {
                throw new ApiException(400, "Missing required parameter 'accessToken' when calling AccountingApi " + path);
            }

            if (wiiseTenantId != null)
            {
                AddTenant(wiiseTenantId);
            }

            if (!String.IsNullOrEmpty(accessToken))
            {
                AddAccessToken(accessToken);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                RequestOptions.QueryParameters.Add("$filter", filter);
            }

            if (pageNumber != null && pageSize != null)
            {
                RequestOptions.QueryParameters.Add("$top", pageSize.ToString());
                RequestOptions.QueryParameters.Add("$skip", (pageSize * pageNumber).ToString());
            }

            if (expand != null)
            {
                RequestOptions.QueryParameters.Add("$expand", expand);
            }

            var response = await this.AsynchronousClient.GetAsync<T>(path, RequestOptions, this.BaseConfiguration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("HTTP GET on" + path, response);
                if (exception != null)
                {
                    LoggingHelper.LogException(exception);
                    throw exception;
                }              
            }

            return response;
        }

        internal async Task<ApiResponse<T>> PostAsyncWithHttpInfo<T>(string accessToken, string tenantId, T entity, string path)
        {
            if (tenantId == null)
            {
                throw new ApiException(400, "Missing required parameter 'tenantId' when calling AccountingApi-> " + path);
            }

            if (accessToken == null)
            {
                throw new ApiException(400, "Missing required parameter 'accessToken' when calling AccountingApi-> " + path);
            }

            if (tenantId != null)
            {
                AddTenant(tenantId);
            }

            // authentication (OAuth2) required
            if (!String.IsNullOrEmpty(accessToken))
            {
                AddAccessToken(accessToken);
            }

            RequestOptions.Data = null; 
            RequestOptions.Data = entity;

            var response = await this.AsynchronousClient.PostAsync<T>(path, RequestOptions, this.BaseConfiguration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("Create on" + path, response);
                if (exception != null)
                {
                    LoggingHelper.LogException(exception);
                    throw exception;
                }
            }

            return response;
        }


        internal async Task<ApiResponse<T>> PatchAsyncWithHttpInfo<T>(string accessToken, string tenantId, T entity, string eTag, string path)
        {
            if (tenantId == null)
            {
                throw new ApiException(400, "Missing required parameter 'tenantId' when calling AccountingApi-> " + path);
            }

            if (accessToken == null)
            {
                throw new ApiException(400, "Missing required parameter 'accessToken' when calling AccountingApi-> " + path);
            }

            if (tenantId != null)
            {
                AddTenant(tenantId);
            }

            if (!String.IsNullOrEmpty(accessToken))
            {
                AddAccessToken(accessToken);
            }

            RequestOptions.Data = null;
            RequestOptions.Data = entity;
            RequestOptions.HeaderParameters.Remove("If-Match");
            RequestOptions.HeaderParameters.Add("If-Match", eTag);

            var response = await this.AsynchronousClient.PatchAsync<T>(path, RequestOptions, this.BaseConfiguration);

            if (this.ExceptionFactory != null)
            {
                Exception exception = this.ExceptionFactory("Create on" + path, response);
                if (exception != null)
                {
                    LoggingHelper.LogException(exception);
                    throw exception;
                }
            }

            return response;
        }
    }
}
