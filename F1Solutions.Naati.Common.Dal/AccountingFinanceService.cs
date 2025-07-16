using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using F1Solutions.Global.Common;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.CacheQuery;
using F1Solutions.Naati.Common.Contracts.Dal.DTO;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using F1Solutions.Naati.Common.Dal.Domain.Extensions;
using F1Solutions.Naati.Common.Dal.Finance;
using F1Solutions.Naati.Common.Dal.NHibernate.Configuration;
using NaatiInvoiceType = F1Solutions.Naati.Common.Contracts.Dal.Enum.InvoiceType;
using NaatiNumberTypeEnum = F1Solutions.Naati.Common.Contracts.Dal.Enum.NaatiNumberTypeEnum;
using User = F1Solutions.Naati.Common.Dal.Domain.User;
using F1Solutions.Naati.Common.Dal.Finance.PayPal;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Naati.Common.Dal.Finance.Wiise;
using F1Solutions.Naati.Common.Wiise.PublicModels;
using F1Solutions.Naati.Common.Wiise;
using Ncms.Contracts;
using F1Solutions.Naati.Common.Contracts.Bl.DTO;

namespace F1Solutions.Naati.Common.Dal
{
    public class AccountingFinanceService : IFinanceService
    {
        private const string WiisePaymentAccountParameterKey = "WIISEPAYMENTACCOUNT";
        private const string EmptyLastName = "[Not Stated]";
        private readonly ISecretsCacheQueryService _secretsProvider;
        private readonly IAutoMapperHelper _autoMapperHelper;
        private readonly IWiiseAccountingApi _wiiseAccountingApi;
        private readonly IWiiseTokenProvider _wiiseTokenProvider;
        //private readonly WiiseToken _wiiseToken;

        public AccountingFinanceService(
            ISecretsCacheQueryService secretsProvider, 
            IAutoMapperHelper autoMapperHelper, 
            IWiiseAccountingApi wiiseAccountingApi,
            IWiiseTokenProvider wiiseTokenProvider)
        {
            _secretsProvider = secretsProvider;
            _autoMapperHelper = autoMapperHelper;
            _wiiseTokenProvider = wiiseTokenProvider;
            _wiiseAccountingApi = wiiseAccountingApi;
           //wiiseToken = _wiiseTokenProvider.GetToken();
        }

        private class InvoicePayment
        {
            public int InvoiceId { get; set; }
            public int TransactionId { get; set; }
            public DateTime InvoiceCreateDate { get; set; }
            public int InvoiceOfficeId { get; set; }
            public string InvoiceOffice { get; set; }
            public int InvoiceLineId { get; set; }
            public decimal InvoiceLineAmount { get; set; }
            public int InvoiceNaatiNumber { get; set; }
            public DateTime InvoiceDueDate { get; set; }
            public string InvoiceCustomer { get; set; }
            public string BSB { get; set; }
            public string ChequeNumber { get; set; }
            public string BankName { get; set; }
            public string EftMachine { get; set; }
            public DateTime PaymentDate { get; set; }
            public decimal PaymentAmount { get; set; }
            public string PaymentType { get; set; }
            public string PaymentOffice { get; set; }
            public int PaymentNaatiNumber { get; set; }
            public string PaymentCustomer { get; set; }
        }

        private class InternalInvoice
        {
            public Guid Id { get; set; }
            public string InvoiceNumber { get; set; }
            public decimal Total { get; set; }
            public decimal? AmountDue { get; set; }
            public decimal Payment { get; set; }
            public int? NaatiNumber { get; set; }
            public string Customer { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime? Date { get; set; }
            public WiiseReference WiiseReference { get; set; }
            public Guid? ThemeId { get; set; }
            public InvoiceStatus Status { get; set; }
            public NaatiInvoiceType Type { get; set; }
            public List<Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>> Payments { get; set; }
            public decimal TotalTax { get; set; }
            public List<InternalLineItem> LineItems { get; set; }
            public string PaymentReference { get; set; }
        }

        private class InternalLineItem
        {
            //public string AccountCode { get; set; }
            public Guid AccountId { get; set; }
            public string Description { get; set; }
            public decimal? LineAmount { get; set; }
        }

        private string _wiisePaymentAccount;
        public string WiisePaymentAccount => _wiisePaymentAccount ?? (_wiisePaymentAccount = NHibernateSession.Current.Query<SystemValue>().Single(x => x.ValueKey.ToUpper() == WiisePaymentAccountParameterKey).Value);

        private AccountingProcessingService _accountingProcessing;
        private ExternalAccountingQueueService _wiiseQueue;

        private AccountingProcessingService AccountingProcessing => _accountingProcessing ?? (_accountingProcessing = new AccountingProcessingService(WiiseQueue));

        private ExternalAccountingQueueService WiiseQueue => _wiiseQueue ?? (_wiiseQueue = new ExternalAccountingQueueService());

        public GetOfficesResponse GetOffices()
        {
            return new GetOfficesResponse
            {
                Data = NHibernateSession.Current.Query<Office>()
                    .Select(x => new OfficeDto
                    {
                        Id = x.Id,
                        Name = x.Institution.InstitutionName,
                        Abbreviation = x.Institution.InstitutionAbberviation,
                    })
                    .ToArray()
            };
        }

        public GetEftMachinesResponse GetEftMachines()
        {
            return new GetEftMachinesResponse
            {
                Data = NHibernateSession.Current.Query<EFTMachine>()
                    .Where(x => x.Visible)
                    .Select(x => new EftMachineDto
                    {
                        Id = x.Id,
                        OfficeId = x.Office.Id,
                        TerminalNumber = x.TerminalNo
                    })
                    .ToArray()
            };
        }

        public GetBankAccountsResponse GetBankAccounts()
        {
            return ExecuteWiiseApi<GetBankAccountsResponse>(async (response, svc, token) =>
            {
                var accounts = await svc.Api.GetAccountsAsync(token.Value, token.Tenant, where: "Type=\"BANK\" || EnablePaymentsToAccount=True");
                response.BankAccounts = accounts.Data._Accounts.Select(a => new BankAccountDto
                {
                    Id = a.AccountID.GetValueOrDefault(),
                    Name = a.Name,
                    EnablePaymentsToAccount = a.EnablePaymentsToAccount
                });
            });
        }

        public GetInvoiceBrandingThemesResponse GetInvoiceBrandingThemes()
        {
            return ExecuteWiiseApi<GetInvoiceBrandingThemesResponse>(async (response, svc, token) =>
            {
                var themes = await svc.Api.GetBrandingThemesAsync(token.Value, token.Tenant);
                response.InvoiceBrandingThemes = themes.Data._BrandingThemes.Select(_autoMapperHelper.Mapper.Map<InvoiceBrandingThemeDto>);
            });
        }

        public GetInvoiceBrandingThemeResponse GetInvoiceBrandingTheme(Guid brandingThemeId)
        {
            return ExecuteWiiseApi<GetInvoiceBrandingThemeResponse>(async (response, svc, token) =>
            {
                //var theme = await svc.Api.GetBrandingThemeAsync(token.Value, token.Tenant, brandingThemeId);
                //response.InvoiceBrandingTheme = _autoMapperHelper.Mapper.Map<InvoiceBrandingThemeDto>(theme._BrandingThemes.FirstOrDefault());
            });
        }

        private List<InternalInvoice> GetWiiseInternalPurchaseInvoices(GetInvoicesRequest request, Office[] offices)
        {
            var contacts = new List<Contact>();
            var invoiceNumbers = request.InvoiceNumber?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];

            var filtersDictionary = new Dictionary<string, string>();

