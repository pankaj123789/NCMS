define([
    'services/application-data-service',
    'services/util',
    'modules/custom-validator'
], function (applicationService, util, customValidator) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {

        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
        };

        $.extend(defaultParams, params);

        var vm = {
            session: params.preRequisiteSession,
            application: defaultParams.application,
            credentialRequest: defaultParams.credentialRequest,
            action: defaultParams.action,
            visibleSteps: defaultParams.visibleSteps,
            selectedPrerequisiteApplications: ko.observableArray(),
            hasValidationError: ko.observable(false),
            credentialPrerequisiteRequest: params.credentialPrerequisiteRequest,
            missingMandatoryDocuments: ko.observableArray(),
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
            ]
        };

        vm.load = function () {
            applicationService.post(vm.credentialPrerequisiteRequest, 'documentMandatories').then(function (data) {
                vm.hasValidationError(data.validationErrors.MissingMandatoryDocuments.length > 0);
                vm.missingMandatoryDocuments(data.validationErrors.MissingMandatoryDocuments);
                vm.credentialPrerequisiteRequest.CredentialRequestTypes.forEach( function (preReqRequest) {
                    data.validationErrors.CreatePrerequisiteRequest.CredentialRequestTypes.forEach( function (preReqResponse) {
                        if (preReqRequest.CredentialRequestType == preReqResponse.CredentialRequestType && preReqResponse.HasValidationError == true) {
                            preReqRequest.HasValidationError = true;
                        }
                    });
                });
            });
        };

        vm.hasValidationError.subscribe(function () {
            if (vm.hasValidationError() == true && vm.credentialPrerequisiteRequest.CreateApplications == true) {
                vm.credentialPrerequisiteRequest.CreateApplications = false;
            }
            if (vm.hasValidationError() == true) {
                vm.session.hasValidationError(true);
            }
            
        });

        vm.isValid = function () {
            return true;
        };

        vm.activate = function () {

        }

        return vm;
    };
});