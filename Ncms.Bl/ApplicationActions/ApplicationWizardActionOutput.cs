using System;
using System.Collections.Generic;
using Ncms.Contracts;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.System;

namespace Ncms.Bl.ApplicationActions
{
    public class ApplicationActionOutput : SytemActionOutput<UpsertApplicationResultModel, CredentialApplicationEmailMessageModel> {
       
        public ApplicationInvoiceCreateRequestModel PendingInvoice { get; set; }
        public CredentialModel PendingCredential { get; set; }      
        public IList<CredentialRequestModel> PendingCredentialRequests { get; set; }
        public string InvoiceNumber { get; set; }
        public Guid InvoiceId { get; set; }
        public string CreditNoteNumber { get; set; }
        public Guid CreditNoteId { get; set; }
        public ProcessFeeModel PendingProcessFee { get; set; }
        public int OperationId { get; set; }
    }
}