using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Security;
using F1Solutions.Naati.Common.Dal.Domain;

using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using Infl = Inflector.Inflector;


namespace F1Solutions.Naati.Common.Dal.NHibernate.Auditing
{
    public class AuditingListener : IPreUpdateEventListener, IPreInsertEventListener, IPreDeleteEventListener, IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        public static readonly int INSERT_AUDIT_TYPE = 1;
        public static readonly int UPDATE_AUDIT_TYPE = 2;
        public static readonly int DELETE_AUDIT_TYPE = 3;

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken cancellationToken)
        {
           return new Task<bool>(()=> false);
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
           
            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken cancellationToken)
        {
            return new Task<bool>(() => false);
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            int[] dirtyIndices = @event.Persister.FindDirty(@event.State, @event.OldState, @event.Entity, @event.Session);

            if (dirtyIndices == null)
                return false;

            var changeDetail = new StringBuilder(string.Format("{0} record ID: {1} was updated. Changed values:", GetAuditName(@event.Entity), GetIdentifier(@event.Persister, @event.Entity)));
            foreach (int index in dirtyIndices)
            {
                string propertyName = Infl.Titleize(@event.Persister.PropertyNames[index]);

                changeDetail.AppendFormat(" {0} changed from '{1}' to '{2}' ;", propertyName, @event.OldState[index], @event.State[index]);
            }

            int[] unchangedIndices = Enumerable.Range(0, @event.State.Length)
                .Except(dirtyIndices)
                .ToArray();

            if (unchangedIndices.Length > 0)
            {
                changeDetail.AppendFormat(" Unchanged values = ;");
                AppendPropertyValues(unchangedIndices.Select(x => @event.State[x]).ToArray(),
                    unchangedIndices.Select(x => @event.Persister.PropertyNames[x]).ToArray(),
                    unchangedIndices.Select(x => @event.Persister.PropertyUpdateability[x]).ToArray(),
                    changeDetail);
            }
        
            WriteAuditRecord(@event.Persister, @event.Entity, @event.Session, changeDetail, UPDATE_AUDIT_TYPE);

            return false;
        }

        public Task<bool> OnPreDeleteAsync(PreDeleteEvent @event, CancellationToken cancellationToken)
        {
            return new Task<bool>(() => false);
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            var changeDetail = new StringBuilder(string.Format("{0} record ID: {1} was deleted. Record values:", GetAuditName(@event.Entity), GetIdentifier(@event.Persister, @event.Entity)));
            AppendPropertyValues(@event.DeletedState, @event.Persister.PropertyNames, @event.Persister.PropertyUpdateability, changeDetail);

            WriteAuditRecord(@event.Persister, @event.Entity, @event.Session, changeDetail, DELETE_AUDIT_TYPE);

            return false;
        }
      
        private void AppendPropertyValues(object[] values, string[] propertyNames, bool[] include, StringBuilder changeDetail)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (include[i])
                {
                    string propertyName = Infl.Titleize(propertyNames[i]);
                    object value = values[i];

                    object displayValue = (value as IDynamicLookupType)?.DisplayName;

                    if (displayValue == null && value != null && value is IAuditObject && !(value is ILookupType))
                    {
                        displayValue = ((IAuditObject)value).Id;
                    }

                    changeDetail.AppendFormat(" {0} = '{1}' ;", propertyName, displayValue ?? value);
                }
            }
        }


        private void UpdateProcessedFlag(AbstractPostDatabaseOperationEvent postEvent)
        {
            foreach (var collection in postEvent.Session.PersistenceContext.CollectionEntries.Values)
            {
                var collectionEntry = collection as CollectionEntry;
                collectionEntry.IsProcessed = true;
            }
        }

        public Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            return new Task<bool>(() => false);
        }

        public void OnPostUpdate(PostUpdateEvent postEvent)
        {
            UpdateProcessedFlag(postEvent);
        }

        protected virtual string GetCurrentUserName()
        {
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;

            if (string.IsNullOrWhiteSpace(userName))
            {
                var secretsProvider = ServiceLocator.Resolve<ISecretsCacheQueryService>();
                userName = secretsProvider.Get(SecuritySettings.NcmsDefaultIdentityKey);
            }

            return userName;
        }

        private void WriteAuditRecord(IEntityPersister persister, object entity, IEventSource session, StringBuilder changeDetail, int auditType)
        {
            string username = GetCurrentUserName();

            object entityId = GetIdentifier(persister, entity);
            string entityName = GetAuditName(entity);

            IAuditObject rootAuditObject = GetRootAuditObject(persister, entity, session, auditType);
            string rootAuditObjectName = string.Empty;
            int? rootAuditObjectId = null;

            if (rootAuditObject != null)
            {
                rootAuditObjectName = rootAuditObject.AuditName;
                rootAuditObjectId = rootAuditObject.Id;
            }

            var command = session.ConnectionManager.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText =
                @"INSERT INTO tblAuditLog (ParentName, RecordName, ParentId, RecordId, AuditTypeId, ChangeDetail, UserId, DateTime) 
 VALUES (@pParentName, @pRecordName, @pParentId, @pRecordId, @pAuditTypeId, @pChangeDetail, (SELECT UserId FROM tblUser WHERE UserName = @pUserName), @pDateTime)";

            command.AddParameter("@pParentName", rootAuditObjectName);
            command.AddParameter("@pRecordName", entityName);
            command.AddParameter("@pParentId", rootAuditObjectId);
            command.AddParameter("@pRecordId", entityId);
            command.AddParameter("@pAuditTypeId", auditType);
            command.AddParameter("@pChangeDetail", changeDetail.ToString());
            command.AddParameter("@pUserName", username);
            command.AddParameter("@pDateTime", DateTime.Now);

            session.Transaction.Enlist(command);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(string.Format(
                    "Failed to update AuditLog. Username: {0}, ParentName: {1}, RecordName: {2}, EntityId: {3}, Database: {4}, AuditType: {5} ",
                    username,
                    rootAuditObjectName,
                    entityName,
                    entityId,
                    command.Connection.Database,
                    auditType), ex);
            }
        }

        private string GetAuditName(object entity)
        {
            return Infl.Titleize(((IAuditObject)entity).AuditName);
        }

        private object GetIdentifier(IEntityPersister persister, object entity)
        {
            return persister.GetIdentifier(entity);
        }

        private IAuditObject GetRootAuditObject(IEntityPersister persister, object entity, IEventSource session, int auditType)
        {
            // if (auditType == DELETE_AUDIT_TYPE)
            // {
            //    return GetRootAuditObjectForDeletedObject(persister, entity, session);
            // }

            var entityBase = entity as EntityBase;

            return entityBase?.RootAuditObject;
        }

        private IAuditObject GetRootAuditObjectForDeletedObject(IEntityPersister persister, object entity, IEventSource session)
        {
            using (var tempSession = session.Factory.OpenSession())
            using (var tempTx = tempSession.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                var entityBase = (EntityBase)tempSession.Get(persister.EntityName, GetIdentifier(persister, entity));
                return entityBase.RootAuditObject;
            }
        }

        public Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellationToken)
        {
            return new Task<bool>(() => false);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var changeDetail = new StringBuilder(string.Format("{0} record ID: {1} was created. New values:", GetAuditName(@event.Entity), GetIdentifier(@event.Persister, @event.Entity)));
            AppendPropertyValues(@event.State, @event.Persister.PropertyNames, @event.Persister.PropertyInsertability, changeDetail);

            WriteAuditRecord(@event.Persister, @event.Entity, @event.Session, changeDetail, INSERT_AUDIT_TYPE);
        }

        public Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            return new Task<bool>(() => false);
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            UpdateProcessedFlag(@event);
        }
    }

    public static class AuditingExtensions
    {
        public static void AddParameter(this IDbCommand command, string parameterName, object parameterValue)
        {
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = parameterValue ?? DBNull.Value;
            command.Parameters.Add(param);
        }
    }
}