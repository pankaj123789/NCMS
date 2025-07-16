using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Ninject.Extensions.Interception;

namespace Ncms.Ui.Ioc
{
    public class QueryServiceInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            NHibernateSession.Current.Flush();
            NHibernateSession.Current.Clear();
            invocation.Proceed();
        }
    }
}