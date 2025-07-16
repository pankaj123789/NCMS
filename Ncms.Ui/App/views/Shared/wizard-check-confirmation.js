define([
    'modules/custom-validator',
], function (customValidator) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            action: null,
            credentialRequest: null,
            stepId: null,
            wizardService: null,
    };

        $.extend(defaultParams, params);

        var serverModel = {
            Checked: ko.observable(),
            Message: ko.observable(),
            OnDisableMessage: ko.observable()
        };

        var vm = {
            option: serverModel,
            optionPromise: null
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

            defaultParams.wizardService.getFluid('checkOptionMessage').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
            });
           
        }

        return vm;
    }
});