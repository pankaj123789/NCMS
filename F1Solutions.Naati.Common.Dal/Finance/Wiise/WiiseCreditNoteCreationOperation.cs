using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Bl;
using F1Solutions.Naati.Common.Contracts.Dal;
using InvoiceType = F1Solutions.Naati.Common.Contracts.Dal.Enum.InvoiceType;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using WiiseCreditNoteType = F1Solutions.Naati.Common.Wiise.PublicModels.CreditNote.TypeEnum;
using static F1Solutions.Naati.Common.Wiise.PublicModels.CreditNote;

namespace F1Solutions.Naati.Common.Dal.Finance.Wiise
{
    public class WiiseCreditNoteCreationOperation : WiiseOperation<WiiseCreateCreditNoteRequest, CreditNote>
    {
        private readonly IAutoMapperHelper _autoMapperHelper;

        public WiiseCreditNoteCreationOperation()
        {
            _autoMapperHelper = ServiceLocator.Resolve<IAutoMapperHelper>();
            
        }
        protected override async Task PrepareInput()
        {
            var invoiceType = Request.InvoiceType.ToWiiseCreditNoteType();

            Contact contact = null;

            if (invoiceType == WiiseCreditNoteType.ACCPAYCREDIT)
            {
                if (string.IsNullOrEmpty(Request.Contact.AccountNumber))
                {
                    throw new WebServiceException("The examiner must have a Supplier Account Number in order to generate a credit note. This can be set in the Person Settings screen.");
                }

                try
                {
                    var result = await Wiise.GetContactsAsync(Token.Value, Token.Tenant, where: $"AccountNumber = \"{Request.Contact.AccountNumber}\"");
                    contact = result.Data._Contacts.FirstOrDefault();
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                if (contact == null)
                {
                    throw new WebServiceException($"A supplier with account number \"{Request.Contact.AccountNumber}\" cannot be found in Wiise.");
                }
            }
            else if (Request.Contact != null)
            {
                contact = WiiseContactCreationOperation.GetWiiseContact(Request.Contact);
            }

            var creditNote = new CreditNote
            {
                CreditNoteNumber = Request.CreditNoteNumber,
                Reference = Request.Reference,
                Contact = contact,
                LineItems = Request.LineItems.Select(x=>ToPublicLineItem(x)).ToList(),
                CreditNoteType = invoiceType,
                DueDate = Request.DueDate.Value,
                PostCreditNoteOnCreate = Request.PostCreditNoteOnCreate
            };

            Input = creditNote;
        }

        public static Common.Wiise.PublicModels.LineItem ToPublicLineItem(WiiseLineItemModel item)
        {
            return new LineItem()
            {
                //AccountCode = item.AccountCode,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitAmount = item.UnitAmount,
                AccountId = item.AccountId.Value
            };
        }

        protected void LogCreditNote(CreditNote creditNote)
        {
            try
            {
                if (creditNote.CreditNoteType == TypeEnum.ACCRECCREDIT)
                {
                    LoggingHelper.LogInfo("Wiise CreditNote {CreditNoteNumber} created; NAATI No: {NaatiNumber}, Amount: {CreditNoteAmount}, Items(s): {@CreditNoteItems}",
                        creditNote.CreditNoteNumber, creditNote.Contact.ContactNumber, creditNote.Total, creditNote.LineItems.Select(x => x.AccountId));
                }
                else
                {
                    LoggingHelper.LogInfo("Wiise Bill {CreditNoteNumber} created; Account No: {AccountNumber}, Amount: {CreditNoteAmount}, Items(s): {@CreditNoteItems}",
                        creditNote.CreditNoteNumber, creditNote.Contact.AccountNumber, creditNote.Total, creditNote.LineItems.Select(x => x.AccountId));
                }
            }
            catch { }
        }

        protected override async Task<CreditNote> ProtectedPerformOperation()
        {
            var creditNotes = new CreditNotes
            {
                _CreditNotes = new List<CreditNote> { Input }
            };
            var results = await Wiise.CreateCreditNotesAsync(Token.Value, Token.Tenant, creditNotes);
            var result = results.Data._CreditNotes.FirstOrDefault();
            LogCreditNote(result);
            return result;
        }

        protected override void PrepareOutput()
        {
            if (ProtectedResult != null)
            {
                Output = ProtectedResult.CreditNoteNumber;
            }
        }
    }

    public class WiiseCreateCreditNoteRequest : WiiseOperationRequest
    {
        public string CreditNoteNumber { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public WiiseCreateContactRequest Contact { get; set; }
        public Guid BrandingThemeId;
        public DateTime? DueDate;
        public string Reference;
        public IEnumerable<WiiseLineItemModel> LineItems;
        public IEnumerable<int> CandidatesNaatiNumber { get; set; }
        public bool PostCreditNoteOnCreate { get; set; }
    }
}
