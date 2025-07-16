using System;
using System.Net.Http;
using System.Web.Http;
using F1Solutions.Naati.Common.Bl.Extensions;
using F1Solutions.Naati.Common.Bl.Security;
using F1Solutions.Naati.Common.Contracts.Bl;
using Ncms.Contracts;
using Ncms.Contracts.BackgroundTask;
using Ncms.Contracts.Models.Application;
using Ncms.Contracts.Models.MaterialRequest;
using Ncms.Contracts.Models.RolePlayer;
using Ncms.Contracts.Models.System;
using Ncms.Ui.Security;

namespace Ncms.Ui.Controllers.Api
{
    [RoutePrefix(NcmsIntegrationSettings.NcmsRoutePrefix)]
    [NcmsPrivateApiAuthentication(EndpointCaller.MyNaati)]
    public class PrivateApiController : ApiController
    {
        private readonly IApplicationService _applicationService;
        private readonly ITestSessionService _testSessionService;
        private readonly IMaterialRequestService _materialRequestService;
 
        private readonly INcmsRefreshPendingUsersTask _refreshPendingUsersTask;
        private readonly INcmsRefreshCookieTask _refreshCookieTask;
        private readonly INcmsRefreshSystemCacheTask _refreshSystemCacheTask;
        private readonly INcmsRefreshAllUserCacheTask _refreshAllUserCacheTask;
        private readonly INcmsRefreshNotificationsTask _refreshNotificationsTask;

        public PrivateApiController(IApplicationService applicationService, ITestSessionService testSessionService,
            IMaterialRequestService materialRequestService,
            INcmsRefreshPendingUsersTask refreshPendingUsersTask,
            INcmsRefreshCookieTask refreshCookieTask, 
            INcmsRefreshSystemCacheTask refreshSystemCacheTask,
            INcmsRefreshAllUserCacheTask refreshAllUserCacheTask,
            INcmsRefreshNotificationsTask refreshNotificationsTask)
        {
            _applicationService = applicationService;
            _testSessionService = testSessionService;
            _materialRequestService = materialRequestService;
            _refreshPendingUsersTask = refreshPendingUsersTask;
            _refreshCookieTask = refreshCookieTask;
            _refreshSystemCacheTask = refreshSystemCacheTask;
            _refreshAllUserCacheTask = refreshAllUserCacheTask;
            _refreshNotificationsTask = refreshNotificationsTask;
        }

