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
            checked: ko.observable(false),
            hasValidationError: ko.observable(false),
            credentialPrerequisiteRequest: params.credentialPrerequisiteRequest,
            createApplications: ko.observable()
        };

        vm.load = function () {
            vm.hasValidationError(vm.session.hasValidationError());

            vm.createApplications(vm.credentialPrerequisiteRequest.CreateApplications);

            //if (vm.hasValidationError() == true) {
            //    ko.utils.arrayForEach(vm.credentialPrerequisiteRequest.CredentialRequestTypes, function (item) {
            //        item.HasValidationError = true;
            //    });
            //}
        };

        vm.isValid = function () {
            return true;
        };

        vm.postData = function () {
            return vm.credentialPrerequisiteRequest;
        };

        vm.activate = function () {
        }

        vm.checked.subscribe(function () {
            if (vm.checked() == false) {
                vm.credentialPrerequisiteRequest.CreateApplications = false;
                vm.createApplications(false);
            }

            if (vm.checked() == true) {
                vm.credentialPrerequisiteRequest.CreateApplications = true;
                vm.createApplications(true);
            }
        });

        return vm;
    };
});