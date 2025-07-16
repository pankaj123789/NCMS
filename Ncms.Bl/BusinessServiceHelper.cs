using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using Ncms.Contracts.Models;

namespace Ncms.Bl
{
    public static class BusinessServiceHelper
    {
        public static GenericResponse<IEnumerable<TModel>> ConvertServiceListResponse<TDto, TModel>(this ServiceResponse<IEnumerable<TDto>> serviceResponse,
            Func<IEnumerable<TModel>, IEnumerable<TModel>> postTransform = null)
            where TDto : class
        {
            var response = new GenericResponse<IEnumerable<TModel>>();
            var autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();

            PopulateServiceResponse(serviceResponse, response);

            if (serviceResponse.Data != null)
            {
                response.Data = serviceResponse.Data.Select(autoMapperHelper.Mapper.Map<TModel>);
                if (postTransform != null)
                {
                    response.Data = postTransform(response.Data);
                }
            }

            return response;
        }

        public static GenericResponse<TModel> ConvertServiceResponse<TResponse, TModel>(this TResponse serviceResponse, Func<TResponse, TModel> transform = null)
            where TResponse : ServiceResponse
        {
            var response = new GenericResponse<TModel>();
            var autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();
            PopulateServiceResponse(serviceResponse, response);

            response.Data =
                transform == null
                    ? autoMapperHelper.Mapper.Map<TModel>(serviceResponse)
                    : transform(serviceResponse);

            return response;
        }

        public static GenericResponse<TModel> ConvertServiceResponse<TResponse, TModel>(this ServiceResponse<TResponse> serviceResponse, Func<TResponse, TModel> transform = null)
        {
            var response = new GenericResponse<TModel>();
            var autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();

            PopulateServiceResponse(serviceResponse, response);

            response.Data =
                transform == null
                    ? autoMapperHelper.Mapper.Map<TModel>(serviceResponse.Data)
                    : transform(serviceResponse.Data);

            return response;
        }

        public static BusinessServiceResponse ConvertServiceResponse<TResponse>(this TResponse serviceResponse)
            where TResponse : ServiceResponse
        {
            var response = new BusinessServiceResponse();
            PopulateServiceResponse(serviceResponse, response);
            return response;
        }

        private static void PopulateServiceResponse(ServiceResponse serviceResponse, BusinessServiceResponse response)
        {
            if (serviceResponse == null)
            {
                throw new Exception("NCMS Service call returned null.");
            }

            var hasErrorMessage = !String.IsNullOrEmpty(serviceResponse.ErrorMessage);
            var hasWarningMessage = !String.IsNullOrEmpty(serviceResponse.WarningMessage);
            response.Success = !serviceResponse.Error;
            if (hasErrorMessage)
            {
                response.Errors.Add(serviceResponse.ErrorMessage);
            }
            if (hasWarningMessage)
            {
                response.Warnings.Add(serviceResponse.WarningMessage);
            }
            if (!String.IsNullOrEmpty(serviceResponse.StackTrace))
            {
                // if there is a stack trace, the developer probably wants this exception to be logged
                var error = new HandledException(
                    hasErrorMessage
                        ? serviceResponse.ErrorMessage
                        : hasWarningMessage
                            ? serviceResponse.WarningMessage
                            : String.Empty,
                    serviceResponse.StackTrace, "NCMS Service");
                LoggingHelper.LogException(error, serviceResponse.ErrorMessage ?? serviceResponse.WarningMessage ?? nameof(PopulateServiceResponse));
            }
        }

        public static GenericResponse<T> CombineResponses<T>(this IList<BusinessServiceResponse> responses)
            where T: new()
        {
            var combined = new GenericResponse<T>(new T());
            foreach (var response in responses)
            {
                if (response != null)
                {
                    combined.Success &= response.Success;
                    combined.Messages.AddRange(response.Messages);
                    combined.Warnings.AddRange(response.Warnings);
                    combined.Errors.AddRange(response.Errors);
                }
            }

            var firstDataResponse = responses.OfType<GenericResponse<T>>().FirstOrDefault();
            if (firstDataResponse != null)
            {
                combined.Data = firstDataResponse.Data;
            }

            return combined;
        }
    }
}