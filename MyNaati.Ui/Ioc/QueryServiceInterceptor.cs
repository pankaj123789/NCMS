using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Ninject.Extensions.Interception;

namespace MyNaati.Ui.Ioc
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