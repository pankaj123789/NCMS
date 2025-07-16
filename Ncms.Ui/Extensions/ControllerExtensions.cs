using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Dal;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Common;
using Ncms.Ui.Helpers;

namespace Ncms.Ui.Extensions
{
    public static class ControllerExtensions 
    {
        public static HttpResponseMessage FileStreamResponse(this ApiController apiController, Func<FileModel> func, bool isDownloadResponse = true)
        {
            try
            {
                return apiController.FileStreamResponse(func(), isDownloadResponse);
            }
            catch (UserFriendlySamException ex)
            {
                if (ex.InnerException != null)
                {
                    HandleException(ex.InnerException);
                }
                return apiController.FileStreamErrorResponse(ex);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return apiController.FileStreamErrorResponse(ex);
            }
        }

        public static HttpResponseMessage FileStreamResponse(this ApiController apiController, FileModel fileModel, bool isDownloadResponse = true)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(fileModel.FileData)
            };

            response.Content.Headers.ContentType = fileModel.FileType.GetHeaderValue();

            if (!string.IsNullOrEmpty(fileModel.FileName))
            {
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileModel.FileName
                };
            }

            if (isDownloadResponse)
            {
                response.Headers.AddCookies(new[]
                {
                    new CookieHeaderValue("fileDownload", "true")
                    {
                        Path = "/",
                        HttpOnly = false
                    }
                });
            }

            return response;
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

    }
}
