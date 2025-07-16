using F1Solutions.Naati.Common.Wiise.NativeModels;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace F1Solutions.Naati.Common.Wiise.Mappers
{
    internal static class InvoiceMapper
    {

        internal static NativeModels.SalesInvoices ToNativeSalesInvoices(this PublicModels.Invoices invoices)
        {
            var result = new NativeModels.SalesInvoices
            {
                _SalesInvoices = new List<SalesInvoice>()
            };
            foreach (var invoice in invoices._Invoices)
            {
                if (invoice.HasValidationErrors)
                {
                    var baseModel = invoice.ToNativeBaseModel();
                    result._SalesInvoices.Add(new NativeModels.SalesInvoice(baseModel));
                }
                else
                {
                    result._SalesInvoices.Add(invoice.ToNativeSalesInvoice());
                }
            }
            return result;
        }

        internal static NativeModels.PurchaseInvoices ToNativePurchaseInvoices(this PublicModels.Invoices invoices)
        {
            var result = new NativeModels.PurchaseInvoices
            {
                _PurchaseInvoices = new List<PurchaseInvoice>()
            };
            foreach (var invoice in invoices._Invoices)
            {
                if (invoice.HasValidationErrors)
                {
                    var baseModel = invoice.ToNativeBaseModel();
                    result._PurchaseInvoices.Add(new NativeModels.PurchaseInvoice(baseModel));
                }
                else
                {
                    result._PurchaseInvoices.Add(invoice.ToNativePurchaseInvoice());
                }
            }
            return result;
        }

        internal static PublicModels.Invoices ToPublicInvoices(this NativeModels.SalesInvoices invoices)
        {
            var result = new PublicModels.Invoices()
            {
                _Invoices = new List<PublicModels.Invoice>()
            };
            foreach (var invoice in invoices._SalesInvoices)
            {
                if (invoice.HasValidationErrors)
                {
                    var baseModel = invoice.ToPublicBaseModel();
                    result._Invoices.Add(new PublicModels.Invoice(baseModel));
                }
                else
                {
                    result._Invoices.Add(invoice.ToPublicInvoice());
                }
            }
            return result; 
        }

        #region mappers

        #region ToPublic
        internal static PublicModels.Invoices ToPublicInvoices(this NativeModels.PurchaseInvoices invoices)
        {
            return new PublicModels.Invoices()
            {
                _Invoices = invoices._PurchaseInvoices.Select(x => x.ToPublicInvoice()).ToList()
            };
        }

        internal static PublicModels.Invoice ToPublicInvoice(this NativeModels.PurchaseInvoice invoice)
        {
            return new PublicModels.Invoice()
            {
                InvoiceNumber = invoice.number,
                Date = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Reference = invoice.vendorInvoiceNumber,
                Status = ToPublicInvoiceStatus(invoice.status),
                HasErrors = invoice.HasValidationErrors,
                HasValidationErrors = invoice.HasValidationErrors,                
            };
        }

        internal static PublicModels.Invoice ToPublicInvoice(this NativeModels.SalesInvoice invoice)
        {
            return new PublicModels.Invoice
            {
                InvoiceID = invoice.Id,
                InvoiceNumber = invoice.Number,
                Reference = invoice.ExternalDocumentNumber,
                LineItems = invoice.SalesInvoiceLines?.ToPublicInvoiceLineItems(),
                Total = invoice.TotalAmountIncludingTax,
                AmountDue = invoice.RemainingAmount,
                Contact = new PublicModels.Contact
                {
                    AccountNumber = invoice.customerNumber,
                    EmailAddress = invoice.Email,
                    ContactNumber = invoice.customerNumber,
                    FirstName = invoice.customerName, //needs to be split
                    LastName = invoice.customerName, //needs to be split
                    ContactID = invoice.customerId,
                    Addresses = invoice.BillingPostalAddress == null ? new List<PublicModels.Address>() : new List<PublicModels.Address> { 
                        new PublicModels.Address
                        {
                            AddressLine1 = invoice.BillingPostalAddress.street,
                            City = invoice.BillingPostalAddress.city,
                            Country = invoice.BillingPostalAddress.countryLetterCode,
                            Region = invoice.BillingPostalAddress.street,
                            PostalCode = invoice.BillingPostalAddress.postalCode
                        }
                    }
                },
                DueDate = invoice.DueDate,
                Date = invoice.InvoiceDate,
                InvoiceType = PublicModels.InvoiceType.Invoice,
                TotalTax = invoice.TotalTaxAmount,
                Status = !string.IsNullOrEmpty(invoice.Status) ? ToPublicInvoiceStatus(invoice.Status) : (PublicModels.Invoice.StatusEnum?)null,
                Payments = ToPublicInvoicePayments(invoice),               
            };
        }

        private static List<PublicModels.Payment> ToPublicInvoicePayments(SalesInvoice invoice)
        {
            var payments = new List<PublicModels.Payment>();
            if(invoice.Status == "Paid")
            {
                payments.Add(new PublicModels.Payment
                {
                    InvoiceId = invoice.Id.Value,
                    InvoiceNumber = invoice.Number,
                    Amount = Decimal.ToDouble(invoice.TotalAmountIncludingTax.Value),
                    Date = invoice.PostingDate,
                    //Account
                    //Reference
                });
            }
            return payments;
        }

        internal static List<PublicModels.LineItem> ToPublicInvoiceLineItems(this List<NativeModels.LineItem> lineItems)
        {
            var publicLineItems = new List<PublicModels.LineItem>();
            if (lineItems != null)
            {
                foreach (NativeModels.LineItem lineItem in lineItems)
                {
                    publicLineItems.Add(new PublicModels.LineItem()
                    {
                        AccountId = lineItem.accountId.Value,
                        Gst = lineItem.taxable.HasValue? lineItem.taxable.Value:false,
                        Quantity = lineItem.quantity,
                        LineItemID = lineItem.itemId,                        
                        LineAmount = lineItem.amountIncludingTax.HasValue ? Convert.ToDecimal(lineItem.amountIncludingTax.Value) : (Decimal?)null,
                        UnitAmount = lineItem.unitPrice.HasValue ? Convert.ToDecimal(lineItem.unitPrice.Value) : (Decimal?)null,
                        Description = lineItem.description
                    });
                }
            }
            return publicLineItems;
        }

        public static SalesInvoice.StatusEnum ToNativeInvoiceStatus(string publicStatus)
        {
            switch(publicStatus)
            {
                case "DRAFT":
                    return SalesInvoice.StatusEnum.Draft;
                case "PAID":
                    return SalesInvoice.StatusEnum.Paid;
                case "OPEN":
                    return SalesInvoice.StatusEnum.Open;
                default:
                    throw new Exception($"Unknown Public Invoice Status '{publicStatus}'");
            }
        }

        public static Invoice.StatusEnum ToPublicInvoiceStatus(string nativeStatus)
        {
            switch (nativeStatus)
            {
                case "Draft":
                    return Invoice.StatusEnum.DRAFT;
                case "Paid":
                    return Invoice.StatusEnum.PAID;
                case "Open":
                    return Invoice.StatusEnum.OPEN;
                case "Canceled":
                    return Invoice.StatusEnum.CANCELED;
                default:
                    throw new Exception($"Unknown Native Invoice Status '{nativeStatus}'");
            }
        }

        #endregion

        #region ToNative

        internal static NativeModels.PurchaseInvoice ToNativePurchaseInvoice(this PublicModels.Invoice invoice)
        {
            var lineItem = invoice.LineItems.FirstOrDefault();
            return new NativeModels.PurchaseInvoice
            {
                //noteForCustomer = invoice.Reference,
                InvoiceDate = invoice.Date ?? DateTime.Now,
                PostingDate = DateTime.Now,
                vendorInvoiceNumber = invoice.InvoiceNumber,
                //totalAmountIncludingTax = lineItem != null ? Convert.ToDouble(lineItem.LineAmount.Value) : (double ?) null,
                //invoice.AmountDue.HasValue?Decimal.ToDouble(invoice.AmountDue.Value):(double?)null,
                vendorId = invoice.Contact.ContactID.Value,
                purchaseInvoiceLines = invoice.LineItems.ToNativeInvoiceLineItems(InvoiceType.Purchase, invoice.Contact)

            };
        }

        internal static NativeModels.SalesInvoice ToNativeSalesInvoice(this PublicModels.Invoice invoice)
        {
            return new NativeModels.SalesInvoice
            {
                customerNumber = invoice.Contact.AccountNumber,
                CustomerPurchaseOrderReference = invoice.Reference,
                InvoiceDate = invoice.Date ?? DateTime.Now,
                PostingDate = DateTime.Now,
                SalesInvoiceLines = invoice.LineItems.ToNativeInvoiceLineItems(InvoiceType.Sales, invoice.Contact),
                customerId = invoice.Contact.ContactID,
                DueDate = invoice.DueDate
            };
        }

        internal static List<NativeModels.LineItem> ToNativeInvoiceLineItems(this List<PublicModels.LineItem> lineItems, InvoiceType invoiceType, PublicModels.Contact contact)
        {
            var nativeLineItems = new List<NativeModels.LineItem>();
            foreach (PublicModels.LineItem lineItem in lineItems)
            {
                var item = new NativeModels.LineItem
                {
                    
                    accountId = lineItem.AccountId,
                    lineType = "Account",
                    quantity = Convert.ToInt16(lineItem.Quantity),
                    unitPrice = invoiceType == InvoiceType.Sales ? lineItem.LineAmount.HasValue ? Decimal.ToDouble(lineItem.LineAmount.Value) : (Double?)null : null,
                    unitCost = invoiceType == InvoiceType.Purchase ? lineItem.LineAmount.HasValue ? Decimal.ToDouble(lineItem.LineAmount.Value) : (Double?)null : null,
                    //amountIncludingTax = lineItem.LineAmount.HasValue ? Decimal.ToDouble(lineItem.LineAmount.Value) : (Double?)null,
                    //lineAmount = lineItem.LineAmount.HasValue ? Decimal.ToDouble(lineItem.LineAmount.Value) : (Double?)null,
                    description = lineItem.Description.Trunc(100),
                    DimensionSetLines = new List<DimensionSetLine>()
                };
                if (lineItem.Tracking != null)
                {
                    foreach (var tracking in lineItem.Tracking)
                    {
                        if (tracking.Name.Equals("Category", StringComparison.InvariantCultureIgnoreCase))
                        {
                            item.DimensionSetLines.Add(DimensionSetType.Category.ToDimensionSetLine(tracking.Option));
                        }
                        if (tracking.Name.Equals("Activity", StringComparison.InvariantCultureIgnoreCase))
                        {
                            item.DimensionSetLines.Add(DimensionSetType.Activity.ToDimensionSetLine(tracking.Option));
                        }
                    }
                }
                else
                {
                    var address = contact.Addresses.Single();
                    if (address.CountryCode == "AU")
                    {
                        item.DimensionSetLines = new List<DimensionSetLine>() { DimensionSetType.Category.ToDimensionSetLine(address.Region) };
                    }
                    else
                    {
                        item.DimensionSetLines = new List<DimensionSetLine>() { DimensionSetType.Category.ToDimensionSetLine($"OS {address.CountryCode}") };
                    }
                }
                nativeLineItems.Add(item);
            }
            return nativeLineItems;
        }
        #endregion
        #endregion

        public enum InvoiceType
        {
            Sales,
            Purchase
        }
    }
}