        /// <summary>
        /// Create a new Application for MyNaati workflow
        /// </summary>
        /// <param name="model">Application</param>
        /// <returns>Id from Application, Credential Request, Notes and Sections added/updated</returns>
        [HttpPost]
        [Route(NcmsIntegrationSettings.CreateApplication)]
        public HttpResponseMessage CreateApplicationForMyNaati(dynamic model)
        {
            var request = new UpsertApplicationRequestModel
            {
                ApplicationInfo = new CredentialApplicationInfoModel
                {
                    NaatiNumber = (int)model.NaatiNumber.Value,
                    ApplicationStatusTypeId = (int)model.ApplicationStatusTypeId,
                    ApplicationTypeId = (int)model.ApplicationTypeId,
                }
            };
            return this.CreateResponse(() => _applicationService.CreateMyNaatiApplication(request));
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.ApplicationWizard)]
        public HttpResponseMessage PostMyNaatiWizard(dynamic request)
        {
            try
            {
                var model = new MyNaatiApplicationActionWizardModel()
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                model.SetProcessInBatch(true);
                model.SetTestessionId((int)(request.TestSessionId?.Value ?? 0));
                model.SetPaymentReference(request.PaymentReference?.Value, request.PaymentAmount != null ? Convert.ToDecimal(request.PaymentAmount?.Value) : null, request.TransactionId?.Value, request.OrderNumber?.Value);
                model.SetInvoiceReference(request.InvoiceReference?.Value);
                model.SetDueDate(request.DueDate?.Value);
                model.SetRefundDetails(
                    (double?)request.RefundPercentage,
                    (int?)request.RefundMethodTypeId,
                    (int?)request.CredentialWorkflowFeeId,
                    null,
                    (string)request.RefundComments,
                    (string)request.RefundBankDetails);

                return this.CreateIntegrationResponse(() => _applicationService.PerformAction(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.ApplicationValidate)]
        public HttpResponseMessage MyNaatiWizardValidate(dynamic request)
        {
            try
            {
                var model = new MyNaatiApplicationActionWizardModel()
                {
                    ApplicationId = (int)request.ApplicationId.Value,
                    CredentialRequestId = (int)(request.CredentialRequestId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                model.SetProcessInBatch(true);
                model.SetTestessionId((int)(request.TestSessionId?.Value ?? 0));
                model.SetPaymentReference(request.PaymentReference?.Value, request.PaymentAmount != null ? Convert.ToDecimal(request.PaymentAmount?.Value) : null, null, null);
                model.SetInvoiceReference(request.InvoiceReference?.Value);
                model.SetDueDate(request.DueDate?.Value);
                return this.CreateResponse(() =>
                {
                    var response = _applicationService.ValidateActionPreconditions(model).Data;
                    return response;
                });
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RolePlayerWizard)]
        public HttpResponseMessage PostRolePlayerWizard(dynamic request)
        {
            try
            {
                var testSessionRolePlayerId = (int)request.TestSessionRolePlayerId.Value;

                var model = new RolePlayerUpdateWizardModel()
                {
                    ActionType = (int)request.ActionId.Value,
                };

                return this.CreateResponse(() => _testSessionService.PerformRolePlayerAction(testSessionRolePlayerId, model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.ApplicationOutstandingInvoices)]
        public HttpResponseMessage PostOutstandingInvoices(dynamic request)
        {
            try
            {
                var model = new UpdateOutstandingInvoicesRequestModel
                {
                    InvoiceNumber = request.InvoiceNumber?.Value,
                    PaymentReference = request.PaymentReference?.Value,
                    TransactionId = request.TransactionId?.Value,
                    OrderNumber = request.OrderNumber?.Value,
                };

                return this.CreateResponse(() => _applicationService.UpdateOutstandingInvoices(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.MaterialRequestWizardValidate)]
        public HttpResponseMessage MyNaatiMaterialRequestWizardValidate(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRequestRoundId?.Value ?? 0),
                    TestMaterialDomainId = (int)(request.TestMaterialDomainId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                model.SetSource(SystemActionSource.MyNaati);
                return this.CreateResponse(() =>
                {
                    var response = _materialRequestService.ValidateActionPreconditions(model).Data;
                    return response;

                });
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.MaterialRequestWizard)]
        public HttpResponseMessage MyNaatiPostWizard(dynamic request)
        {
            try
            {
                var model = new MaterialRequestWizardModel
                {
                    MaterialRequestId = (int)request.MaterialRequestId.Value,
                    MaterialRequestRoundId = (int)(request.MaterialRequestRoundId?.Value ?? 0),
                    TestMaterialDomainId = (int)(request.TestMaterialDomainId?.Value ?? 0),
                    ActionType = (int)request.ActionId.Value,
                    Data = request.Steps
                };
                model.SetSource(SystemActionSource.MyNaati);
                return this.CreateResponse(() => _materialRequestService.PerformAction(model));
            }
            catch (Exception ex)
            {
                return this.FailureResponse(ex);
            }
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.TestConnection)]
        public HttpResponseMessage TestConnection(dynamic request)
        {
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RefreshUserCache)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage RefreshUserCache(RefreshUsersRequest request)
        {
            _refreshPendingUsersTask.RefreshLocalUsers(request.Users);
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RefreshAllUsersCache)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage RefreshAllUsersCache(dynamic request)
        {
            _refreshAllUserCacheTask.RefreshAllUsersLocalCache();
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RefreshAllInvalidCookies)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage RefreshAllInvalidCookies(dynamic request)
        {
            _refreshCookieTask.RefreshAllInvalidLocalCookies();
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RefreshSystemCache)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage RefreshSystemCache(dynamic request)
        {
            _refreshSystemCacheTask.RefreshLocalSystemCache();
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.ExecuteRefreshPendingUsersTask)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage ExecuteRefreshPendingUserTask(dynamic request)
        {
            Startup.RefreshPendingUsersCache(null, null);
            return this.CreateResponse(() => true);
        }

        [HttpPost]
        [Route(NcmsIntegrationSettings.RefreshUserNotifications)]
        [NcmsPrivateApiAuthentication(EndpointCaller.Ncms)]
        public HttpResponseMessage RefreshUserNotifications(RefreshUserNotificationsRequest request)
        {
            _refreshNotificationsTask.RefreshLocalUserNotifications(request.UserNames);
            return this.CreateResponse(() => true);
        }
    }
}