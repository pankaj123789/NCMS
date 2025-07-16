define([
    'services/util',
    'modules/enums',
    'services/material-request-data-service',
    'modules/common'
], function (util, enums, materialRequestService, common) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            wizardService: null,
            materialRequest: null,
            outputMaterial: null,
            action: null,
            materialRound: null,
            stepId: null
        };

        $.extend(defaultParams, params);

        var vm = {
            materialRequest: defaultParams.materialRequest,
            panelOptions: {}
        };

        var validation = ko.validatedObservable([vm.materialRequest.PanelId]);


        vm.panelOptions = {
            value: vm.materialRequest.PanelId,
            multiple: false,
            optionsValue: 'Id',
            optionsText: 'DisplayName',
            afterRender: function () { common.functions().getLookup('Panel').then(this.options) }
        };

        vm.isValid = function () {
            var isValid = validation.isValid();

            if (!isValid) {
                validation.errors.showAllMessages();
            }

            return isValid;
        };

        vm.postData = function () {

            var data = {
                PanelId: vm.materialRequest.PanelId()
        };

            return data;
        };

        return vm;
    }
});