using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;
using F1Solutions.Naati.Common.Contracts.Dal;
using Resources = Naati.Resources;
namespace F1Solutions.Naati.Common.Bl.Extensions
{
    public static class ControllerExtensions
    {

        /// <summary>
        /// For calls that don't already return a GenericResponse, this wraps their response in a GenericResponse.
        /// </summary>
        public static GenericResponse<T> CreateGenericResponse<T>(this ApiController apiController, Func<T> func)
        {

            try
            {
                return func();
            }
            catch (Exception e)
            {
                var response = new GenericResponse<T>();
                response.Success = false;
                HandleException(e);
                response.Errors.Add(FailureMessage(e));
                return response;
            }
        }

        /// <summary>
        /// This one is able to override the success creiteria (instead of just using GenericRespons.Success)
        /// </summary>
        public static HttpResponseMessage CreateResponse<T>(this ApiController apiController, Func<GenericResponse<T>> func, Func<GenericResponse<T>, bool> successCriteria)
        {
            var response = new GenericResponse<T>();
            try
            {
                response = func();
            }
            catch (Exception e)
            {
                response.Success = false;
                HandleException(e);
                response.Errors.Add(FailureMessage(e));
            }

            if (successCriteria(response))
            {
                return apiController.SuccessResponse(response);
            }

            return apiController.CreateResponse(response);
        }

        public static HttpResponseMessage CreateResponse<T>(this ApiController apiController, Func<GenericResponse<T>> func)
        {
            var response = new GenericResponse<T>();

            try
            {
                response = func();
            }
            catch (Exception e)
            {
                response.Success = false;
                HandleException(e);
                response.Errors.Add(FailureMessage(e));
            }

            return apiController.CreateResponse(response);
        }

        private static HttpResponseMessage CreateResponse(this ApiController apiController, Func<BusinessServiceResponse> func, bool userFriendlySamExceptionAsMessage)
        {
            var response = new BusinessServiceResponse();

            try
            {
                response = func();
            }
            catch (Exception e)
            {
                response.Success = userFriendlySamExceptionAsMessage ? e is UserFriendlySamException : false;
                HandleException(e);
                response.Errors.Add(FailureMessage(e));
            }

            return apiController.CreateResponse(response);
        }

        public static HttpResponseMessage CreateResponse(this ApiController apiController, Func<BusinessServiceResponse> func)
        {
            return CreateResponse(apiController, func, false);
        }

        public static HttpResponseMessage CreateIntegrationResponse(this ApiController apiController, Func<BusinessServiceResponse> func)
        {
            return CreateResponse(apiController, func, true);
        }

        public static HttpResponseMessage CreateSearchResponse<T>(this ApiController apiController, Func<IEnumerable<T>> func)
        {
            return apiController.CreateSearchResponse(AppSettings.SearchResultLimit, func);
        }

        public static HttpResponseMessage CreateSearchResponse<T>(this ApiController apiController, Func<GenericResponse<IEnumerable<T>>> func)
        {
            return apiController.CreateSearchResponse(AppSettings.SearchResultLimit, func());
        }

        public static HttpResponseMessage CreateSearchResponse<T>(this ApiController apiController, int searchResultLimit, Func<IEnumerable<T>> func)
        {
            var response = apiController.CreateGenericResponse(func);
            return apiController.CreateSearchResponse(searchResultLimit, response);
        }

        public static HttpResponseMessage CreateSearchResponse<T>(this ApiController apiController, int searchResultLimit, GenericResponse<IEnumerable<T>> response)
        {
            if (!response.Success)
            {
                return apiController.CreateResponse(response);
            }

            if (response.Data == null || !response.Data.Any())
            {
                response.Messages.Add(Resources.Server.NoSearchResultsMessage);
            }
            else if (response.Data.Count() > searchResultLimit)
            {
                response.Messages.Add(string.Format(Resources.Server.LimitedSearchResultsMessage, searchResultLimit, response.Data.Count()));
                response.Data = response.Data.Take(searchResultLimit).ToList();
            }

            return apiController.CreateResponse(response);
        }

        public static HttpResponseMessage CreateResponse<T>(this ApiController apiController, Func<T> func)
        {
            var response = apiController.CreateGenericResponse(func);

            return apiController.CreateResponse(response);
        }


        public static HttpResponseMessage CreateResponse(this ApiController apiController, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                return apiController.FailureResponse(e);
            }

            return apiController.SuccessResponse();
        }

        public static HttpResponseMessage CreateResponse<T>(this ApiController apiController, GenericResponse<T> response)
        {
            return apiController.CreateResponse(response, r => r.Data);
        }

        private static HttpResponseMessage CreateResponse(this ApiController apiController, BusinessServiceResponse response)
        {
            return response.Success
                ? apiController.SuccessResponse(new { }, response.Messages, response.Warnings, response.Errors)
                : apiController.FailureResponse(response.Errors);
        }

