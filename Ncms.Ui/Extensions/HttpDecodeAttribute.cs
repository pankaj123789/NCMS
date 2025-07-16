

using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Ncms.Ui.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class HttpEncodeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if ((actionContext.Request.Method == HttpMethod.Post)
                || (actionContext.Request.Method== HttpMethod.Put))
            {
                foreach (var item in actionContext.ActionArguments.Values)
                {
                    try
                    {
                        // Get the type of the object
                        Type type = item.GetType();

                        // For each property of this object, html decode it if it is of type string
                        foreach (PropertyInfo propertyInfo in type.GetProperties())
                        {
                            var prop = propertyInfo.GetValue(item);
                            if (prop != null && prop.GetType() == typeof(string))
                            {
                                propertyInfo.SetValue(item, HttpUtility.HtmlEncode((string)prop));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }
}