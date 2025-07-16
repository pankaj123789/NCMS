define([
    'services/util',
], function (util) {
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

        var serverModel = {
        };

        var vm = {
            application: defaultParams.application,
            action: defaultParams.action,
            credentialRequest: defaultParams.credentialRequest,
            visibleSteps: defaultParams.visibleSteps,
            option: serverModel,
        };

        vm.load = function () {
        };

        vm.isValid = function () {
            return true;
        };

        return vm;
    };
});