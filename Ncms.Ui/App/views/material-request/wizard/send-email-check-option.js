define([
    'modules/custom-validator',
    'services/screen/date-service',
], function (customValidator, dateService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            action: null,
            materialRound: null,
            stepId: null
        };

        $.extend(defaultParams, params);

        var serverModel = {
            PreventToSendEmailChecked: ko.observable(),
            PreventToSendEmailMessage: ko.observable(),
            ReadOnly: ko.observable(true)
        };

        var vm = {
            option: serverModel,
            optionPromise: null
        };


        vm.load = function () {
        };

        vm.postData = function () {
            return { Checked: vm.option.PreventToSendEmailChecked() };
        };

        vm.isValid = function () {

            return true;
        };

        vm.activate = function () {
            defaultParams.wizardService.getFluid('wizard/send-email-check-option').then(function (data) {
                ko.viewmodel.updateFromModel(serverModel, data);
            });

        }

        return vm;
    }
});