        public static HttpResponseMessage CreateResponse<T, TU>(this ApiController apiController, GenericResponse<T> response, Func<GenericResponse<T>, TU> transform)
        {
            return response.Success
                ? apiController.SuccessResponse(transform(response), response.Messages, response.Warnings, response.Errors)
                : apiController.FailureResponse(response.Errors);
        }

        public static HttpResponseMessage SuccessResponse(this ApiController apiController)
        {
            return apiController.Request.CreateResponse(HttpStatusCode.OK);
        }

        public static HttpResponseMessage SuccessResponse<T>(this ApiController apiController, T data)
        {
            return apiController.Request.CreateResponse(HttpStatusCode.OK, new { data });
        }

        public static HttpResponseMessage SuccessResponse<T>(this ApiController apiController, GenericResponse<T> response)
        {
            return apiController.SuccessResponse(response.Data, response.Messages, response.Warnings, response.Errors);
        }

        public static HttpResponseMessage SuccessResponse<T>(this ApiController apiController, T data, IEnumerable<string> messages, IEnumerable<string> warnings, IEnumerable<string> errors)
        {
            return apiController.Request.CreateResponse(HttpStatusCode.OK, new { data, messages, warnings, errors });
        }

        public static HttpResponseMessage FailureResponse(this ApiController apiController, string message)
        {
            LoggingHelper.LogWarning(message);
            return apiController.Request.CreateResponse(HttpStatusCode.BadRequest, message);
        }

        public static HttpResponseMessage FailureResponse(this ApiController apiController, IEnumerable<string> messages)
        {
            messages?.ForEach(x => LoggingHelper.LogWarning(x));
            return apiController.Request.CreateResponse(HttpStatusCode.BadRequest, messages);
        }

        public static HttpResponseMessage FailureResponse(this ApiController apiController, Exception exception)
        {
            try
            {
                HandleException(exception);
            }
            catch
            {
                // ignore exceptions raised while handling the exception, as we need to have control over the response. at this point there is nothing more we can do.
            }
            return apiController.Request.CreateResponse(HttpStatusCode.BadRequest, FailureMessage(exception));
        }

      
        public static HttpResponseMessage FileStreamErrorResponse(this ApiController apiController, Exception exception)
        {
            var errorMessage = FailureMessage(exception);
            var response = apiController.Request.CreateResponse(HttpStatusCode.InternalServerError, new { error = errorMessage });

            response.Headers.AddCookies(new[]
            {
                new CookieHeaderValue("error", errorMessage)
                {
                    Path = "/"
                }
            });

            return response;
        }

        public static async Task<HttpResponseMessage> ProcessMultipartFileData<T>(this ApiController apiController, Func<string, MultipartFileData, MultipartFormDataStreamProvider, T> action)
        {
            // Check if the request contains multipart/form-data.
            if (!apiController.Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartFormDataStreamProvider(ConfigurationManager.AppSettings["tempFilePath"]);
            var list = new List<T>();

            // Read the form data and return an async task.
            try
            {
                await apiController.Request.Content.ReadAsMultipartAsync(provider);

                foreach (var fileData in provider.FileData)
                {
                    try
                    {
                        var fileName = provider.FormData["file"];

                        if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                        {
                            fileName = fileName.Trim('"');
                        }

                        if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                        {
                            fileName = Path.GetFileName(fileName);
                        }

                        var fileExtension = fileName.Split('.').Last();

                        if (!AppSettings.SettingList<string>("IncludedFileExtensionsList").Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                        {
                            return apiController.FailureResponse($"Unsupported File Type for file {fileName} will not be uploaded. Please refresh the screen to check on successfully uploaded file(s).");
                        }

                        fileName = Path.GetFileNameWithoutExtension(fileName) + Path.GetExtension(fileName);

                        list.Add(action(fileName, fileData, provider));
                    }
                    catch (Exception ex)
                    {
                        return apiController.FailureResponse(ex);
                    }
                    finally
                    {
                        // this is redundant as the file is moved
                        File.Delete(fileData.LocalFileName);
                    }
                }

                return apiController.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                return apiController.FailureResponse(ex);
            }
        }

        private static string FailureMessage(Exception exception)
        {
            return exception is UserFriendlySamException
                ? exception.Message
                : Resources.Server.GenericError;
        }

        private static void HandleException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            if (exception is UserFriendlySamException)
            {
                // we only log the inner exception of UserFriendlySamException
                LoggingHelper.LogWarning(exception.InnerException, exception.Message);
            }
            else
            {
                LoggingHelper.LogException(exception);
            }
        }
        public static GenericResponse<T> ToGenericResponse<T>(this T data, BusinessServiceResponse serviceResponse = null)
        {
            return new GenericResponse<T>
            {
                Data = data,
                Success = (serviceResponse?.Success).GetValueOrDefault(),
                Messages = serviceResponse?.Messages ?? new List<string>(),
                Warnings = serviceResponse?.Warnings ?? new List<string>(),
                Errors = serviceResponse?.Errors ?? new List<string>(),
            };
        }
    }
}
