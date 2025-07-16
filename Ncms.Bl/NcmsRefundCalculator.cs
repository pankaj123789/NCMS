using F1Solutions.Naati.Common.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Bl.Refund;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using Ncms.Contracts;
using System;
using System.Linq;

namespace Ncms.Bl
{
    public class NcmsRefundCalculator : RefundCalculator
    {
        private readonly IFinanceService mFinanceService;
        private readonly ISystemQueryService mSystemQueryService;

        public NcmsRefundCalculator(
            IApplicationQueryService applicationQueryService,
            IFinanceService financeService,
            ISystemQueryService systemQueryService
            ) : base(applicationQueryService, systemQueryService)
        {
            mFinanceService = financeService;
            mSystemQueryService = systemQueryService; 
        }

        public override RefundCalculationDto CalculateRefund(int credentialRequestId)
        {
            var refundResponse = base.CalculateRefund(credentialRequestId);
            if (refundResponse.AvailableRefundMethodTypes.Any() && (refundResponse.PaidAmount == null || refundResponse.RefundableAmount == null))
            {
                var invoicesResponse = mFinanceService.GetInvoices(new GetInvoicesRequest
                {
                    InvoiceNumber = new string[] { refundResponse.InvoiceNumber }
                });

                if (invoicesResponse != null && invoicesResponse.Invoices.Any())
                {
                    var invoice = invoicesResponse.Invoices.Single();

                    var glCode = mSystemQueryService.GetSystemValue(new GetSystemValueRequest { ValueKey = "PayPalGlCode" }).Value;
                    var externalAccountingId = mFinanceService.GetExternalAccountIdByCode(glCode);

                    refundResponse.RefundableAmount = CalculateRefundAmount(invoice, externalAccountingId);
                    refundResponse.PaidAmount = invoice.Payment;

                    if (refundResponse.CredentialApplicationRefundId != null)
                    {
                        //refund request exists
                        _mApplicationQueryService.UpdateCredentialApplicationRefundRequest(new UpdateCredentialApplicationRefundRequest
                        {
                            Id = refundResponse.CredentialApplicationRefundId.Value,
                            InitialPaidAmount = CalculatePaidAmount(invoice, externalAccountingId),
                            InitialPaidTax = invoice.TotalTax
                        });
                    }
                }
            }          
            return refundResponse;
        }

        private static decimal CalculateRefundAmount(InvoiceDto invoice, Guid externalAccountingId)
        {
            if (invoice.LineItems.Length < 2)
            {
                return invoice.Payment;
            }
            else
            {
                //note!!!: if glCode is to be changed then make sure all refunds are processed before changing otherwise refunds for old data will not work
                var surcharge = invoice.LineItems.ToList().FirstOrDefault(x => x.AccountId == externalAccountingId); //this is a paypal surcharge
                if (surcharge != null && surcharge.LineAmount.HasValue)
                {
                    return invoice.Payment - surcharge.LineAmount.Value;
                }
                else
                {
                    return invoice.Payment;
                }
            }
        }

        public static decimal? CalculatePaidAmount(InvoiceDto invoice, Guid externalAccountingId)
        {
            if (invoice.LineItems.Length < 2)
            {
                return invoice.Payment;
            }
            else
            {
                //note!!!: if glCode is to be changed then make sure all refunds are processed before changing otherwise refunds for old data will not work
                var surcharge = invoice.LineItems.ToList().FirstOrDefault(x => x.AccountId == externalAccountingId); //this is a paypal surcharge
                if(surcharge != null && surcharge.LineAmount.HasValue)
                {
                    return invoice.Payment - surcharge.LineAmount.Value;
                }
                else
                {
                    return invoice.Payment;
                }
            }
        }

    }
}
