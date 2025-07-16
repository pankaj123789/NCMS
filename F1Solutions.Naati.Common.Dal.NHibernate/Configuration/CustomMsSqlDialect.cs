using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace F1Solutions.Naati.Common.Dal.NHibernate.Configuration
{
    public class CustomMsSqlDialect : MsSql2012Dialect
    {
        public CustomMsSqlDialect() : base()
        {
            RegisterFunction("contains", new StandardSQLFunction("contains", null));
            RegisterFunction("NEWID", new StandardSQLFunction("NEWID", NHibernateUtil.Guid));
            RegisterFunction("HASHBYTES", new StandardSQLFunction("HASHBYTES", NHibernateUtil.String));
            RegisterFunction("ADD_DAYS", new SQLFunctionTemplate(NHibernateUtil.DateTime, "DATEADD(DAY,?1,?2)" ));
            RegisterFunction("ADD_MINUTES", new SQLFunctionTemplate(NHibernateUtil.DateTime, "DATEADD(MINUTE,?1,?2)" ));
            RegisterFunction("STRING_AGG", new StandardSQLFunction("STRING_AGG", NHibernateUtil.String));
        }
    }
}