define([
    'modules/custom-validator',
    'services/application-data-service',
], function (customValidator, applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
            stepId : null
        };

        $.extend(defaultParams, params);

        var serverModel = {
          
        };
    
        var vm = {
            applicationReference: defaultParams.application.ApplicationReference,
            label:ko.observable()

        };
  

        vm.load = function () {
        };

        vm.postData = function () {
            return ko.toJS(serverModel);
        };

        vm.isValid = function () {

            return true;
        };

        vm.activate = function () {
            var request = {
                stepId: defaultParams.stepId(),
                applicationId: defaultParams.application.ApplicationId(),
                credentialRequestId: defaultParams.credentialRequest.Id() || 0
            };

            applicationService.getFluid('confirmationMessage', request).then(vm.label);

        }

        return vm;
    }
});