using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using F1Solutions.Global.Common.Logging;
using F1Solutions.Global.Common.Mapping;
using F1Solutions.Naati.Common.Contracts.Dal;
using F1Solutions.Naati.Common.Contracts.Dal.Enum;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;
using F1Solutions.Naati.Common.Contracts.Dal.Request;
using F1Solutions.Naati.Common.Contracts.Dal.Response;
using F1Solutions.Naati.Common.Dal.Domain;
using MyNaati.Bl.Properties;
using MyNaati.Contracts.BackOffice;
using InvoiceStatus = MyNaati.Contracts.BackOffice.InvoiceStatus;
using InvoiceType = F1Solutions.Naati.Common.Contracts.Dal.Enum.InvoiceType;


namespace MyNaati.Bl.BackOffice
{
    public class AccountingService : IAccountingService
    {
        
        private readonly IFinanceService mFinanceService;
        private readonly IApplicationQueryService mApplicationQueryService;
        private readonly IAutoMapperHelper _autoMapperHelper;

        public AccountingService(IFinanceService financeService, IApplicationQueryService applicationQueryService, IAutoMapperHelper autoMapperHelper)
        {
            mFinanceService = financeService;
            mApplicationQueryService = applicationQueryService;
            _autoMapperHelper = autoMapperHelper;
        }

