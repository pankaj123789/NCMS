using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Bl;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseExecutor
    {
        private const int Timeout = 1000 * 60 * 5; //5 minutes
        public static T Execute<T>(Func<IWiiseIntegrationService, WiiseToken, Task<T>> action)
        {
            var task = Task.Run(() =>
            {
                var defaultUrl = ConfigurationManager.AppSettings["DefaultAppLocalUrl"];
                HttpContext.Current = new HttpContext(new HttpRequest(string.Empty, defaultUrl, string.Empty), new HttpResponse(new StringWriter()));
                var svc = ServiceLocator.Resolve<IWiiseIntegrationService>();
                var tokenProvider = ServiceLocator.Resolve<IWiiseTokenProvider>();
                //cutout for moment until OAuth2 is provided by Wiise
                var token = tokenProvider.GetToken();

                return Execute(
                    () =>
                    {
                        var t = action(svc, token);
                        if (!t.Wait(Timeout))
                        {
                            throw new Exception("Inner Operation timed out");
                        }

                        return t.Result;
                    });
            });

            return Execute(() =>
            {
                if (!task.Wait(Timeout + 1000)) //one second more
                {
                    throw new Exception("Operation timed out");
                }

                if (task.Exception != null)
                {
                    throw task.Exception;
                }
                return task.Result;
            });
        }

        private static T Execute<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex);
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        public static void Execute(Func<IWiiseIntegrationService, WiiseToken, Task> action)
        {
            Execute(async (svc, token) =>
            {
                await action(svc, token);
                return string.Empty;
            });
        }
    }
}
