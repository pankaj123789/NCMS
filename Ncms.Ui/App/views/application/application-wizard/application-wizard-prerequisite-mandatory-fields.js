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
            credentialRequest: null,
            action: null,
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
            missingMandatoryFields: ko.observableArray(),
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
            vm.selectedPrerequisiteApplications(vm.session.selectedApplications());

            vm.session.hasValidationError(false);

            vm.credentialPrerequisiteRequest.ParentApplicationId = vm.application.ApplicationId();
            vm.credentialPrerequisiteRequest.ParentCredentialRequestId = vm.credentialRequest.Id();
            vm.credentialPrerequisiteRequest.CreateApplications = true;
            vm.credentialPrerequisiteRequest.CredentialRequestTypes = [];

            ko.utils.arrayForEach(vm.selectedPrerequisiteApplications(), function (item) {
                var childPrerequisite = {
                    CredentialRequestType: item.PrerequisiteCredentialName(),
                    ApplicationTypeId: item.ApplicationTypePrerequisiteId(),
                    SkillId: item.PrerequisiteSkillId(),
                    HasValidationError: item.hasValidationError
                };

                vm.credentialPrerequisiteRequest.CredentialRequestTypes.push(childPrerequisite);
            });

            applicationService.post(vm.credentialPrerequisiteRequest, 'fieldMandatories').then(function (data) {
                vm.hasValidationError(data.validationErrors.MissingMandatoryFields.length > 0);
                vm.missingMandatoryFields(data.validationErrors.MissingMandatoryFields);
                vm.credentialPrerequisiteRequest.CredentialRequestTypes.forEach(function (preReqRequest) {
                    data.validationErrors.CreatePrerequisiteRequest.CredentialRequestTypes.forEach(function (preReqResponse) {
                        if (preReqRequest.CredentialRequestType == preReqResponse.CredentialRequestType && preReqResponse.HasValidationError == true) {
                            preReqRequest.HasValidationError = true;
                        }
                    });
                });
            });
        };

        vm.hasValidationError.subscribe(function () {
            if (vm.hasValidationError() == true) {
                vm.session.hasValidationError(true);
            }
            if (vm.hasValidationError() == false) {
                vm.session.hasValidationError(false);
            }

            if (vm.hasValidationError() == true && vm.credentialPrerequisiteRequest.CreateApplications == true) {
                vm.credentialPrerequisiteRequest.CreateApplications = false;
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