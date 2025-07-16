using System;
using System.Collections.Generic;
using System.Linq;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using Newtonsoft.Json;
using User = F1Solutions.Naati.Common.Dal.Domain.User;

namespace F1Solutions.Naati.Common.Dal.Finance
{
    public class ExternalAccountingQueueService
    {
        public ExternalAccountingOperation QueueOperation(ExternalAccountingOperationTypeName type, object data,
            string reference, object completionData, string description, int userId, int? prerequisiteId, bool batchProcess)
        {
            var operation = new ExternalAccountingOperation();
            //{
            operation.RequestedByUser = NHibernateSession.Current.Get<User>(userId);
                operation.RequestedDateTime = DateTime.Now;
            operation.InputType = data.GetType().FullName;
            operation.Type = NHibernateSession.Current.Load<ExternalAccountingOperationType>((int)type);
                operation.Status = NHibernateSession.Current.Load<ExternalAccountingOperationStatus>(
                    (int)ExternalAccountingOperationStatusName.Requested);
            operation.Input = JsonConvert.SerializeObject(data);
                operation.CompletionType = completionData != null ? completionData.GetType().FullName : null;
            operation.CompletionInput = completionData != null ? JsonConvert.SerializeObject(completionData) : null;
            operation.Reference = reference;
                operation.PrerequisiteOperation = prerequisiteId.HasValue ? NHibernateSession.Current.Get<ExternalAccountingOperation>(prerequisiteId) : null;
            operation.Description = description;
            operation.BatchProcess = batchProcess;
            //};

            NHibernateSession.Current.Save(operation);
            NHibernateSession.Current.Flush();

            return operation;
        }

        public WiiseOperation GetOperation(ExternalAccountingOperation operationRequest)
        {
            var operationType = Type.GetType(operationRequest.InputType);
            var operation = JsonConvert.DeserializeObject(operationRequest.Input, operationType) as WiiseOperation;
            if (operation == null)
            {
                throw new Exception(string.Format(
                    "Failed to deserialise external accounting operation ({0}). Operation ID: {1}",
                    operationRequest.Type, operationRequest.Id));
            }

            return operation;
        }

        public WiiseCompletionOperation GetCompletionOperation(ExternalAccountingOperation operationRequest)
        {
            WiiseCompletionOperation operation = null;
            if (!string.IsNullOrEmpty(operationRequest.CompletionType))
            {
                var operationType = Type.GetType(operationRequest.CompletionType);
                operation = JsonConvert.DeserializeObject(operationRequest.CompletionInput, operationType) as WiiseCompletionOperation;
                if (operation == null)
                {
                    throw new Exception(string.Format(
                        "Failed to deserialise external accounting completion operation ({0}). Operation ID: {1}",
                        operationRequest.CompletionType, operationRequest.Id));
                }
            }
            return operation;
        }

        public T GetOperation<T>(ExternalAccountingOperation operationRequest) where T : WiiseOperation
        {
            var operation = JsonConvert.DeserializeObject<T>(operationRequest.Input);
            if (operation == null)
            {
                throw new Exception(string.Format(
                    "Failed to deserialise external accounting operation ({0}). Operation ID: {1}",
                    operationRequest.Type, operationRequest.Id));
            }

            operation.OperationId = operationRequest.Id; 
            return operation;
        }

        public ExternalAccountingOperation GetOperationRequest(int operationId)
        {
            return NHibernateSession.Current.Get<ExternalAccountingOperation>(operationId);
        }

        public void SetOperationStatusAndSave(ExternalAccountingOperation operation,
            ExternalAccountingOperationStatusName status, int userId)
        {
            operation.Status = NHibernateSession.Current.Load<ExternalAccountingOperationStatus>((int)status);
            operation.RequestedByUser = NHibernateSession.Current.Get<User>(userId);
            NHibernateSession.Current.Save(operation);
            NHibernateSession.Current.Flush();
        }

        public void CancelOperation(int operationId)
        {
            using (var trans = NHibernateSession.Current.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
            {
                try
                {
                    var request = GetOperationRequest(operationId);
                    if (request == null)
                    {
                        trans.Rollback();
                        return;
                    }

                    var status = (ExternalAccountingOperationStatusName)request.Status.Id;
                    if (status != ExternalAccountingOperationStatusName.Requested
                        && status != ExternalAccountingOperationStatusName.Failed)
                    {
                        throw new WebServiceException(string.Format("The operation cannot be cancelled: request status is '{0}'.",
                            request.Status.DisplayName));
                    }

                    NHibernateSession.Current.Delete(request);
                    NHibernateSession.Current.Flush();
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<ExternalAccountingOperation> GetQueuedOperations(IEnumerable<ExternalAccountingOperationStatusName> statuses, DateTime? requestedFrom, DateTime? requestedTo)
        {
            var statusIds = statuses.Select(x => (int)x).ToArray();
            var query = NHibernateSession.Current.Query<ExternalAccountingOperation>();

            if (statuses != null && statuses.Any())
            {
                query = query.Where(x => statusIds.Contains(x.Status.Id));
            }

            if (requestedFrom.HasValue)
            {
                var from = requestedFrom.Value.Date;
                query = query.Where(x => x.RequestedDateTime >= new DateTime(from.Year, from.Month, from.Day, 0, 0, 0));
            }

            if (requestedTo.HasValue)
            {
                var to = requestedTo.Value.Date;
                query = query.Where(x => x.RequestedDateTime <= new DateTime(to.Year, to.Month, to.Day, 23, 59, 59));
            }

            return query
                .OrderByDescending(x => x.Id)
                .Take(500)
                .ToList();
        }

        public IList<ExternalAccountingOperation> GetQueuedBatchOperations(params ExternalAccountingOperationStatusName[] statuses)
        {
            var statusIds = statuses.Select(x => (int)x).ToArray();
            var query = NHibernateSession.Current.Query<ExternalAccountingOperation>()
                .Where(x => x.BatchProcess);


            if (statuses.Any())
            {
                query = query.Where(x => statusIds.Contains(x.Status.Id));
            }

            return query.ToList();
        }
    }
}