            CheckFilter(nameof(request.InvoiceNumber), filtersDictionary, () => invoiceNumbers.Any(),
                () => $"({string.Join(" || ", invoiceNumbers.Select(x => $"InvoiceNumber = \"{x.ToUpper()}\""))})");

            //var response = await _wiiseAccountingApi.GetPurchaseInvoicesAsync(_wiiseToken.Value, _wiiseToken.Tenant, null, null, string.Join(" && ", filtersDictionary.Values));

            var invoicesResponse = WiiseExecutor.Execute((svc, token) =>
                svc.Api.GetPurchaseInvoicesAsync(
                    token.Value,
                    token.Tenant,
                    statuses: null,
                    contactIDs: null,
                    where: string.Join(" && ", filtersDictionary.Values)
                    )
                );

            var invoices = invoicesResponse.Data._Invoices
                .Select(x => new InternalInvoice
                {
                    Id = x.InvoiceID.GetValueOrDefault(),
                    InvoiceNumber = x.InvoiceNumber,
                    Total = Convert.ToDecimal(x.Total.GetValueOrDefault()),
                    AmountDue = Convert.ToDecimal(x.AmountDue),
                    //Payment = Convert.ToDecimal(x.Payments.Sum(y => y.Amount)), //+ x.Overpayments.Sum(y => y.Total ?? 0)),
                    //NaatiNumber = Convert.ToInt32(x.Contact.ContactNumber),
                    //Customer = x.Contact.Name, //.Replace($" - {x.Contact.ContactNumber}", ""),
                    DueDate = x.DueDate.GetValueOrDefault(),
                    Date = x.Date.GetValueOrDefault(),
                    WiiseReference = x.GetWiiseReference(offices),
                    //ThemeId = x.BrandingThemeID,
                    Status = x.Status.ToInternalStatus(),
                    Type = x.InvoiceType == Wiise.PublicModels.InvoiceType.Bill ? NaatiInvoiceType.Bill : NaatiInvoiceType.Invoice,
                    //Payments = GetPayments(request.IncludeFullPaymentInfo, x.Payments, offices, invoice: x),
                    //LineItems = GetLineItems(x.LineItems),
                    TotalTax = Convert.ToDecimal(x.TotalTax)
                }).ToList();

            if (!request.ExcludeCreditNotes)
            {
                if (filtersDictionary.ContainsKey(nameof(request.InvoiceNumber)))
                {
                    filtersDictionary.Remove(nameof(request.InvoiceNumber));
                    CheckFilter(nameof(request.InvoiceNumber), filtersDictionary, () => true,
                        () => $"({string.Join(" || ", invoiceNumbers.Select(x => $"CreditNoteNumber = \"{x.ToUpper()}\""))})");
                }

                if (filtersDictionary.ContainsKey(nameof(request.ExcludePayables)))
                {
                    filtersDictionary.Remove(nameof(request.ExcludePayables));
                    CheckFilter(nameof(request.ExcludePayables), filtersDictionary, () => true, () => "Type=\"ACCRECCREDIT\"");
                }

                CheckFilter(nameof(contacts), filtersDictionary, () => contacts.Any(),
                    () => $"({string.Join(" || ", contacts.Select(x => $"Contact.ContactID = Guid(\"{x.ContactID}\")"))})");

                var credits = WiiseExecutor.Execute((svc, token) => svc.Api.GetCreditNotesAsync(token.Value, token.Tenant, where: string.Join(" && ", filtersDictionary.Values)));
                invoices.AddRange(credits
                    .Data._CreditNotes
                    .Select(x =>
                        new InternalInvoice
                        {
                            Id = x.CreditNoteID.GetValueOrDefault(),
                            InvoiceNumber = x.CreditNoteNumber,
                            Total = Convert.ToDecimal(x.Total.GetValueOrDefault() * -1),
                            AmountDue = 0,
                            NaatiNumber = Convert.ToInt16(x.Contact.ContactNumber),
                            Customer = x.Contact.Name.Replace($" - {x.Contact.ContactNumber}", ""),
                            Date = x.Date,
                            WiiseReference = x.GetWiiseReference(offices),
                            Status = x.Status.ToInternalStatus(),
                            Type = NaatiInvoiceType.CreditNote,
                            PaymentReference = x.Reference
                        }));
            }

            return invoices;
        }

        private List<InternalInvoice> GetWiiseInternalInvoices(GetInvoicesRequest request, Office[] offices)
        {
            try
            {
                var contacts = new List<Contact>();
                var invoiceNumbers = request.InvoiceNumber?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];

                if (request.NaatiNumber != null && request.NaatiNumber.Any())
                {
                    var getContactsResponse = GetContacts(request.NaatiNumber);
                    contacts = getContactsResponse.ToList();

                    if (!contacts.Any())
                    {
                        // if the filter included naati numbers but none of those contacts exist in Wiise,
                        // then we know there are no invoices matching those naati numbers
                        return new List<InternalInvoice>();
                    }
                }

                var status = new[]
                {
                    StatusEnum.PAID.ToString(),
                    StatusEnum.OPEN.ToString()
                }.ToList();

                if (request.IncludeVoidedStatus)
                {
                    status.Add(StatusEnum.CANCELED.ToString());
                }

                var filtersDictionary = new Dictionary<string, string>();

                CheckFilter(nameof(request.DateCreatedFrom), filtersDictionary, () => request.DateCreatedFrom.HasValue, () => $"Date >= {request.DateCreatedFrom?.ToString("yyyy-MM-dd")}");
                CheckFilter(nameof(request.DateCreatedTo), filtersDictionary, () => request.DateCreatedTo.HasValue, () => $"Date <= {request.DateCreatedTo?.ToString("yyyy-MM-dd")}");
                CheckFilter(nameof(request.Office), filtersDictionary,
                     () => request.Office?.Any() ?? false,
                     () => $"({string.Join(" || ", offices.Where(x => request.Office.Contains(x.Id)).Select(x => $"Reference != null && Reference.StartsWith(\"{x.Institution.InstitutionAbberviation}\")"))})");

                CheckFilter(nameof(request.InvoiceNumber), filtersDictionary, () => invoiceNumbers.Any(),
                    () => $"({string.Join(" || ", invoiceNumbers.Select(x => $"InvoiceNumber = \"{x.ToUpper()}\""))})");

                //CheckFilter(nameof(request.ExcludePayables), filtersDictionary, () => request.ExcludePayables, () => "Type=\"ACCREC\"");

                var invoicesResponse = WiiseExecutor.Execute((svc, token) =>
                    svc.Api.GetAllInvoicesAsync(
                        token.Value,
                        token.Tenant,
                        statuses: status,
                        contactIDs: contacts.Any() ? contacts.Where(y => y.ContactID.HasValue).Select(y => y.ContactID.Value).ToList() : null,
                        where: string.Join(" && ", filtersDictionary.Values)
                        )
                    );

            var invoices = invoicesResponse.Data._Invoices
                .Select(x => new InternalInvoice
                {
                    Id = x.InvoiceID.GetValueOrDefault(),
                    InvoiceNumber = x.InvoiceNumber,
                    Total = Convert.ToDecimal(x.Total.GetValueOrDefault()),
                    AmountDue = Convert.ToDecimal(x.AmountDue),
                    Payment = Convert.ToDecimal(x.Payments.Sum(y => y.Amount)),

                    NaatiNumber = Convert.ToInt32(x.Contact.ContactNumber),
                    Customer = x.Contact.Name,
                    DueDate = x.DueDate.GetValueOrDefault(),
                    Date = x.Date,
                    Status = x.Status.ToInternalStatus(),

                    Type = x.InvoiceType == Wiise.PublicModels.InvoiceType.Bill ? NaatiInvoiceType.Bill : NaatiInvoiceType.Invoice,
                    Payments = GetPayments(request.IncludeFullPaymentInfo, x.Payments, offices, invoice: x),
                    LineItems = GetLineItems(x.LineItems),
                    TotalTax = Convert.ToDecimal(x.TotalTax),
                    PaymentReference = x.Reference,
                    WiiseReference = x.GetWiiseReference(offices)
                }).ToList();

                if (!request.ExcludeCreditNotes)
                {
                    if (filtersDictionary.ContainsKey(nameof(request.InvoiceNumber)))
                    {
                        filtersDictionary.Remove(nameof(request.InvoiceNumber));
                        CheckFilter(nameof(request.InvoiceNumber), filtersDictionary, () => true,
                            () => $"({string.Join(" || ", invoiceNumbers.Select(x => $"CreditNoteNumber = \"{x.ToUpper()}\""))})");
                    }

                    if (filtersDictionary.ContainsKey(nameof(request.ExcludePayables)))
                    {
                        filtersDictionary.Remove(nameof(request.ExcludePayables));
                        CheckFilter(nameof(request.ExcludePayables), filtersDictionary, () => true, () => "Type=\"ACCRECCREDIT\"");
                    }

                    CheckFilter(nameof(contacts), filtersDictionary, () => contacts.Any(),
                        () => $"({string.Join(" || ", contacts.Select(x => $"Contact.ContactID = Guid(\"{x.ContactID}\")"))})");

                    var credits = WiiseExecutor.Execute((svc, token) => svc.Api.GetCreditNotesAsync(token.Value, token.Tenant, where: string.Join(" && ", filtersDictionary.Values)));
                    invoices.AddRange(credits
                        .Data._CreditNotes
                        .Select(x =>
                            new InternalInvoice
                            {
                                Id = x.CreditNoteID.GetValueOrDefault(),
                                InvoiceNumber = x.CreditNoteNumber,
                                Total = Convert.ToDecimal(x.Total.GetValueOrDefault() * -1),
                                AmountDue = 0,
                                Payment = Convert.ToDecimal(x.Payments.Sum(y => y.Amount) * -1),
                                NaatiNumber = Convert.ToInt32(x.Contact.ContactNumber),
                                Customer = x.Contact.Name,
                                Date = x.Date,
                                WiiseReference = x.GetWiiseReference(offices),
                                //ThemeId = x.BrandingThemeID,
                                Status = x.Status.ToInternalStatus(),
                                Type = NaatiInvoiceType.CreditNote,
                                Payments = GetPayments(request.IncludeFullPaymentInfo, x.Payments, offices, creditNote: x),
                                PaymentReference = x.Reference
                            }));
                }

                return invoices;
            }
            catch(Exception ex)
            {
                LoggingHelper.LogException(ex);
                throw ex;
            }
        }

        private static void CheckFilter(string filterName, Dictionary<string, string> dictionary, Func<bool> conditionFunction, Func<string> filterFunction)
        {
            if (conditionFunction())
            {
                dictionary[filterName] = filterFunction();
            }
        }

        private List<Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>> GetPayments(bool includeFullPaymentInfo, IEnumerable<Payment> payments, IEnumerable<Office> offices, Invoice invoice = null, CreditNote creditNote = null)
        {
            return includeFullPaymentInfo
                ? new List<Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>>()
                : payments.Select(p =>
                {
                    //p.CreditNote = creditNote;
                    //p.Invoice = invoice;
                    return new Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>
                    (
                        p,
                        p.GetWiiseReference(WiisePaymentAccount, offices), 
                        (Invoice)null,//p.Invoice, 
                        (CreditNote)null//p.CreditNote
                    );
                }).ToList();
        }

        private List<InternalLineItem> GetLineItems(List<LineItem> lineItems)
        {
            var glCodes = NHibernateSession.Current.Query<GLCode>().ToList();

            return lineItems.Select(x=>new InternalLineItem()
            {
                //AccountCode = glCodes.Where(code => code.ExternalReferenceAccountId == x.AccountId).Single().Code,
                AccountId = x.AccountId, 
                Description = x.Description,
                LineAmount = x.LineAmount
            }).ToList();
        }

        private IEnumerable<InvoiceDto> GetWiisePurchaseInvoices(GetInvoicesRequest request)
        {
            var offices = NHibernateSession.Current.Query<Office>().ToArray();
            var eftMachines = NHibernateSession.Current.Query<EFTMachine>().ToArray();

            var invoiceDto = new List<InvoiceDto>();

            try { 


            var invoices = GetWiiseInternalPurchaseInvoices(request, offices);

            invoiceDto = invoices.Select(x => new InvoiceDto
            {
                InvoiceId = x.Id,
                OfficeId = x.WiiseReference?.OfficeId,
                Office = x.WiiseReference?.OfficeName,
                InvoiceNumber = x.InvoiceNumber,
                Total = x.Total,
                AmountDue = x.AmountDue,
                Payment = x.Payment,
                Balance = x.Total - x.Payment,
                NaatiNumber = x.NaatiNumber,
                Customer = x.Customer,
                Type = x.Type,
                Status = (InvoiceStatus)x.Status,
                Date = x.Date,
                DueDate = x.DueDate,
                WiiseReference = x.WiiseReference?.Reference,
                ThemeId = x.ThemeId,
                //Payments = x.Payments.Select(y => MapPayment(y.Item1, y.Item2, x.Type, eftMachines, accounts._Accounts)).ToArray(),
                //LineItems = MapLineItems(x.LineItems, x.NaatiNumber, x.InvoiceNumber),
                TotalTax = x.TotalTax
            }).ToList();
            }
            catch(Exception ex)
            {
                LoggingHelper.LogException(ex);
                throw ex;
            }

            return invoiceDto;
        }

        private IEnumerable<InvoiceDto> GetWiiseInvoices(GetInvoicesRequest request)
        {
            var offices = NHibernateSession.Current.Query<Office>().ToArray();
            var eftMachines = NHibernateSession.Current.Query<EFTMachine>().ToArray();

            var invoices = GetWiiseInternalInvoices(request, offices);

            if (!invoices.Any())
            {
                return Enumerable.Empty<InvoiceDto>();
            }

            if (request.IncludeFullPaymentInfo)
            {
                var invoiceIds = invoices.Select(i => i.Id.ToString()).ToArray();
                //var response = GetInvoicePayments(request.PaidToAccount, invoiceIds);
                var response = invoices.Select(x => x.Payments).ToList();
                var payments = response.ToArray();

                //foreach (var p in payments)
                //{
                //    var id = p.Invoice?.InvoiceID ?? p.CreditNote.CreditNoteID;
                //    var i = invoices.First(y => y.Id == id);
                //    i.Payments.Add(new Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>(p, p.GetWiiseReference(WiisePaymentAccount, offices), p.Invoice, p.CreditNote));
                //}
            }

            if (request.PaymentType != null && request.PaymentType.Any())
            {
                invoices = invoices.Where(x =>
                    x.Payments
                        .Any(y => request.PaymentType.Contains(y.Item2.PaymentType))
                ).ToList();
            }

            if (request.EftMachine != null && request.EftMachine.Any())
            {
                invoices = invoices.Where(x =>
                    x.Payments
                        .Select(y => eftMachines.FirstOrDefault(z => z.TerminalNo.Equals(y.Item2.EftMachine)))
                        .Any(y => y != null && request.EftMachine.Contains(y.Id))
                ).ToList();
            }

            //foreach (var payment in invoices.SelectMany(x => x.Payments))
            //{
                //payment.Item1.Invoice = payment.Item3;
               // payment.Item1.CreditNote = payment.Item4;
            //}

            var accountIds = invoices.SelectMany(x => x.Payments)
                .Select(x => x.Item1.Account)
                .Select(x => x)
                .Distinct()
                .Select(x => $"AccountID = Guid(\"{x}\")");

            //var accounts = WiiseExecutor.Execute((svc, token) => svc.Api.GetAccountsAsync(token.Value, token.Tenant, where: string.Join(" || ", accountIds)));

            var invoiceDto =  invoices.Select(x => new InvoiceDto
            {
                InvoiceId = x.Id,
                OfficeId = x.WiiseReference.OfficeId,
                Office = x.WiiseReference.OfficeName,
                InvoiceNumber = x.InvoiceNumber,
                Total = x.Total,
                AmountDue = x.AmountDue,
                Payment = x.Payment,
                Balance = x.Total - x.Payment,
                NaatiNumber = x.NaatiNumber,
                Customer = x.Customer,
                Type = x.Type,
                Status = (InvoiceStatus)x.Status,
                Date = x.Date,
                DueDate = x.DueDate,
                WiiseReference = x.PaymentReference,
                ThemeId = x.ThemeId,
                //Payments = x.Payments.Select(y => MapPayment(y.Item1, y.Item2, x.Type, eftMachines, accounts._Accounts)).ToArray(),
                LineItems = MapLineItems(x.LineItems,x.NaatiNumber,x.InvoiceNumber),
                TotalTax = x.TotalTax
            });

            return invoiceDto;
        }

        private LineItemDto[] MapLineItems(List<InternalLineItem> internalLineItems,int? naatinumber,string invoiceNumber)
        {
            var returnData = new List<LineItemDto>();
            try
            {
                if (internalLineItems != null)
                {
                    foreach (InternalLineItem item in internalLineItems)
                    {
                        returnData.Add(new LineItemDto()
                        {
                            //AccountCode = item.AccountCode,
                            AccountId = item.AccountId,
                            Description = item.Description,
                            LineAmount = item.LineAmount
                        });
                    }
                }
                else
                {
                    LoggingHelper.LogInfo($"Map Line Items warning. No line items found for Naati Number: {naatinumber} Invoice: {invoiceNumber}");
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError($"Map Line Items error : {ex.ToString()}");
            }
            return returnData.ToArray();
        }

        /// <summary>
        /// Current advice from KPMG is that Wiise cannot retrieve payments
        /// Leaving in because one day it probably will
        /// </summary>
        /// <param name="paidToAccount"></param>
        /// <param name="invoiceIds"></param>
        /// <returns></returns>
        private IEnumerable<Payment> GetInvoicePayments(string[] paidToAccount, string[] invoiceIds)
        {
            var payments = new List<Payment>();

            if (!invoiceIds.Any())
            {
                return payments;
            }

            var filters = invoiceIds.Select(i => $"Invoice.InvoiceID = Guid(\"{i}\")").ToArray();

            var paidFilter = string.Join(" || ", paidToAccount?.Select(i => $"Account.AccountID = Guid(\"{i}\")") ?? new string[0]);

            // Parallelize  call to prevent QueryString overflow
            var maxRequestLength = int.Parse(ConfigurationManager.AppSettings["ParallelizeWiisePaymentsRange"]);
            var maxParallelRequests = int.Parse(ConfigurationManager.AppSettings["WiiseMaxParallelRequests"]);

            var p = Partitioner
                .Create(0, filters.Length, maxRequestLength)
                .GetPartitions(maxParallelRequests)
                .AsParallel()
                .Select(p =>
                {

                    var response = new List<Payment>();

                    while (p.MoveNext())
                    {
                        var ids = new List<string>();

                        for (int i = p.Current.Item1; i < p.Current.Item2; i++)
                        {
                            ids.Add(filters[i]);
                        }

                        var queryString = string.Join(" || ", ids);

                        // placing this inside the lock effectively serialises the parallel oerations. this is a pity but it is done to prevent
                        // "Collection was modified" exceptions which were occuring during the parallel operations, and there isn't
                        // time to find another solution (besides, spreading out the calls a bit reduces the chance of hitting the
                        // API 60 calls/min limit)
                        var query = $"Status != \"DELETED\" && "
                                    + (!string.IsNullOrWhiteSpace(paidFilter)
                                        ? $"({paidFilter}) && "
                                        : string.Empty) + $"({queryString})";

                        var r = WiiseExecutor.Execute((svc, token) => svc.Api.GetPaymentsAsync(token.Value, token.Tenant, where: query));
                        response.AddRange(r.Data._Payments);
                    }

                    return response;
                });

            payments = p.SelectMany(p => p).ToList();

            return payments;
        }

        public GetInvoicesResponse GetPurchaseInvoices(GetInvoicesRequest request)
        {
            var result = GetFinanceServiceResponseData<GetInvoicesRequest, GetInvoicesResponse, InvoiceDto>(request, GetWiisePurchaseInvoices);
            var response = result.Item2;
            response.Invoices = result.Item1;
            return response;
        }

        public GetInvoicesResponse GetInvoices(GetInvoicesRequest request)
        {
            var result = GetFinanceServiceResponseData<GetInvoicesRequest, GetInvoicesResponse, InvoiceDto>(request, GetWiiseInvoices);
            var response = result.Item2;
            response.Invoices = result.Item1;
            return response;
        }

        public GetInvoicesResponse GetUnraisedInvoices(int naatiNumber)
        {
            var unraisedInvoiceApplications = NHibernateSession.Current.Query<CredentialApplication>()
                .Where(x => x.CredentialApplicationStatusType.Id.Equals((int)CredentialApplicationStatusTypeName.AwaitingAssessmentPayment) && 
                x.Person.Entity.NaatiNumber.Equals(naatiNumber)).ToArray();

            var unraisedInvoiceDtos = new List<InvoiceDto>();
            foreach (var unraisedInvoice in unraisedInvoiceApplications)
            {
                unraisedInvoiceDtos.Add(new InvoiceDto()
                {
                    NaatiNumber = naatiNumber,
                    AmountDue = null,
                    Balance = 0,
                    Status = InvoiceStatus.Open,
                    Date = unraisedInvoice.StatusChangeDate,
                    InvoiceNumber = unraisedInvoice.Reference,
                    CredentialApplicationTypeId = unraisedInvoice.CredentialApplicationType.Id,
                    CredentialApplicationId = unraisedInvoice.Id
                });
            }

            return new GetInvoicesResponse()
            {
                Invoices = unraisedInvoiceDtos.ToArray()
            };
        }

        private static WiiseCreateContactRequest GetCreateCustomerRequest(Institution org, Person person, string accountNumber)
        {
            if (org == null && person == null)
            {
                throw new Exception("No entity for CreateCustomer(). NAATI #" + accountNumber);
            }

            var contactPerson = org?.ContactPersons?.FirstOrDefault();
            var contactRequest = new WiiseCreateContactRequest();
            contactRequest.OrgName = org?.CurrentName.Name;
            contactRequest.FirstName = contactPerson?.Name ?? person?.GivenName;
            contactRequest.LastName = contactPerson != null ? string.Empty : person?.Surname.Replace(EmptyLastName, string.Empty, StringComparison.InvariantCultureIgnoreCase);
            contactRequest.Email = org?.Entity.PrimaryEmail?.EmailAddress ?? person?.PrimaryEmailAddress;

            var primaryAddress = org?.Entity.PrimaryAddress ?? person?.PrimaryAddress;
            if (primaryAddress != null)
            {
                contactRequest.Street = primaryAddress.StreetDetails;
                var postcode = primaryAddress.Postcode;
                if (postcode != null)
                {
                    contactRequest.City = postcode.Suburb.Name;
                    contactRequest.State = postcode.Suburb.State.Abbreviation;
                    contactRequest.Postcode = postcode.PostCode;
                }
                contactRequest.Country = primaryAddress.Country.Name;
                contactRequest.CountryCode = primaryAddress.Country.Code;
            }

            contactRequest.AccountNumber = accountNumber;
            contactRequest.Abn = org?.Entity.Abn ?? person?.Entity.Abn;

            return contactRequest;
        }

        private static NaatiEntity GetEntityById(int entityId, out Person person, out Institution institution)
        {
            var entity = NHibernateSession.Current.Get<NaatiEntity>(entityId);
            if (entity == null)
            {
                throw new Exception($"Entity with Id {entityId} does not exist.");
            }

            return GetTypedEntity(entity, out person, out institution);
        }

        private static NaatiEntity GetEntityByNaatiNumber(int naatiNumber, out Person person, out Institution institution)
        {
            var entity = NHibernateSession.Current.Query<NaatiEntity>().SingleOrDefault(x => x.NaatiNumber == naatiNumber);
            if (entity == null)
            {
                throw new Exception($"Entity with NAATI #{naatiNumber} does not exist.");
            }

            return GetTypedEntity(entity, out person, out institution);
        }

        private static NaatiEntity GetTypedEntity(NaatiEntity entity, out Person person, out Institution institution)
        {
            if (entity.EntityTypeId == (int)NaatiNumberTypeEnum.Institution)
            {
                institution = NHibernateSession.Current.Query<Institution>().Single(x => x.Entity.Id == entity.Id);
                person = null;
            }
            else
            {
                person = NHibernateSession.Current.Query<Person>().Single(x => x.Entity.Id == entity.Id);
                institution = null;
            }

            return entity;
        }

        private static WiiseCreateCreditNoteRequest GetWiiseCreateCreditNoteRequest(CreateApplicationRefundRequest request, CredentialApplicationRefund refund)
        {
            var user = refund.User;
            Institution org;
            Person person;
            Guid brandingThemeId;


            brandingThemeId = request.BrandingThemeId.GetValueOrDefault();


            var entity = request.EntityId != 0
                ? GetEntityById(request.EntityId, out person, out org)
                : GetEntityByNaatiNumber(request.NaatiNumber, out person, out org);

            IList<int> candidateNaatiNumbers;
            var lineItems = GetLineItems(request, person, false, out candidateNaatiNumbers);

            return new WiiseCreateCreditNoteRequest
            {
                CreditNoteNumber = refund.CreditNoteNumber,
                Reference = request.Reference ?? new WiiseInvoiceReference(user.Office).Reference,
                Contact = GetCreateCustomerRequest(org, person, entity.NaatiNumber.ToString()),
                LineItems = lineItems,
                BrandingThemeId = brandingThemeId,
                DueDate = request.DueDate,
                CandidatesNaatiNumber = candidateNaatiNumbers,
                InvoiceType = request.InvoiceType
            };
        }

        private static WiiseCreateInvoiceRequest GetWiiseCreateInvoiceRequest(CreateInvoiceRequest request, User user)
        {
            Institution org;
            Person person;

            var entity = request.EntityId != 0
                ? GetEntityById(request.EntityId, out person, out org)
                : GetEntityByNaatiNumber(request.NaatiNumber, out person, out org);

            IList<int> candidateNaatiNumbers;
            var lineIems = GetLineItems(request, person, request.InvoiceType == NaatiInvoiceType.Bill, out candidateNaatiNumbers);

            //get payment method id. 
            var paymentMethod = request.Payments.SingleOrDefault()?.PaymentType;
            //Assume Direct Debit unless payments exist
            PaymentMethodType paymentMethodType = null;
            if (paymentMethod == null)
            {
                paymentMethodType = NHibernateSession.Current.Query<PaymentMethodType>().SingleOrDefault(x => x.Id == 6); //directdebit
            }
            else
            {
                paymentMethodType = NHibernateSession.Current.Query<PaymentMethodType>().SingleOrDefault(x => x.Id == (int)paymentMethod);
            }

            return new WiiseCreateInvoiceRequest
            {
                InvoiceNumber = request.InvoiceNumber,
                InvoiceType = request.InvoiceType,
                Reference = request.Reference ?? new WiiseInvoiceReference(user.Office).Reference,
                Contact = request.InvoiceType != NaatiInvoiceType.Bill
                    ? GetCreateCustomerRequest(org, person, entity.NaatiNumber.ToString())
                    : new WiiseCreateContactRequest
                    {
                        AccountNumber = entity.AccountNumber
                    },
                LineItems = lineIems,
                DueDate = request.DueDate,
                CandidatesNaatiNumber = candidateNaatiNumbers,
                PaymentMethodId = paymentMethodType?.ExternalReferenceId
            };
        }

        private static IEnumerable<WiiseLineItemModel> GetLineItems(CreatPayableRequest request,
            Person person, bool trackingCategory, out IList<int> candidateNaatiNumbers)
        {
            int credentialApplicationId = 0;
            //below is a hack -- can be made better for a long terrm solution
            if (!string.IsNullOrWhiteSpace(request.Reference))
            {
                // Extracting the numeric id after “APP” for building the query later. 
                var m = System.Text.RegularExpressions.Regex.Match(request.Reference, @"APP(\d+)");
                if (m.Success)
                {
                    credentialApplicationId = int.Parse(m.Groups[1].Value);
                }
            }
            bool isNaatiFunded = credentialApplicationId != 0 && NHibernateSession.Current.Query<CredentialApplication>().Any(ca => ca.Id == credentialApplicationId && ca.SponsorInstitution != null && ca.SponsorInstitution.InstitutionName == "Scholarship - NAATI funded");
            bool offsetAlreadyInRequest = request.LineItems.Any(li => li.ProductSpecificationId == 134); // for avoiding duplicacy of scholarship offsets 
            var description = string.Empty;
            var candidateIds = new HashSet<int>(); // for avoiding duplicacy of scholarship offsets 
            var productSpecificationIds = request.LineItems.Select(x => x.ProductSpecificationId).ToArray();
            var productSpecifications =
                NHibernateSession.Current.Query<ProductSpecification>()
                    .Where(x => productSpecificationIds.Contains(x.Id))
                    .ToArray();
            var lineItems = request.LineItems.Select(x =>
            {
               var glCode = new GLCode();
               var spec = productSpecifications.FirstOrDefault(y => y.Id == x.ProductSpecificationId);
               if (spec == null)
               {
                   //may be a paypal surcharge - ProductSpecificationId will be set to 0
                   if (x.GlCode != null)
                   {
                       var paypalSetting = NHibernateSession.Current.Query<SystemValue>().Where(x => x.ValueKey.Equals("PayPalGlCode")).First().Value;
                       var payPalAccount = NHibernateSession.Current.Query<GLCode>().Where(x => x.Code.Equals(paypalSetting)).First().ExternalReferenceAccountId;
                       glCode.Code = x.GlCode;
                       glCode.ExternalReferenceAccountId = payPalAccount;
                       description = x.Description;
                   }
                   else
                   {
                       throw new WebServiceException("Invalid product specification ID: " + x.ProductSpecificationId);
                   }
               }
               else
               {
                   glCode = spec.GLCode;
                   description = string.IsNullOrEmpty(x.Description) ? spec.Description : x.Description;
               }
               Person itemPerson;
               // if someone other than the candidate is being invoiced, add candidate detail to the line item (UC-SAM-2002 BR7).
               // note that either entity or naati number can be specified as the invoicee
               if (x.EntityId != request.EntityId || request.NaatiNumber != 0 && x.NaatiNumber != request.NaatiNumber)
               {
                   Institution itemOrg;
                   var itemNaatiNumber = x.EntityId != 0
                       ? GetEntityById(x.EntityId, out itemPerson, out itemOrg).NaatiNumber
                       : GetEntityByNaatiNumber(x.NaatiNumber, out itemPerson, out itemOrg).NaatiNumber;

                   candidateIds.Add(itemNaatiNumber);
               }
               else
               {
                   itemPerson = person;
                   candidateIds.Add(x.NaatiNumber);
               }

               return new WiiseLineItemModel
               {
                   //AccountCode = glCode.Code,
                   Description = description,
                   Quantity = x.Quantity,
                   UnitAmount = x.IncGstCostPerUnit,
                   Gst = x.GstApplies,
                   TrackingCategories = trackingCategory
                       ? GetTracking(itemPerson, spec)
                       : null,
                   AccountId = glCode.ExternalReferenceAccountId
               };
           }).ToList();

            if (isNaatiFunded && !offsetAlreadyInRequest)
            {
                var feeItem = lineItems.First(li => li.UnitAmount > 0);
                var offsetSpec = NHibernateSession.Current.Query<ProductSpecification>().First(ps => ps.Code == "SCHOLARSHIPNAATIFUND");
                lineItems.Add(new WiiseLineItemModel
                {
                    Description = offsetSpec.Description,
                    Quantity = 1,
                    UnitAmount = -feeItem.UnitAmount,
                    Gst = false,
                    TrackingCategories = feeItem.TrackingCategories,
                    AccountId = offsetSpec.GLCode.ExternalReferenceAccountId
                });
            }
            candidateNaatiNumbers = candidateIds.ToList();
            return lineItems;
        }

        private static List<System.Tuple<string, string>> GetTracking(Person person, ProductSpecification productSpecification)
        {
            if (string.IsNullOrEmpty(person?.ExaminerTrackingCategory))
            {
                throw new WebServiceException("The examiner must have a Supplier Tracking Category in order to generate a bill. This can be set in the Person Details screen.");
            }

            if (string.IsNullOrEmpty(productSpecification?.TrackingActivity))
            {
                throw new WebServiceException("Product Tracking Activity is required to generate a bill");
            }

            return new List<System.Tuple<string, string>>
            {
                new System.Tuple<string, string>
                (
                    "Category",
                    person.ExaminerTrackingCategory
                ),
                new System.Tuple<string, string>
                (
                    "Activity",
                    productSpecification.TrackingActivity
                )
            };
        }

        public CreateInvoiceResponse CreateInvoice(CreateInvoiceRequest request)
        {
            return CreateInvoice(request, new ManualCreateInvoiceCompletionOperation());
        }

        private void RemoveWorkflowFee(int feeId)
        {
            var fee = NHibernateSession.Current.Load<CredentialWorkflowFee>(feeId);

            NHibernateSession.Current.Delete(fee);
            NHibernateSession.Current.Flush();
        }

        private void ResetCredentialApplicationRefund(int credentialApplicationRefundId)
        {
            var refund = NHibernateSession.Current.Load<CredentialApplicationRefund>(credentialApplicationRefundId);

            refund.CreditNoteNumber = null;
            refund.CreditNoteId = null;
            refund.PaymentReference = null;
            refund.RefundedDate = null;
            refund.CreditNoteProcessedDate = null;
            refund.CreditNotePaymentProcessedDate = null;

            NHibernateSession.Current.Save(refund);
            NHibernateSession.Current.Flush();
        }

        private CredentialWorkflowFee CreateWorkflowFee(CreateApplicationInvoiceRequest request)
        {
            var credentialApplication = NHibernateSession.Current.Get<CredentialApplication>(request.CredentialApplicationId);
            var credentialRequest = NHibernateSession.Current.Get<CredentialRequest>(request.CredentialRequestId); 
            var credentialFeeProduct = NHibernateSession.Current.Get<CredentialFeeProduct>(request.CredentialFeeProductId);

            var invoiceAction = request.InvoiceCompletionAction.HasValue
                ? NHibernateSession.Current.Get<SystemActionType>((int)request.InvoiceCompletionAction)
                : null;
            var paymentAction = request.PaymentCompletionAction.HasValue
                ? NHibernateSession.Current.Get<SystemActionType>((int)request.PaymentCompletionAction)
                : null;
            var productSpec = NHibernateSession.Current.Get<ProductSpecification>(request.ProductSpecificationId);


            credentialApplication.NotNull($"CredentialApplication not found. ID: {request.CredentialApplicationId}");
            productSpec.NotNull($"ProductSpecification not found. ID: {request.ProductSpecificationId}");
            credentialFeeProduct.NotNull($"CredentilaFeeProduct not found. ID: {request.CredentialFeeProductId}");

            var credentialApplicationRefundPolicy = NHibernateSession.Current.Get<CredentialApplicationRefundPolicy>(credentialFeeProduct.CredentialApplicationRefundPolicy.Id);

            string orderNumber = null;
            string transactionId = null;
            if (request.Payments != null && request.Payments.Any())
            {
                orderNumber = string.Join(",", request.Payments?.Select(payment => payment.OrderNumber));
                transactionId = string.Join(",", request.Payments?.Select(payment => payment.Reference));
            }
            var fee = new CredentialWorkflowFee
            {
                CredentialApplication = credentialApplication,
                CredentialRequest = credentialRequest,
                OnInvoiceCreatedSystemActionType = invoiceAction,
                OnPaymentCreatedSystemActionType = paymentAction,
                ProductSpecification = productSpec,
                OrderNumber = orderNumber,
                TransactionId = transactionId,
                CredentialApplicationRefundPolicy = credentialApplicationRefundPolicy
            };

            NHibernateSession.Current.Save(fee);
            NHibernateSession.Current.Flush();

            return fee;
        }

        public CreateInvoiceResponse CreateApplicationInvoice(CreateApplicationInvoiceRequest request)
        {
            // todo: this is really a busines layer responsibility, probably should move it there
            var fee = CreateWorkflowFee(request);
            var response = new CreateInvoiceResponse();

            try
            {
                var invoiceCompletion = new CreateApplicationInvoiceCompletionOperation
                {
                    CredentialWorkflowFeeId = fee.Id
                };

                var paymentCompletion = new CreateApplicationPaymentCompletionOperation
                {
                    CredentialWorkflowFeeId = fee.Id
                };

                response = CreateInvoice(request, invoiceCompletion, paymentCompletion);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.Error = true;
            }
            finally
            {
                if (response.Error && request.CancelOperationIfError)
                {
                    RemoveWorkflowFee(fee.Id);
                }
            }

            return response;
        }

        public CreateCreditNoteResponse CreateApplicationRefund(CreateApplicationRefundRequest request)
        {
            var response = new CreateCreditNoteResponse();

            try
            {
                var refundCompletion = new CreateApplicationRefundCompletionOperation
                {
                    CredentialApplicationRefundId = request.CredentialApplicationRefundId
                };

                var paymentCompletion = new CreateApplicationRefundPaymentCompletionOperation
                {
                    CredentialApplicationRefundId = request.CredentialApplicationRefundId
                };

                response = CreateApplicationRefund(request, refundCompletion, paymentCompletion);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.Error = true;
            }
            finally
            {
                if (response.Error && request.CancelOperationIfError)
                {
                    ResetCredentialApplicationRefund(request.CredentialApplicationRefundId);
                }
            }

            return response;
        }

        private CreateCreditNoteResponse CreateApplicationRefund(CreateApplicationRefundRequest request, WiiseCompletionOperation completionOperation, CreatePaymentCompletionOperation createPaymentCompletionOperation = null, User user = null)
        {
            var refund = NHibernateSession.Current.Get<CredentialApplicationRefund>(request.CredentialApplicationRefundId);
            user = user ?? refund.User;

            var creditNoteCreateCompletionAction = request.CreditNoteCreateCompletionAction.HasValue
               ? NHibernateSession.Current.Get<SystemActionType>((int)request.CreditNoteCreateCompletionAction)
               : null;
            var creditNotePaymentCompletionAction = request.CreditNotePaymentCompletionAction.HasValue
                ? NHibernateSession.Current.Get<SystemActionType>((int)request.CreditNotePaymentCompletionAction)
                : null;

            refund.OnCreditNoteCreatedSystemActionType = creditNoteCreateCompletionAction;
            refund.OnPaymentCreatedSystemActionType = creditNotePaymentCompletionAction;

            NHibernateSession.Current.Save(refund);
            NHibernateSession.Current.Flush();

            CreatePaymentResponse paymentResponse = null;
            ExternalAccountingOperationResult<CreditNote> invoiceResult = null;

            var creditNoteRequest = GetWiiseCreateCreditNoteRequest(request, refund);
            try
            {
                invoiceResult = AccountingProcessing.CreateCreditNote(creditNoteRequest, completionOperation, request.Description, user.Id, request.BatchProcess);
            }
            catch (WebServiceException ex)
            {
                return new CreateCreditNoteResponse
                {
                    Error = true,
                    ErrorMessage = ex.Message
                };
            }

            var invoiceResponse = GetCreateCreditNoteResponse(invoiceResult, request.BatchProcess);

            if (invoiceResponse.Error)
            {
                invoiceResponse.StackTrace = invoiceResponse.StackTrace;
            }

            if (request.Payments != null && request.Payments.Any())
            {
                paymentResponse = AddPayments(user.Office, user.Id, request.Payments, null, invoiceResponse.Number, invoiceResponse.OperationId, request.BatchProcess, invoiceResponse.Error, createPaymentCompletionOperation);
                invoiceResponse.PaymentErrorMessage = paymentResponse.ErrorMessage;
                invoiceResponse.PaymentWarningMessage = paymentResponse.WarningMessage;

                if (paymentResponse.Error)
                {
                    invoiceResponse.StackTrace = paymentResponse.StackTrace;
                }
            }

            invoiceResponse.Error = invoiceResponse.Error || paymentResponse != null && paymentResponse.Error;

            if (invoiceResponse.Error && request.CancelOperationIfError && invoiceResponse.OperationId != null)
            {
                CancelOperation(invoiceResult.OperationId.GetValueOrDefault());
            }

            return invoiceResponse;
        }

        private CreateInvoiceResponse CreateInvoice(CreateInvoiceRequest request, WiiseCompletionOperation completionOperation, CreatePaymentCompletionOperation createPaymentCompletionOperation = null, User user = null)
        {
            user = user ?? NHibernateSession.Current.Get<User>(request.UserId);
            CreatePaymentResponse paymentResponse = null;
            ExternalAccountingOperationResult<Wiise.PublicModels.Invoice> invoiceResult = null;

            var invoiceRequest = GetWiiseCreateInvoiceRequest(request, user);
            try
            {
                invoiceResult = AccountingProcessing.CreateInvoice(invoiceRequest, completionOperation, request.Description, request.UserId, request.BatchProcess);
            }
            catch (WebServiceException ex)
            {
                return new CreateInvoiceResponse
                {
                    Error = true,
                    ErrorMessage = ex.Message
                };
            }

            var invoiceResponse = GetCreateInvoiceResponse(invoiceResult, request.BatchProcess);

            if (invoiceResponse.Error)
            {
                invoiceResponse.StackTrace = invoiceResponse.StackTrace;
            }
            //only add to the queue if amount > 0!
            var realPayments = request.Payments?.Where(p => p.Amount > 0).ToList();
            if (realPayments != null && realPayments.Any())
            {
                paymentResponse = AddPayments(user.Office, request.UserId, realPayments, invoiceResponse.Number, null, invoiceResponse.OperationId, request.BatchProcess, invoiceResponse.Error, createPaymentCompletionOperation);
                invoiceResponse.PaymentErrorMessage = paymentResponse.ErrorMessage;
                invoiceResponse.PaymentWarningMessage = paymentResponse.WarningMessage;

                if (paymentResponse.Error)
                {
                    invoiceResponse.StackTrace = paymentResponse.StackTrace;
                }
            }

            invoiceResponse.Error = invoiceResponse.Error || paymentResponse != null && paymentResponse.Error;

            if (invoiceResponse.Error && request.CancelOperationIfError && invoiceResponse.OperationId != null)
            {
                CancelOperation(invoiceResult.OperationId.GetValueOrDefault());
            }

            return invoiceResponse;
        }

        private static CreateInvoiceResponse GetCreateInvoiceResponse(ExternalAccountingOperationResult<Invoice> operationResult, bool batchProcess)
        {
            var response = new CreateInvoiceResponse();

            if (operationResult.Success && !batchProcess && operationResult.Result == null)
            {
                throw new WebServiceException("Invoice creation may have failed but no error was returned.");
            }

            response.Error = !operationResult.Success;
            response.ErrorMessage = operationResult.ErrorMessage;
            response.WarningMessage = operationResult.WarningMessage;
            response.OperationId = operationResult.OperationId;
            response.Number = operationResult.Result?.InvoiceNumber;
            response.Id = operationResult.Result?.InvoiceID ?? default(Guid);
            response.Reference = operationResult.Result?.Reference;
            response.StackTrace = operationResult.Exception?.StackTrace;

            return response;
        }

        private static CreateCreditNoteResponse GetCreateCreditNoteResponse(ExternalAccountingOperationResult<CreditNote> operationResult, bool batchProcess)
        {
            var response = new CreateCreditNoteResponse();

            if (operationResult.Success && !batchProcess && operationResult.Result == null)
            {
                throw new WebServiceException("Invoice creation may have failed but no error was returned.");
            }

            response.Error = !operationResult.Success;
            response.ErrorMessage = operationResult.ErrorMessage;
            response.WarningMessage = operationResult.WarningMessage;
            response.OperationId = operationResult.OperationId;
            response.Number = operationResult.Result?.CreditNoteNumber;
            response.Id = operationResult.Result?.CreditNoteID ?? default(Guid);
            response.Reference = operationResult.Result?.Reference;
            response.StackTrace = operationResult.Exception?.StackTrace;

            return response;
        }

        private GetInvoicePdfResponse GetInvoicePdf(string invoiceNumber)
        {
            var response = new GetInvoicePdfResponse();
            WiiseExecutor.Execute(async (svc, token) =>
            {
                var invoicesResponse = await svc.Api.GetInvoicesAsync(token.Value, token.Tenant, where: $"InvoiceNumber=\"{invoiceNumber}\"");
                var invoices = invoicesResponse.Data._Invoices;

                if (!invoices.Any())
                {
                    response.Error = true;
                    response.ErrorMessage = $"Invoice {invoiceNumber} does not exist in Wiise.";
                }
                else
                {
                    var stream = await svc.Api.GetInvoiceAsPdfAsync(token.Value, token.Tenant, invoices.First().InvoiceID.GetValueOrDefault());
                    response.FileContent = StreamToBytes(stream.Data);
                }
            });
            return response;
        }

        private GetInvoicePdfResponse GetInvoicePdfById(Guid invoiceId)
        {
            var response = new GetInvoicePdfResponse();
            var stream = WiiseExecutor.Execute((svc, token) => svc.Api.GetInvoiceAsPdfAsync(token.Value, token.Tenant, invoiceId));
            response.FileContent = StreamToBytes(stream.Data);
            return response;
        }

        private byte[] StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private GetInvoicePdfResponse GetCreditNotePdf(string invoiceNumber)
        {
            var response = new GetInvoicePdfResponse();
            WiiseExecutor.Execute(async (svc, token) =>
            {
                var invoicesResponse = await svc.Api.GetCreditNotesAsync(token.Value, token.Tenant, where: $"CreditNoteNumber=\"{invoiceNumber}\"");
                var invoices = invoicesResponse.Data._CreditNotes;

                if (!invoices.Any())
                {
                    response.Error = true;
                    response.ErrorMessage = $"Credit note {invoiceNumber} does not exist in Wiise.";
                }
                else
                {
                    var stream = await svc.Api.GetCreditNoteAsPdfAsync(token.Value, token.Tenant, invoices.First().CreditNoteID.GetValueOrDefault());
                    response.FileContent = StreamToBytes(stream.Data);
                }
            });

            return response;
        }

        private GetInvoicePdfResponse GetCreditNotePdfById(Guid invoiceId)
        {
            var response = new GetInvoicePdfResponse();
            var stream = WiiseExecutor.Execute((svc, token) => svc.Api.GetCreditNoteAsPdfAsync(token.Value, token.Tenant, invoiceId));
            response.FileContent = StreamToBytes(stream.Data);
            return response;
        }

        public GetInvoicePdfResponse GetInvoicePdf(GetInvoicePdfRequest request)
        {
            var response = new GetInvoicePdfResponse();
            try
            {
                response = request.Type == NaatiInvoiceType.CreditNote
                    ? GetCreditNotePdf(request.InvoiceNumber)
                    : GetInvoicePdf(request.InvoiceNumber);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }

        public GetInvoicePdfResponse GetInvoicePdfById(GetInvoicePdfByIdRequest request)
        {
            var response = new GetInvoicePdfResponse();
            try
            {
                response = request.Type == NaatiInvoiceType.CreditNote
                    ? GetCreditNotePdfById(request.InvoiceId)
                    : GetInvoicePdfById(request.InvoiceId);
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.ErrorMessage = ex.ToString();
            }
            return response;
        }


        public CreateInvoiceResponse CreateInvoiceFromQueue(SubmitQueuedOperationRequest request)
        {
            var operationResult = AccountingProcessing.CreateInvoice(request.OperationId, request.UserId, false);
            return GetCreateInvoiceResponse(operationResult, true);
        }

        private static CreatePaymentResponse GetCreatePaymentResponse(ExternalAccountingOperationResult<Payments> operationResult)
        {
            return new CreatePaymentResponse
            {
                Error = !operationResult.Success,
                ErrorMessage = operationResult.ErrorMessage,
                WarningMessage = operationResult.WarningMessage,
                OperationId = operationResult.OperationId,
                PaymentId = operationResult.Result?._Payments.FirstOrDefault()?.Date.ToString(),
                Reference = operationResult.Result?._Payments.FirstOrDefault()?.Reference,
                StackTrace = operationResult.Exception?.StackTrace
            };
        }

        private static CreatePaymentResponse GetCreatePaymentResponse(ExternalAccountingOperationResult<IEnumerable<Payment>> operationResult)
        {
            return new CreatePaymentResponse
            {
                Error = !operationResult.Success,
                ErrorMessage = operationResult.ErrorMessage,
                WarningMessage = operationResult.WarningMessage,
                OperationId = operationResult.OperationId,
                PaymentId = operationResult.Result?.FirstOrDefault()?.Date.ToString(),
                Reference = operationResult.Result?.FirstOrDefault()?.Reference,
                StackTrace = operationResult.Exception?.StackTrace
            };
        }

        public CreatePaymentResponse CreatePayment(CreatePaymentRequest request)
        {
            var paymentRequest = _autoMapperHelper.Mapper.Map<WiiseCreatePaymentRequest>(request);
            var operationResult = AccountingProcessing.CreatePayment(paymentRequest, null, request.UserId, request.BatchProcess, false);
            return GetCreatePaymentResponse(operationResult);
        }

        public CreatePaymentResponse CreatePaymentFromQueue(SubmitQueuedOperationRequest request)
        {
            var operationResult = AccountingProcessing.CreatePayment(request.OperationId, request.UserId, false);
            return GetCreatePaymentResponse(operationResult);
        }

        private IEnumerable<PaymentDto> GetWiisePayments(GetPaymentsRequest request)
        {
            var offices = NHibernateSession.Current.Query<Office>().ToArray();
            var eftMachines = NHibernateSession.Current.Query<EFTMachine>().ToArray();
            var wiisePaymentAccount = WiisePaymentAccount;

            var filterStrings = new List<string>
            {
                "Status != \"DELETED\""
            };

            if (request.NaatiNumber != null && request.NaatiNumber.Any())
            {
                var invoices = GetWiiseInternalInvoices(new GetInvoicesRequest
                {
                    NaatiNumber = request.NaatiNumber
                }, offices);

                var invoiceIds = invoices.Select(x => x.Id);
                filterStrings.Add($"({string.Join(" || ", invoiceIds.Select(x => $"Invoice.InvoiceID = \"{x}\""))})");
            }

            if (request.DateCreatedFrom.HasValue)
            {
                filterStrings.Add($"Date >= {request.DateCreatedFrom?.ToString()}");
            }

            if (request.DateCreatedTo.HasValue)
            {
                filterStrings.Add($"Date <= {request.DateCreatedTo?.ToString()}");
            }

            var filteredOffices = offices.Where(x => (request.Office?.Contains(x.Id)).GetValueOrDefault()).ToArray();

            if (filteredOffices.Any())
            {
                filterStrings.Add($"({string.Join(" || ", filteredOffices.Select(x => $"Reference != null && Reference.StartsWith(\"{x.Institution.InstitutionAbberviation}\")"))})");
            }

            if (request.EftMachine != null && request.EftMachine.Any())
            {
                var eftMachineTerminals = request.EftMachine.Select(x => eftMachines.FirstOrDefault(y => y.Id == x)?.TerminalNo).Where(x => !string.IsNullOrEmpty(x));
                filterStrings.Add($"({string.Join(" || ", eftMachineTerminals.Select(x => $"Reference != null && Reference.Contains(\"{x}\")"))})");
            }

            var invoiceNumbers = request.InvoiceNumber?.Where(x => !string.IsNullOrEmpty(x)).ToArray() ?? new string[0];

            if (invoiceNumbers.Any())
            {
                filterStrings.Add($"({string.Join(" || ", invoiceNumbers.Select(x => $"Invoice.InvoiceNumber = \"{x.ToUpper()}\""))})");
            }

            if (request.PaidToAccount != null && request.PaidToAccount.Any())
            {
                filterStrings.Add($"({string.Join(" || ", request.PaidToAccount.Select(x => $"Account.AccountID = Guid(\"{x}\")"))})");
            }


            return WiiseExecutor.Execute(async (svc, token) =>
            {
                var paymentsResponse = await svc.Api.GetPaymentsAsync(token.Value, token.Tenant, where: string.Join(" && ", filterStrings));
                var payments = paymentsResponse.Data._Payments
                    
                    .Select(x => new Tuple<Payment, WiisePaymentReference, Invoice, CreditNote>
                    (
                        x,
                        x.GetWiiseReference(wiisePaymentAccount, offices),
                        null,
                        null
                        //x.Invoice, 
                        //x.CreditNote
                    ))
                    .ToArray();

                if (request.PaymentType != null && request.PaymentType.Any())
                {
                    payments = payments.Where(x => request.PaymentType.Contains(x.Item2.PaymentType)|| (request.PaymentType.Contains("PayPal") && x.Item2.Reference.Contains(PayPalService.PAYPALORDERPREFIX))).ToArray();
                }

                var accountIds = payments
                    .Select(x => x.Item1.Account)
                    .Distinct()
                    .Select(x => $"AccountID = Guid(\"{x}\")");

                var accounts = await svc.Api.GetAccountsAsync(token.Value, token.Tenant, where: string.Join(" || ", accountIds));
                return payments.Select(x => MapPayment(x.Item1, x.Item2, x.Item3 != null && x.Item3.Type == Invoice.TypeEnum.ACCREC ? NaatiInvoiceType.Invoice : NaatiInvoiceType.CreditNote, eftMachines, null)).ToArray();
            });
        }

        public GetPaymentsResponse GetPayments(GetPaymentsRequest request)
        {
            var result = GetFinanceServiceResponseData<GetPaymentsRequest, GetPaymentsResponse, PaymentDto>(request, GetWiisePayments);
            var response = result.Item2;
            response.Payments = result.Item1;
            response.Payments.ForEach(x => x=ChangeForPayPal(x));
            return response;
        }

        private PaymentDto ChangeForPayPal(PaymentDto x)
        {
            if(x.Reference != null && x.Reference.Contains(PayPalService.PAYPALORDERPREFIX))
            {
                x.PaymentType = "PayPal";
                x.Office = "Online";
            }
            return x;
        }

        private Tuple<TDto[], TResponse> GetFinanceServiceResponseData<TRequest, TResponse, TDto>(TRequest request, Func<TRequest, IEnumerable<TDto>> getWiiseDataFunc)
            where TRequest : EndOfPeriodRequest
            where TResponse : FinanceServiceResponse, new()
        {
            var dtoList = new List<TDto>();
            var response = ExecuteWiiseApi<TResponse>(async (x, svc, token) =>
            {
                var resp = getWiiseDataFunc(request);
                dtoList.AddRange(resp);
            });

            response ??= new TResponse();

            bool searchLimited;
            int originalCount;
            var data = dtoList.LimitSearchResults(out searchLimited, out originalCount).ToArray();

            if (searchLimited)
            {
                response.WarningMessage = $"Search results were limited to {data.Length} of {originalCount} records.";
            }

            var result = new Tuple<TDto[], TResponse>(data, response);
            return result;
        }


        private IEnumerable<Contact> GetContacts(IEnumerable<int> naatiNumber)
        {
            var where = string.Join(" || ", naatiNumber.Select(y => $"AccountNumber = \"{y}\""));
            var data = WiiseExecutor.Execute((svc, token) => svc.Api.GetAllContactsAsync(token.Value, token.Tenant, where: where));
            return data.Data._Contacts;
        }

        private void ExecuteWiiseApi<T>(T response, Func<T, IWiiseIntegrationService, WiiseToken, Task> action) where T : FinanceServiceResponse
        {
            try
            {
                WiiseExecutor.Execute((svc, token) => action(response, svc, token));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private T ExecuteWiiseApi<T>(Func<T, IWiiseIntegrationService, WiiseToken, Task> action) where T : FinanceServiceResponse, new()
        {
            var response = new T();
            ExecuteWiiseApi(response, action);
            return response;
        }

        private static PaymentDto MapPayment(Payment payment, WiisePaymentReference reference, NaatiInvoiceType invoiceType, IEnumerable<EFTMachine> eftMachines, IEnumerable<Account> accounts)
        {
            var eftMachine = eftMachines.FirstOrDefault(x => x.TerminalNo.Equals(reference.EftMachine));
            var account = accounts.FirstOrDefault(x => x.Id == (payment.Account));
            //var contact = payment.GetContact();

            return new PaymentDto
            {
                //InvoiceNumber = payment.Invoice != null
                //    ? payment.Invoice.InvoiceNumber
                //    : payment.CreditNote.CreditNoteNumber,
                InvoiceType = invoiceType,
                DatePaid = payment.Date.GetValueOrDefault(),
                Amount = Convert.ToDecimal(payment.Amount),
                Reference = payment.Reference,
                PaymentType = payment.Reference != null && payment.Reference.StartsWith("PAYID") ? PaymentTypeDto.PayPal.ToString() : reference.PaymentType,
                BSB = reference.Bsb,
                ChequeNumber = reference.ChequeNumber,
                BankName = reference.BankName,
                EftMachineId = eftMachine?.Id,
                EftMachine = reference.EftMachine,
                OfficeId = reference.OfficeId,
                Office = payment.Reference != null && payment.Reference.StartsWith("PAYID") ? "Online" : reference.OfficeName,
                NaatiNumber = Convert.ToInt16(payment.Account),
                //Customer = contact?.Name.Replace($" - {contact.ContactNumber}", ""),
                PaymentAccount = account?.Name
            };
            
        }

        public FinanceServiceResponse CancelOperation(int operationId)
        {
            var response = new FinanceServiceResponse();
            try
            {
                WiiseQueue.CancelOperation(operationId);
            }
            catch (WebServiceException ex) // todo we really need a FriendlyWebServiceException; there is code in SAM that assumes all WebServiceExceptions are friendly, but they aren't
            {
                response.Error = true;
                // assume friendly
                response.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                response.Error = true;
                // assume non-friendly
                response.StackTrace = ex.StackTrace;
            }

            return response;
        }

        public GetQueuedOperationsResponse GetQueuedOperations(GetQueuedOperationsRequest request)
        {
            var operations = WiiseQueue.GetQueuedOperations(request.Statuses, request.RequestedFrom, request.RequestedTo);
            return new GetQueuedOperationsResponse
            {
                Data = operations.Select(MapOperation)
            };
        }

        public GetOnliceNaatiOfficeAbbrAndEftMachineTermianlResponse GetOnlineNaatiAbbrOfficeAndEftMachineTerminal(GetOnlineNaatiAbbrOfficeAndEftMachineTerminalRequest request)
        {

            var onlineOfficeAbbr = NHibernateSession.Current.Query<Office>().Where(x => x.Id == request.OnlineOfficeId).ToList().Select(y => y.Institution.InstitutionAbberviation).FirstOrDefault();
            var onlineEftMachineTerminalNo = NHibernateSession.Current.Query<EFTMachine>().Where(x => x.Id == request.OnlineEftMachineId).ToList().Select(y => y.TerminalNo).FirstOrDefault();

            var getServiceResponse = new GetOnliceNaatiOfficeAbbrAndEftMachineTermianlResponse
            {
                OnlineOfficeAbbr = onlineOfficeAbbr,
                OnlineEftMachineTerminalNo = onlineEftMachineTerminalNo
            };

            return getServiceResponse;
        }

        private static QueuedOperationDto MapOperation(ExternalAccountingOperation source)
        {
            return new QueuedOperationDto
            {
                Id = source.Id,
                TypeId = source.Type.Id,
                TypeDisplayName = source.Type.DisplayName,
                RequestedBy = source.RequestedByUser.FullName,
                RequestedDateTime = source.RequestedDateTime,
                ProcessedDateTime = source.ProcessedDateTime,
                Description = source.Description,
                StatusId = source.Status.Id,
                StatusDescription = source.Status.DisplayName,
                Message = source.Message,
                BatchProcess = source.BatchProcess
            };
        }

        public CreatePaymentResponse AddPayments(Office office, int userId, IEnumerable<CreatePaymentModel> payments, string invoiceNumber, string creditNoteNumber, int? invoiceOperationId, bool batchProcess, bool deferred, CreatePaymentCompletionOperation createPaymentCompletionOperation)
        {
            if (string.IsNullOrWhiteSpace(WiisePaymentAccount))
            {
                throw new Exception("WiisePaymentAccount configuration does not exist.");
            }

            var paymentRequest = new WiiseCreatePaymentRequest
            {
                PrerequisiteRequestId = invoiceOperationId,
                Payments = payments.Select(p =>
                {
                    var wiiseReference = p.GetWiiseReference(office, p.PaymentType);

                    return new WiiseCreatePaymentModel
                    {
                        AccountId = Guid.Parse(WiisePaymentAccount),
                        Amount = p.Amount,
                        Date = p.Date,
                        InvoiceNumber = invoiceNumber,
                        CreditNoteNumber = creditNoteNumber,
                        Reference = string.IsNullOrWhiteSpace(p.Reference) ? wiiseReference.Reference : $"{wiiseReference.Reference} - {p.Reference}",
                    };
                })
            };

            var result = AccountingProcessing.CreatePayment(paymentRequest, createPaymentCompletionOperation, userId, batchProcess, deferred);
            return GetCreatePaymentResponse(result);
        }

        public bool HasUnpaidInvoices(int naatiNumber)
        {
            var getContactsResponse = GetContacts(new[] { naatiNumber });
            var contacts = getContactsResponse.ToList();
            return contacts.Any() && contacts.First().Balances > 0;
        }

        public FinanceBatchProcessResponse PerformBatchOperations(PerformBatchOperationsRequest request)
        {
            IList<ExternalAccountingOperation> operations = WiiseQueue.GetQueuedBatchOperations(ExternalAccountingOperationStatusName.Requested);
            FilterBatchOperationsAndCleanQueue(operations);
            return AccountingProcessing.ProcessBatch(operations, request.MaxBatchSize, request.UserId);
        }

        public string GetInvoiceNumber(Guid invoiceId)
        {
            var invoiceNumber = NHibernateSession.Current.Query<CredentialWorkflowFee>().FirstOrDefault(x => x.InvoiceId == invoiceId)?.InvoiceNumber;
            return invoiceNumber;
        }

        public Guid GetExternalAccountIdByCode(string code)
        {
            return NHibernateSession.Current.Query<GLCode>().FirstOrDefault(x => x.Code.Equals(code)).ExternalReferenceAccountId.Value;
        }

        public GenericResponse<bool> ProgressCreditNote(int applicationId, int credentialRequestId, RefundDto refund)
        {
            //find the objects to update. Having got this far the data should be correct
            var credentialRequest = NHibernateSession.Current.Query<CredentialRequest>().FirstOrDefault(x => x.Id.Equals(credentialRequestId));

            var credentialRefund = NHibernateSession.Current.Query<CredentialApplicationRefund>().FirstOrDefault(x => x.CredentialWorkflowFee.Id.Equals(credentialRequest.CredentialWorkflowFees.First().Id));

            //update

            //refund
            var systemActionType = NHibernateSession.Current.Get<SystemActionType>((int)refund.OnPaymentCreatedSystemActionTypeId);

            credentialRefund.CreditNoteNumber = refund.CreditNoteNumber;
            credentialRefund.PaymentReference = refund.PaymentReference;
            credentialRefund.RefundedDate = refund.RefundedDate;
            credentialRefund.CreditNotePaymentProcessedDate = refund.RefundedDate;
            credentialRefund.OnPaymentCreatedSystemActionType = systemActionType; //shoulld be 1056
            credentialRefund.DisallowProcessing = refund.DisallowProcessing.HasValue?refund.DisallowProcessing.Value:false;

            //application
            var credentialApplicationStatusType = NHibernateSession.Current.Get<CredentialApplicationStatusType>(6); //completed

            credentialRequest.CredentialApplication.CredentialApplicationStatusType = credentialApplicationStatusType;

            //credential
            var statusType = NHibernateSession.Current.Get<CredentialRequestStatusType>(16); //Withdrawn

            credentialRequest.CredentialRequestStatusType = statusType;

            //save
            NHibernateSession.Current.Flush();


            return new GenericResponse<bool>() { Data = true};
        }

        public GenericResponse<CredentialWorkflowFeeDto> FindCredentialWorkflowFee(int credentialRequestId)
        {
            var credentialWorkflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>().FirstOrDefault(x => x.CredentialRequest.Id == credentialRequestId);
            if(credentialWorkflowFee == null)
            {
                return new GenericResponse<CredentialWorkflowFeeDto>() { Success = false, Errors = { $"No Workflow Fee found for {credentialRequestId}" } };
            }

            return new CredentialWorkflowFeeDto()
            {
                InvoiceId = credentialWorkflowFee.InvoiceId,
                InvoiceNumber = credentialWorkflowFee.InvoiceNumber,
                PaymentActionProcessedDate = credentialWorkflowFee.PaymentActionProcessedDate,
                PaymentReference = credentialWorkflowFee.PaymentReference,
                ProductSpecification = new ProductSpecificationDetailsDto()
                {
                    Name = credentialWorkflowFee.ProductSpecification.Name,
                    ProductCategoryId = credentialWorkflowFee.ProductSpecification.ProductCategory.Id
                }
            };
        }

        public GenericResponse<bool> ProgressInvoice(int applicationId, int credentialRequestId, string invoiceNo, string invoiceId, string paymentId)
        {
            var credentialWorkflowFee = NHibernateSession.Current.Query<CredentialWorkflowFee>().FirstOrDefault(x=>x.CredentialRequest.Id == credentialRequestId);

            var completionInput = "{\"CredentialWorkflowFeeId\":" + credentialWorkflowFee.Id.ToString() + "}";

            var externalAccountingOperations = NHibernateSession.Current.Query<ExternalAccountingOperation>().Where(x => x.CompletionInput == completionInput).ToList();

            var invoice = externalAccountingOperations.FirstOrDefault(x => x.InputType == "F1Solutions.Naati.Common.Dal.Finance.Wiise.WiiseInvoiceCreationOperation");
            var payment = externalAccountingOperations.FirstOrDefault(x => x.InputType == "F1Solutions.Naati.Common.Dal.Finance.Wiise.WiisePaymentCreationOperation");

            if(invoice == null)
            {
                return GenerateGenericResponseError(applicationId, "WiiseInvoiceCreationOperation not found");
            }

            if (payment == null)
            {
                GenerateGenericResponseError(applicationId, "WiisePaymentCreationOperation not found");
                return new GenericResponse<bool>()
                {
                    Success = false
                };
            }



            //now make the updates

            credentialWorkflowFee.PaymentReference = paymentId;
            credentialWorkflowFee.InvoiceNumber = invoiceNo;
            var parsedInvoiceId = Guid.Empty;

            if (!Guid.TryParse(invoiceId, out parsedInvoiceId))
            {
                return GenerateGenericResponseError(applicationId, "Invalid Guid");
            }

            credentialWorkflowFee.InvoiceId = parsedInvoiceId;
            invoice.Output = invoiceId;

            payment.Output = paymentId;

            var status = NHibernateSession.Current.Get<ExternalAccountingOperationStatus>((int)4);//completed

            payment.Status = status;

            NHibernateSession.Current.Flush();

            return new GenericResponse<bool>(){
                Success = true
            };
        }

        private GenericResponse<bool> GenerateGenericResponseError(int applicationId, string error)
        {
            LoggingHelper.LogDebug($"Progress Credit Note for {applicationId} Error: {error}");

            return new GenericResponse<bool>()
            {
                Success = false,
                Errors = new List<string>
                {
                    error
                }
            };
        }

        private void FilterBatchOperationsAndCleanQueue(IList<ExternalAccountingOperation> operations)
        {
            // evict and delete operations for deleted applications
            foreach (var invoiceOperation in operations.Where(x => x.InputType == typeof(WiiseInvoiceCreationOperation).FullName).ToList())
            {
                var completion = WiiseQueue.GetCompletionOperation(invoiceOperation);

                var credentialWorkflowFeeId = (completion as CreateApplicationInvoiceCompletionOperation)?.CredentialWorkflowFeeId;
                if (credentialWorkflowFeeId == null)
                {
                    continue;
                }

                var application = NHibernateSession.Current.Get<CredentialWorkflowFee>(credentialWorkflowFeeId).CredentialApplication;

                // remove operations for Draft applications from the list
                if (application.CredentialApplicationStatusType.Name == CredentialApplicationStatusTypeName.Draft.ToString())
                {
                    var paymentOperation = operations.SingleOrDefault(x => x.PrerequisiteOperation != null && x.PrerequisiteOperation.Equals(invoiceOperation));
                    if (paymentOperation == null)
                    {
                        operations.Remove(invoiceOperation);
                    }
                }

                // remove operations for Deleted applications from the list, and delete them from the table
                else if (application.CredentialApplicationStatusType.Name == CredentialApplicationStatusTypeName.Deleted.ToString())
                {
                    using (var trans = NHibernateSession.Current.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
                    {
                        try
                        {
                            operations.Remove(invoiceOperation);
                            NHibernateSession.Current.Delete(invoiceOperation);
                            var paymentOperation = operations.SingleOrDefault(x => x.PrerequisiteOperation != null && x.PrerequisiteOperation.Equals(invoiceOperation));
                            if (paymentOperation != null)
                            {
                                operations.Remove(paymentOperation);
                                NHibernateSession.Current.Delete(paymentOperation);
                            }
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
            }
        }
    }
}
