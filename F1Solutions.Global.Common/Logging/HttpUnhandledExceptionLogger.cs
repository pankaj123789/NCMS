using System.Web.Http.ExceptionHandling;

namespace F1Solutions.Global.Common.Logging
{
    public class HttpUnhandledExceptionLogger : ExceptionLogger
    {
        public string DefaultSource { get; set; }

        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception != null)
            {
                if (DefaultSource != null)
                {
                    context.Exception.Source = DefaultSource;
                }
                LoggingHelper.LogException(context.Exception);
            }
        }
    }
}