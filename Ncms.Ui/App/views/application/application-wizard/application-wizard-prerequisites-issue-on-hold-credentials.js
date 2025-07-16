define([
    'services/application-data-service',
    'services/util',
    'modules/custom-validator',
    'modules/enums',
], function (applicationService, util, customValidator, enums) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var serverModel = {
            Checked: ko.observable(),
            Message: ko.observable(),
            OnDisableMessage: ko.observable(),
            OnEnableMessage: ko.observable()
        };

        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
        };

        $.extend(defaultParams, params);

        var vm = {
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
            onHoldCredentials: ko.observableArray([]),
            option: serverModel,
            onHoldLoaded: ko.observable(false),
            optionLoaded: ko.observable(false),
        };

        vm.tableDefinition = {
            dom: "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-4'l><'col-sm-3'i><'col-sm-5'p>>",
            searching: false,
            paging: false,
            oLanguage: { sInfoEmpty: '', sInfo: '' },
            columnDefs: [
                {
                    orderable: false,
                    targets: 0
                }
            ],
        };

        vm.load = function () {
            applicationService.getFluid('getOnHoldCredentialsToIssue/{0}'.format(vm.credentialRequest.Id())).then(function (data) {
                ko.utils.arrayForEach(data, function (item) {
                    if (item.CredentialRequestId == vm.credentialRequest.Id()) {
                        return;
                    }

                    item.appStatus = getAppStatus(item.CredentialApplicationStatusTypeId);
                    item.credStatus = getCredStatus(item.CredentialRequestStatusTypeId);
                    item.url = '#application/{0}'.format(item.CredentialApplicationId);
                    item.ExistingApplicationId = 'APP{0}'.format(item.CredentialApplicationId);

                    vm.onHoldCredentials.push(item);
                });

                vm.onHoldLoaded(true);
            });

            applicationService.getFluid('issueOnHoldCredentialCheckOptionData').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
                vm.optionLoaded(true);
            });
        };

        vm.noOnHoldCreds = ko.pureComputed(function () {
            if (vm.onHoldCredentials().length == 0) {
                return true;
            }

            return false;
        });

        vm.allLoaded = ko.pureComputed(function () {
            if (vm.onHoldLoaded() && vm.optionLoaded()) {
                return true;
            }

            return false;
        });

        vm.isValid = function () {
            return true;
        };

        vm.postData = function () {
            if (vm.option.Checked()) {
                ko.utils.arrayForEach(vm.onHoldCredentials(), function (item) {
                    delete item.appStatus;
                    delete item.url;
                    delete item.ExistingApplicationId;
                    delete item.credStatus;
                });
            }

            var request = {
                Checked: vm.option.Checked(),
                OnHoldCredentials: vm.onHoldCredentials()
            }

            return request;
        };

        vm.activate = function () {
            return;
        }

        function getAppStatus(status) {
            var statuses = enums.ApplicationStatusTypes;

            if (status === statuses.Draft) {
                return 'label-gray';
            }

            if (status === statuses.ProcessingSubmission || status === statuses.ProcessingApplicationInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.Entered) {
                return 'label-info';
            }
            if (status === statuses.BeingChecked) {
                return 'label-orange';
            }
            if (status === statuses.Rejected) {
                return 'label-danger';
            }
            if (status === statuses.InProgress) {
                return 'label-orange';
            }
            if (status === statuses.Completed) {
                return 'label-success';
            }
            if (status === statuses.AwaitingAssessmentPayment || status === statuses.AwaitingApplicationPayment) {
                return 'label-dark-yellow';
            }
            else {
                return 'label-success';
            }
        }

        function getCredStatus(status) {
            var statuses = enums.CredentialRequestStatusTypes;

            if (status === statuses.Draft) {
                return 'label-gray';
            }

            if (status === statuses.ProcessingSubmission) {
                return 'label-dark-gray';
            }

            if (status === statuses.RequestEntered) {
                return 'label-info';
            }
            if (status === statuses.ReadyForAssessment) {
                return 'label-info';
            }
            if (status === statuses.BeingAssessed || status === statuses.CertificationOnHold) {
                return 'label-orange';
            }
            if (status === statuses.Pending) {
                return 'label-dark-yellow';
            }
            if (status === statuses.AssessmentFailed) {
                return 'label-danger';
            }
            if (status === statuses.AssessmentPaidReview) {
                return 'label-orange';
            }
            if (status === statuses.EligibleForTesting) {
                return 'label-info';
            }
            if (status === statuses.AssessmentSuccessful) {
                return 'label-orange';
            }
            if (status === statuses.Rejected) {
                return 'label-danger';
            }
            if (status === statuses.TestFailed) {
                return 'label-danger';
            }
            //if (status === statuses.CredentialIssued) {
            //    return 'label-success';
            //}
            if (status === statuses.Withdrawn) {
                return 'label-purple';
            }
            if (status === statuses.Canceled) {
                return 'label-purple';
            }
            if (status === statuses.Deleted) {
                return 'label-purple';
            }
            if (status === statuses.AwaitingTestPayment || status === statuses.AwaitingApplicationPayment || status === statuses.AwaitingSupplementaryTestPayment) {
                return 'label-dark-yellow';
            }
            if (status === statuses.TestAccepted) {
                return 'label-info';
            }
            if (status === statuses.TestSat) {
                return 'label-orange';
            }
            if (status === statuses.CheckIn) {
                return 'label-orange';
            }
            if (status === statuses.UnderPaidTestReview) {
                return 'label-orange';
            }
            if (status === statuses.IssuedPassResult) {
                return 'label-orange';
            }
            if (status === statuses.ProcessingPaidReviewInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.AwaitingPaidReviewPayment) {
                return 'label-dark-yellow';
            }
            if (status === statuses.SupplementaryTestInvoiceProcessed) {
                return 'label-dark-gray';
            }
            if (status === statuses.TestInvalidated) {
                return 'label-purple';
            }
            if (status === statuses.ToBeIssued) {
                return 'label-dark-gray';
            }
            if (status === statuses.ProcessingTestInvoice) {
                return 'label-dark-gray';
            }
            if (status === statuses.ProcessingRequest) {
                return 'label-dark-gray';
            }
            if (status === statuses.RefundRequested) {
                return 'label-orange';
            }
            if (status === statuses.ProcessingCreditNote) {
                return 'label-dark-gray';
            }
            if (status === statuses.AwaitingCreditNotePayment) {
                return 'label-orange';
            }
            if (status === statuses.RefundRequestApproved) {
                return 'label-dark-gray';
            }
            if (status === statuses.RefundFailed) {
                return 'label-orange';
            }
            if (status === statuses.CertificationOnHold) {
                return 'label-orange';
            }
            if (status == statuses.OnHoldToBeIssued){
                return 'label-orange';
            }

            return 'label-success';
        }

        return vm;
    };
});