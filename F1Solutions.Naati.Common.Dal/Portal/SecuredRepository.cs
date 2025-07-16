using System;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Contracts.Dal.Portal;

namespace F1Solutions.Naati.Common.Dal.Portal
{
    public class SecuredRepository<T> : Repository<T>
        where T : class
    {
        private readonly IDataSecurityProvider mDataSecurityProvider;

        public SecuredRepository(ICustomSessionManager sessionManager, IDataSecurityProvider dataSecurityProvider)
            : base(sessionManager)
        {
            mDataSecurityProvider = dataSecurityProvider;
        }

        public override T Get(int id)
        {
            var result = Session.Get<T>(id);
            if (result == null)
                return null;

            var primaryEmailOfRecordOwner = GetPrimaryEmailOfRecordOwner(result);
            if (primaryEmailOfRecordOwner == null)
                return result;

            if (!mDataSecurityProvider.CurrentUserCanAccess(primaryEmailOfRecordOwner))
            {
                LoggingHelper.LogError("User {CurrentUserEmail} is attempting to access {RecordType} ID {RecordId} but record is owned by {RecordOwnerEmail}", 
                    mDataSecurityProvider.CurrentUserEmail, typeof(T).Name, id, primaryEmailOfRecordOwner);
                throw new Exception("Context user's email doesn't match the record accessed.");
            }

            return result;
        }

        protected virtual string GetPrimaryEmailOfRecordOwner(T record)
        {
            return null;
        }
    }
}
