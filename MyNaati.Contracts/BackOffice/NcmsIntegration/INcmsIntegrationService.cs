using System;
using System.Collections.Generic;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace MyNaati.Contracts.BackOffice.NcmsIntegration
{
    public interface INcmsIntegrationService : IInterceptableservice
    {
        CreateNcmsApplicationResponse CreateApplication(CreateNcmsApplicationRequest request);

        BusinessServiceResponse CreateNcmsApplicationCredentialRequest(
            int applicationId,
            int skillId,
            int credentialTypeId,
            int categoryId);

        BusinessServiceResponse ExecuteNcmsApplicationAction(NcmsApplicationActionRequest request);

        BusinessServiceResponse ExecuteRolePlayerAction(NcmsRolePlayerActionRequest request);
        BusinessServiceResponse UpdateNcmsOutstandingInvoices(string invoiceNumber, string paymentReference, string transactionId, string orderNumber);

        IList<string> ValidateCredentialApplication(NcmsApplicationActionRequest request);

        IList<string> ValidateMaterialRequest(NcmsMaterialRequestActionRequest request);

        IList<string> ExecuteMaterialRequestAction(NcmsMaterialRequestActionRequest request);
        bool TestConnection();
    }

    public class CreateNcmsApplicationRequest
    {
        public int ApplicationTypeId { get; set; }

        public int NaatiNumber { get; set; }

        public int ApplicationStatusTypeId { get; set; }
    }

    public class NcmstValidationResponse : GenericResponse<IList<NcmsValidationResultModel>>
    {
    }

    public class NcmsValidationResultModel
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class NcmsMaterialRequestActionRequest
    {
        public int MaterialRequestId { get; set; }

        public int MaterialRequestRoundId { get; set; }

        public int TestMaterialDomainId { get; set; }

        public int ActionId { get; set; }

        public IList<object> Steps { get; set; }
    }

    public class NcmsRolePlayerActionRequest
    {
        public int? TestSessionRolePlayerId { get; set; }

        public int ActionId { get; set; }
    }

    public class NcmsApplicationActionRequest
    {
        public int ApplicationId { get; set; }

        public int CredentialRequestId { get; set; }

        public int ActionId { get; set; }

        public IList<object> Steps { get; set; }

        public DateTime? DueDate { get; set; }

        public string InvoiceReference { get; set; }

        public string PaymentReference { get; set; }

        public decimal PaymentAmount { get; set; }

        public int? TestSessionId { get; set; }

        public double? RefundPercentage { get; set; }

        public int? RefundMethodTypeId { get; set; }

        public int? CredentialWorkflowFeeId { get; set; }

        public string OrderNumber { get; set; }

        public string TransactionId { get; set; }

        public string RefundComments { get; set; }
        public string RefundBankDetails { get; set; }
    }

    public class CreateNcmsApplicationResponse : GenericResponse<CreateApplicationResultModel>
    {
    }
}