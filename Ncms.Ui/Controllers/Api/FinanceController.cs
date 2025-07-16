using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Contracts.Security;
using Ncms.Contracts;
using Ncms.Contracts.Models;
using Ncms.Contracts.Models.Accounting;
using Ncms.Contracts.Models.Person;
using Ncms.Ui.Extensions;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix("api/finance")]
    public class FinanceController : BaseApiController
    {
        private readonly IAccountingService _accountingService;
        private readonly ISystemService _systemService;
        private readonly IPersonService _personService;

        public FinanceController(IAccountingService accountingService, ISystemService systemService, IPersonService personService)
        {
            _accountingService = accountingService;
            _systemService = systemService;
            _personService = personService;
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Invoice)]
        [Route("invoiceoptions")]
        public HttpResponseMessage InvoiceOptionsGet()
        {
            try
            {
                var themes = _accountingService.GetInvoiceBrandingThemes();
                if (!themes.Success)
                {
                    return this.CreateResponse(themes);
                }

                var defaultBranding = themes.Data.First().BrandingThemeId;

                return this.CreateResponse(() => new
                {
                    SalesInvoiceBrandings = themes.Data,
                    DefaultBranding = defaultBranding
                });
            }
            catch (Exception e)
            {
                return this.FailureResponse(e);
            }
        }

        [HttpGet]
        [Route("paymentoptions")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Payment)]
        public HttpResponseMessage PaymentOptionsGet()
        {
            try
            {
                var accountingOptions = _accountingService.GetBankAccounts();
                if (!accountingOptions.Success)
                {
                    return this.CreateResponse(accountingOptions);
                }

                // todo add GetAccount(id) to service
                var paymentAccount = _systemService.GetSystemValue("WiisePaymentAccount");
                var accountName = !string.IsNullOrEmpty(paymentAccount)
                    ? accountingOptions.Data.SingleOrDefault(x => x.Id.Equals(Guid.Parse(paymentAccount)))?.Name
                    : string.Empty;

                return this.CreateResponse(() => new
                {
                    PaymentAccount = new
                    {
                        Id = paymentAccount,
                        Name = accountName
                    }
                });
            }
            catch (Exception e)
            {
                return this.FailureResponse(e);
            }
        }

        [HttpGet]
        [Route("offices")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.FinanceOther)]
        public HttpResponseMessage OfficesGet()
        {
            return this.CreateResponse(_accountingService.GetOffices);
        }

        [HttpGet]
        [Route("eftMachines")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.FinanceOther)]
        public HttpResponseMessage EftMachinesGet()
        {
            return this.CreateResponse(_accountingService.GetEftMachines);
        }

        [HttpGet]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.FinanceOther)]
        [Route("eftMachines/{officeId}")]
        public HttpResponseMessage EftMachinesGet(int officeId)
        {
            Func<object> search = () =>
            {
                var response = _accountingService.GetEftMachines();

                if (response.Success)
                {
                    return response.Data.Where(x => x.OfficeId == officeId);
                }

                throw new Exception(string.Join("; ", response.Errors));
            };

            return this.CreateResponse(search);
        }

        [HttpGet]
        [Route("exportEndOfPeriod")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.FinanceOther)]
        public HttpResponseMessage ExportEndOfPeriod([FromUri]InvoiceRequest request)
        {
            return this.FileStreamResponse(() => _accountingService.ExportEndOfPeriod(request));
        }

        [HttpGet]
        [Route("invoice")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Invoice)]
        public HttpResponseMessage InvoiceGet([FromUri]InvoiceRequest request)
        {
            return this.CreateResponse(() => _accountingService.GetInvoices(request));
        }

        [HttpGet]
        [Route("purchaseinvoice")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Invoice)]
        public HttpResponseMessage PurchaseInvoiceGet([FromUri]InvoiceRequest request)
        {
            return this.CreateResponse(() => _accountingService.GetPurchaseInvoices(request));
        }

        [HttpGet]
        [Route("payment")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Payment)]
        public HttpResponseMessage PaymentGet([FromUri]EndOfPeriodRequest request)
        {
            return this.CreateResponse(() => _accountingService.GetPayments(request));
        }

        [HttpGet]
        [Route("downloadInvoice")]
        [NcmsAuthorize(SecurityVerbName.Download, SecurityNounName.Invoice)]
        public HttpResponseMessage DownloadInvoicePdf(string number, InvoiceTypeModel type)
        {
            return this.FileStreamResponse(() => _accountingService.GetInvoicePdf(number, type));
        }

        [HttpPost]
        [Route("invoice")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Invoice)]
        public HttpResponseMessage InvoicePost([FromBody]InvoiceCreateRequestModel model)
        {
            model.CancelOperationIfError = true;
            return this.CreateResponse(() => _accountingService.CreateInvoice(model), r => r.Data?.OperationId != null);
        }

        [HttpPost]
        [Route("retryInvoice")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Invoice)]
        public HttpResponseMessage RetryInvoicePost([FromBody]SubmitQueuedOperationModel model)
        {
            return this.CreateResponse(() => _accountingService.RetryCreateInvoice(model.OperationId), r => r.Data?.OperationId != null);
        }

        [HttpPost]
        [Route("payment")]
        [NcmsAuthorize(SecurityVerbName.Create, SecurityNounName.Payment)]
        public HttpResponseMessage PaymentPost([FromBody]PaymentCreateRequestModel[] models)
        {
            return this.CreateResponse(() => _accountingService.CreatePayment(models), r => r.Data?.OperationId != null);
        }

        [HttpPost]
        [Route("retryPayment")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Payment)]
        public HttpResponseMessage RetryPaymentPost([FromBody]SubmitQueuedOperationModel model)
        {
            return this.CreateResponse(() => _accountingService.RetryCreatePayment(model.OperationId), r => r.Data?.OperationId != null);
        }

        [HttpPost]
        [Route("progressCreditNote")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Payment)]
        public HttpResponseMessage ProgressCreditNote(ProgressCreditNoteModel model)
        {
            return this.CreateResponse(() => _accountingService.ProgressCreditNote(model), r => r.Data);
        }

        [HttpPost]
        [Route("progressInvoice")]
        [NcmsAuthorize(SecurityVerbName.Update, SecurityNounName.Payment)]
        public HttpResponseMessage ProgressInvoice(ProgressInvoiceModel model)
        {
            return this.CreateResponse(() => _accountingService.ProgressInvoice(model), r => r.Data);
        }


        private IEnumerable<EntitySearchResultModel> SearchPeopleAndInstitutions(QueryRequest query)
        {
            var result = _personService.PersonAndAndInstitutionSearch(query);

            if (!result.Success)
            {
                throw new Exception(string.Join("; ", result.Errors));
            }

            return result.Data;
        }

        [HttpPost]
        [Route("cancel")]
        [NcmsAuthorize(SecurityVerbName.Manage, SecurityNounName.PayRun)]
        public HttpResponseMessage CancelPost([FromBody]OperationCancelRequestModel model)
        {
            return this.CreateResponse(() => _accountingService.CancelOperation(model));
        }

        [HttpGet]
        [Route("bills")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.Invoice, SecurityNounName.Person)]
        public HttpResponseMessage BillsGet([FromUri]string naatiNumber)
        {
            Func<int, dynamic> bill = i =>
            {
                var billNumber = i.ToString().PadLeft(4, i.ToString()[0]);

                return new
                {
                    BillNo = billNumber,
                    Status = "Status",
                    Total = i * 1000,
                    AmountDue = i * 100,
                    CreatedDate = DateTime.Now.AddDays(i * -1)
                };
            };

            return this.CreateResponse(() => CreateDummyList(bill, 3));
        }

        [HttpGet]
        [Route("customer")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.FinanceOther)]
        public HttpResponseMessage CustomerSearchGet([FromUri]QueryRequest query)
        {
            var search = new Func<IEnumerable<object>>(() =>
            {
                return SearchPeopleAndInstitutions(query).Select(x => new
                {
                    Type = x.EntityTypeId,
                    TypeName = x.PersonId.HasValue ? "Person" : "Organisation",
                    x.EntityId,
                    x.NaatiNumber,
                    x.Name
                });
            });

            return this.CreateResponse(search);
        }

        [HttpGet]
        [Route("newpayment/search")]
        [NcmsAuthorize(SecurityVerbName.Search, SecurityNounName.Payment)]
        public HttpResponseMessage NewPaymentSearchGet([FromUri]QueryRequest query)
        {
            var result = new List<object>();
            var search = new Func<IEnumerable<object>>(() =>
            {
                result.AddRange(SearchPeopleAndInstitutions(query).Select(x => new
                {
                    Type = x.EntityTypeId.ToString(),
                    TypeName = x.PersonId.HasValue ? "Person" : "Organisation",
                    EntityId = x.EntityId.ToString(),
                    NaatiNumber = x.NaatiNumber.HasValue ? x.NaatiNumber.ToString() : "",
                    x.Name
                }));

                if (result.Any())
                {
                    return result;
                }

                var request = new InvoiceRequest
                {
                    InvoiceNumber = new[] { query.Term },
                    IncludeFullPaymentInfo = false
                };

                var inv = _accountingService.GetInvoices(request);
                if (!inv.Success)
                {
                    throw new Exception(string.Join("; ", inv.Errors));
                }

                if (inv.Data != null)
                {
                    result.AddRange(inv.Data.Select(x => new
                    {
                        Type = x.Type == InvoiceTypeModel.CreditNote ? "C" : "I",
                        TypeName = x.Type == InvoiceTypeModel.CreditNote ? "Credit note" : "Invoice",
                        EntityId = x.InvoiceNumber,
                        NaatiNumber = x.NaatiNumber.ToString(),
                        Name = x.WiiseReference
                    }));
                }

                return result;
            });

            return this.CreateResponse(search);
        }

        [HttpGet]
        [Route("financequeue")]
        [NcmsAuthorize(SecurityVerbName.Read, SecurityNounName.FinanceOther)]
        public HttpResponseMessage FinanceQueueGet([FromUri]string request)
        {
            return this.CreateSearchResponse(() => _accountingService.GetQueuedOperations(request));
        }
    }
}