        public GetAccountingInvoicesResponse GetInvoices(GetAccountingInvoiceRequestContract request)
        {

            var response = new GetAccountingInvoicesResponse();

            var serviceRequest = _autoMapperHelper.Mapper.Map<GetInvoicesRequest>(request);
            try
            {
                var serviceResponse = mFinanceService.GetInvoices(serviceRequest);

                if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
                {
                    response.Warnings.Add(serviceResponse.WarningMessage);
                }

                if (!serviceResponse.Error)
                {
                    response.Data = serviceResponse.Invoices.Select(_autoMapperHelper.Mapper.Map<AccountingInvoiceDto>)
                        .Where(x => x.Status == InvoiceStatus.Open || x.Status == InvoiceStatus.Paid);
                }
                else
                {
                    throw new Exception($"Error getting invoices: {serviceResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.UnHandledException = true;
                response.UnHandledExceptionMessage = Resources.InvoiceRetrievalError;
                try
                {
                    LoggingHelper.LogException(ex, Resources.InvoiceRetrievalError);
                }
                catch
                {
                    // Deliberately ignore
                }
            }

            return response;
        }

        public GetAccountingInvoicesResponse GetUnraisedInvoices(int naatiNumber)
        {
            var response = new GetAccountingInvoicesResponse();

            try
            {
                var serviceResponse = mFinanceService.GetUnraisedInvoices(naatiNumber);                

                if (!string.IsNullOrEmpty(serviceResponse.WarningMessage))
                {
                    response.Warnings.Add(serviceResponse.WarningMessage);
                }

                if (!serviceResponse.Error)
                {
                    response.Data = serviceResponse.Invoices.Select(_autoMapperHelper.Mapper.Map<AccountingInvoiceDto>)
                        .Where(x => x.Status == InvoiceStatus.Open || x.Status == InvoiceStatus.Paid);
                }
                else
                {
                    throw new Exception($"Error getting invoices: {serviceResponse.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.UnHandledException = true;
                response.UnHandledExceptionMessage = Resources.InvoiceRetrievalError;
                try
                {
                    LoggingHelper.LogException(ex, Resources.InvoiceRetrievalError);
                }
                catch
                {
                    // Deliberately ignore
                }
            }

            var addedFees = new List<AccountingInvoiceDto>();
            foreach (var unraisedInvoice in response.Data)
            {
                var application = mApplicationQueryService.GetApplication(unraisedInvoice.CredentialApplicationId.Value);

                var addedFee = unraisedInvoice;
                var fee = mApplicationQueryService.GetApplicationTypeFees(unraisedInvoice.CredentialApplicationTypeId.Value, FeeTypeName.ApplicationAssessment).FeeProducts.First().ProductSpecification;
                bool isNaatiFunded = application.Result.SponsorInstitutionNaatiNumber == 950079; //checking if the application is sponsored by NAATI 
                unraisedInvoice.CredentialApplicationTypeDisplayName = fee.Description;
                addedFee.Code = fee.Code;
                addedFee.AmountDue = fee.CostPerUnit;
                addedFee.GST = fee.GstApplies && application.Result.HasAddressInAustralia;
                addedFee.Balance = fee.CostPerUnit;
                if (isNaatiFunded)
                {
                    addedFee.IsNaatiSponsored = true;
                }             
                addedFees.Add(addedFee);
            }
            response.Data = addedFees;

            return response;
        }

        public GetAccountingInvoicePdfResponse GetInvoicePdfByInvoiceId(GetInvoicePdfRequestContract request)
        {
            var response = new GetAccountingInvoicePdfResponse();
            
            if (request.Location == FinanceInfoLocation.Wiise)
            {
                var serviceRequest = new GetInvoicePdfByIdRequest
                {
                    InvoiceId = request.InvoiceId,
                    Type = (InvoiceType)request.Type
                };

                var serviceResponse = mFinanceService.GetInvoicePdfById(serviceRequest);

                if (serviceResponse.Error)
                {
                    throw new Exception(serviceResponse.ErrorMessage);
                }

                response.AccountingInvoicePdfFileContent = serviceResponse.FileContent;
            }

            return response;
        }

        public GetAccountingInvoicePdfResponse GetInvoicePdfByInvoiceNumber(GetInvoicePdfRequestContract request)
        {
            var response = new GetAccountingInvoicePdfResponse();

            if (request.Location == FinanceInfoLocation.Wiise)
            {
                var serviceRequest = new GetInvoicePdfRequest
                {
                    InvoiceNumber = request.InvoiceNumber,
                    Type = (InvoiceType)request.Type
                };

                var serviceResponse = mFinanceService.GetInvoicePdf(serviceRequest);

                if (serviceResponse.Error)
                {
                    throw new Exception(serviceResponse.ErrorMessage);
                }

                response.AccountingInvoicePdfFileContent = serviceResponse.FileContent;
            }

            return response;
        }

        public GetOfficeAbbrAndEftMachineTermianlResponse GetOnlineofficeAbbrAndEftMachineTerminal(GetOfficeAbbrAndEftMachineTermianlRequest request)
        {
            GetOnliceNaatiOfficeAbbrAndEftMachineTermianlResponse serviceResponse = null;
            var serviceRequest = new GetOnlineNaatiAbbrOfficeAndEftMachineTerminalRequest
            {
                OnlineOfficeId = request.OnlineOfficeId,
                OnlineEftMachineId = request.OnlineEftMachineId
            };

            serviceResponse = mFinanceService.GetOnlineNaatiAbbrOfficeAndEftMachineTerminal(serviceRequest);

            var getOfficeAbbrAndEftMachineTermianlResponse = new GetOfficeAbbrAndEftMachineTermianlResponse
            {
                OnlineEftMachineTerminalNo = serviceResponse.OnlineEftMachineTerminalNo,
                OnlineOfficeAbbr = serviceResponse.OnlineOfficeAbbr
            };
            return getOfficeAbbrAndEftMachineTermianlResponse;
        }

        public string GetInvoiceNumber(Guid invoiceId)
        {
            var invoiceNumber = mFinanceService.GetInvoiceNumber(invoiceId);
            return invoiceNumber;
        }

        public PaymentCreateResponseModel CreatePayment(PaymentCreateRequestModel paymentCreateRequestModel)
        {
            var paymentCreateResponseModel = new PaymentCreateResponseModel();

            try
            {
                var user = mApplicationQueryService.GetUser(new GetUserRequest { UserName = paymentCreateRequestModel.UserName });

                var requestModel = _autoMapperHelper.Mapper.Map<CreatePaymentModel>(paymentCreateRequestModel);
                requestModel.Date = paymentCreateRequestModel.DatePaid;
                requestModel.AccountId = paymentCreateRequestModel.AccountId;

                var payments = new List<CreatePaymentModel> { requestModel };

                var createPaymentRequest = new CreatePaymentRequest
                {
                    UserId = user.UserId ?? 0,
                    Payments = payments
                };
                
                var serviceResponse = mFinanceService.CreatePayment(createPaymentRequest);

                paymentCreateResponseModel.InvoiceId = paymentCreateRequestModel.InvoiceNumber;
                paymentCreateResponseModel.Error = serviceResponse.Error;
                paymentCreateResponseModel.ErrorMessage = serviceResponse.ErrorMessage;
                paymentCreateResponseModel.OperationId = serviceResponse.OperationId;
                paymentCreateResponseModel.Reference = serviceResponse.Reference;
                paymentCreateResponseModel.PaymentId = serviceResponse.PaymentId;
                paymentCreateResponseModel.UnHandledException = false;
            }
            catch (Exception ex)
            {
                var message =
                    "There was an error in processing your payment. Please do not attempt to pay again. Please contact NAATI at finance@naati.com.au, and quote the following information: " +
                    $"SecurePay Reference Number: {paymentCreateRequestModel.Reference}, Invoice Number: {paymentCreateRequestModel.InvoiceNumber}, Customer Number {paymentCreateRequestModel.NaatiNumber}";
                
                paymentCreateResponseModel.UnHandledException = true;
                paymentCreateResponseModel.UnHandledExceptionMessage = message;

                try
                {
                    LoggingHelper.LogException(ex, message);
                }
                catch
                {
                    //Deliverately ignore
                }
            }
           
            return paymentCreateResponseModel;
        }

    }
}
