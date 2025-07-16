using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using static F1Solutions.Naati.Common.Wiise.PublicModels.Invoice;
using InvoiceType = F1Solutions.Naati.Common.Contracts.Dal.Enum.InvoiceType;


namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseInvoiceCreationOperation : WiiseOperation<WiiseCreateInvoiceRequest, Invoice>
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public WiiseInvoiceCreationOperation()
        {
            _autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();
        }
        protected override async Task PrepareInput()
        {
            var invoiceType = Request.InvoiceType.ToWiiseInvoiceAccountType();

            Contact contact = null;
            //ACCPAY is a purchase invoice. It runs in real time, not batch. In this case the contact is the Vendor
            if (invoiceType == TypeEnum.ACCPAY)
            {
                if (string.IsNullOrEmpty(Request.Contact.AccountNumber))
                {
                    throw new WebServiceException("The examiner must have a Supplier Account Number in order to generate a bill. This can be set in the Person Settings screen.");
                }

                var result = await Wiise.GetVendorsAsync(Token.Value, Token.Tenant, where: $"AccountNumber = \"{Request.Contact.AccountNumber}\"");
                contact = result.Data._Contacts.FirstOrDefault();

                if (contact == null)
                {
                    throw new WebServiceException($"A supplier with account number \"{Request.Contact.AccountNumber}\" cannot be found in Wiise.");
                }
            }
            else if (Request.Contact != null)
            {
                contact = WiiseContactCreationOperation.GetWiiseContact(Request.Contact);
            }

            if(contact == null)
            {
                contact = WiiseContactCreationOperation.GetWiiseContact(Request.Contact);
            }

            var lineItems = new List<LineItem>();
            if (Request.LineItems != null)
            {
                lineItems.AddRange(Request.LineItems.Select(x =>
                {
                    var item = new LineItem()
                    {
                        Description = x.Description,
                        Gst = x.Gst,
                        LineAmount = x.UnitAmount,
                        Quantity = x.Quantity, 
                        AccountId = x.AccountId.Value
                    };

                    if (x.TrackingCategories?.Any() ?? false)
                    {
                        item.Tracking = new List<LineItemTracking>();
                        item.Tracking.AddRange(x.TrackingCategories.Select(y => new LineItemTracking
                        {
                            Name = y.Item1,
                            Option = y.Item2
                        }));
                    }


                    return item;
                }));
            }

            var invoice = new Invoice
            {
                InvoiceNumber = Request.InvoiceNumber,
                InvoiceType = (Common.Wiise.PublicModels.InvoiceType)Request.InvoiceType,
                Reference = Request.Reference,
                BrandingThemeID = Request.BrandingThemeId,
                Contact = contact,
                DueDate = Request.DueDate.HasValue
                    ? new DateTime(Request.DueDate.Value.ToLocalTime().Date.Ticks, DateTimeKind.Unspecified)
                    : (DateTime?)null,
                LineItems = lineItems,
                Type = invoiceType,
                Status = invoiceType == TypeEnum.ACCPAY
                    ? Invoice.StatusEnum.OPEN
                    : Invoice.StatusEnum.PAID,
                PaymentMethodId = Request.PaymentMethodId
            };

            Input = invoice;
        }

        protected void LogInvoice(Invoice invoiceOut, Invoice invoiceIn)
        {
            try
            {
                if (invoiceOut.HasValidationErrors || (invoiceOut.HasErrors.HasValue && invoiceOut.HasErrors.Value))
                {
                    if (invoiceOut.Type == TypeEnum.ACCREC)
                    {
                        LoggingHelper.LogError("Wiise Invoice Creation Error; NAATI No: {NaatiNumber}, Amount: {InvoiceAmount}, Items(s): {@InvoiceItems}",
                            invoiceOut.Contact.ContactNumber, invoiceOut.Total, invoiceOut.LineItems?.Select(x => x.AccountId));
                    }
                    else
                    {
                        LoggingHelper.LogError("Wiise Bill Creation Error; Account No: {AccountNumber}, Amount: {InvoiceAmount}, Items(s): {@InvoiceItems}",
                            invoiceIn.Contact.AccountNumber, invoiceIn.LineItems.First().LineAmount, invoiceIn.LineItems?.Select(x => x.Description));
                    }
                }
                else
                {
                    if (invoiceOut.Type == TypeEnum.ACCREC)
                    {
                        LoggingHelper.LogInfo("Wiise Invoice {InvoiceNumber} created; NAATI No: {NaatiNumber}, Amount: {InvoiceAmount}, Items(s): {@InvoiceItems}",
                            invoiceOut.InvoiceNumber, invoiceOut.Contact.ContactNumber, invoiceOut.Total, invoiceOut.LineItems?.Select(x => x.AccountId));
                    }
                    else
                    {
                        LoggingHelper.LogInfo("Wiise Bill {InvoiceNumber} created; Account No: {AccountNumber}, Amount: {InvoiceAmount}, Items(s): {@InvoiceItems}",
                            invoiceOut.InvoiceNumber, invoiceIn.Contact.AccountNumber, invoiceIn.LineItems.First().LineAmount, invoiceIn.LineItems?.Select(x => x.Description));
                    }
                }
            }
            catch { }
        }

        protected override async Task<Invoice> ProtectedPerformOperation()
        {
            var invoices = new Invoices
            {
                _Invoices = new List<Invoice> { Input }
            };
            var results = await Wiise.CreateInvoicesAsync(Token.Value, Token.Tenant, invoices);
            var result = results.Data._Invoices.FirstOrDefault();
            LogInvoice(result,invoices._Invoices.First());
            return result;
        }

        protected override void PrepareOutput()
        {
            if (ProtectedResult != null)
            {
                Output = ProtectedResult.InvoiceNumber;
            }
        }
    }

    public class WiiseCreateInvoiceRequest : WiiseOperationRequest
    {
        public string InvoiceNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public WiiseCreateContactRequest Contact { get; set; }
        public Guid BrandingThemeId;
        public DateTime? DueDate;
        public string Reference;
        public IEnumerable<WiiseLineItemModel> LineItems;
        public IEnumerable<int> CandidatesNaatiNumber { get; set; }
        public Guid? PaymentMethodId { get; set; }
    }
}
