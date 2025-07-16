using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class CreditNoteMapper
    {

        internal static NativeModels.CreditNotes ToNativeCreditNotes(this PublicModels.CreditNotes creditNotes)
        {
            return new NativeModels.CreditNotes()
            {
                _CreditNotes = creditNotes._CreditNotes.Select(x => x.ToNativeCreditNote()).ToList()
            };

        }

        internal static NativeModels.CreditNote ToNativeCreditNote(this PublicModels.CreditNote creditNote)
        {
            return new NativeModels.CreditNote
            {
                CustomerNumber = creditNote.Contact.AccountNumber,
                SalesCreditMemoLines = creditNote.LineItems.Select(lineItem => lineItem.ToNativeLineItem(creditNote.Contact)),
                Id = creditNote.CreditNoteID,
                Number = creditNote.CreditNoteNumber,
                DueDate = creditNote.DueDate,
                CreditMemoDate = DateTime.Now,
                PostingDate = DateTime.Now
            };
        }


        internal static NativeModels.SalesCreditMemoLine ToNativeLineItem(this PublicModels.LineItem lineItem, Contact contact)
        {
            var address = contact.Addresses.Single();

            var item = new NativeModels.SalesCreditMemoLine
            {
                AccountId = lineItem.AccountId,
                Description = lineItem.Description,
                Quantity = lineItem.Quantity.HasValue ? lineItem.Quantity.Value : 0,
                UnitPrice = lineItem.UnitAmount.HasValue ? lineItem.UnitAmount.Value : 0
            };

            if (address.CountryCode == "AU")
            {
                item.DimensionSetLines = new List<NativeModels.DimensionSetLine> { DimensionSetType.Category.ToDimensionSetLine(address.Region) };
            }
            else
            {
                item.DimensionSetLines = new List<NativeModels.DimensionSetLine> { DimensionSetType.Category.ToDimensionSetLine($"OS {address.CountryCode}") };
            }

            return item; 
        }

        internal static PublicModels.CreditNotes ToPublicCreditNotes(this NativeModels.CreditNotes creditNotes)
        {
            return new PublicModels.CreditNotes
            {
                _CreditNotes = creditNotes._CreditNotes.Select(creditNote => creditNote.ToPublicCreditNote()).ToList()
            };
        }


        //TODO - much more work on this mapping.
        internal static PublicModels.CreditNote ToPublicCreditNote(this NativeModels.CreditNote creditNote)
        {
            var baseModel = creditNote.ToPublicBaseModel();
            return new CreditNote
            {
                CreditNoteNumber = creditNote.Number,
                CreditNoteID = creditNote.Id, 
                Status = ToPublicModelStatus(creditNote.Status),
                LineItems = creditNote.SalesCreditMemoLines?.Select(lineItem => lineItem.ToPublicLineItem()).ToList(),
                HasValidationErrors = baseModel.HasValidationErrors,
                ValidationErrors = baseModel.ValidationErrors,
                DueDate = creditNote.DueDate,
                Date = creditNote.CreditMemoDate,
                Contact = new Contact
                {
                    ContactNumber = creditNote.CustomerNumber,
                    Name = creditNote.CustomerName
                },
                AppliedAmount = creditNote.TotalAmountIncludingTax.HasValue? creditNote.TotalAmountIncludingTax.Value:0,
                SubTotal = Convert.ToDecimal(creditNote.TotalAmountExcludingTax.HasValue? creditNote.TotalAmountExcludingTax.Value:0),
                Total = Convert.ToDecimal(creditNote.TotalAmountIncludingTax.HasValue ? creditNote.TotalAmountIncludingTax.Value : 0),
                Reference = creditNote.ExternalDocumentNumber,
                Payments = ToPublicCreditNotePayments(creditNote)
            };
        }

        private static List<Payment> ToPublicCreditNotePayments(NativeModels.CreditNote creditNote)
        {
            var payments = new List<PublicModels.Payment>();
            if (creditNote.Status == "Paid")
            {
                payments.Add(new PublicModels.Payment
                {
                    InvoiceId = creditNote.Id.Value,
                    InvoiceNumber = creditNote.Number,
                    Amount = decimal.ToDouble(creditNote.TotalAmountIncludingTax.Value),
                    Date = creditNote.PostingDate,
                    //Account
                    //Reference
                });
            }
            return payments;
        }

        private static CreditNote.StatusEnum ToPublicModelStatus(string status)
        {
            switch(status)
            {
                case "Open":
                    return CreditNote.StatusEnum.OPEN;
                case "Draft":
                    return CreditNote.StatusEnum.DRAFT;
                case "Paid":
                    return CreditNote.StatusEnum.PAID;
                default:
                    return CreditNote.StatusEnum.PAID;
            }
        }

        internal static PublicModels.LineItem ToPublicLineItem(this NativeModels.SalesCreditMemoLine lineItem)
        {
            return new LineItem
            {
                LineItemID = lineItem.ItemId,
                AccountId = lineItem.AccountId,
                UnitAmount = lineItem.UnitPrice,
                Quantity = lineItem.Quantity,
                Description = lineItem.Description
            };
        }

    }
}
