using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Ninject.Extensions.Interception;

namespace Ncms.Ui.Ioc
{
    public class WiiseAccountingApiInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation?.Request?.Method?.Name ?? String.Empty;
            object arg = null;
            if (methodName.ToLowerInvariant().StartsWith("create") || methodName.ToLowerInvariant().StartsWith("update"))
            {
                arg = invocation.Request.Arguments[2];
            }
            else if (!methodName.ToLowerInvariant().StartsWith("get"))
            {
                methodName = null;
            }
            if (!String.IsNullOrWhiteSpace(methodName))
            {
                Log(methodName, arg);
            }
            invocation.Proceed();
        }

        private void Log(string memberName, object arg)
        {
            var argumentType = arg != null ? $"{Util.GetBriefTypeName(arg)}" : "none";
            LoggingHelper.LogDebug("Wiise API method '{MemberName}' was called with argument type {ArgumentType}", memberName, argumentType);
            if (arg != null)
            {
                LoggingHelper.LogVerbose("Wiise.{MemberName} passed {@ArgumentDetail}", memberName, arg);
            }
        }
    }
}