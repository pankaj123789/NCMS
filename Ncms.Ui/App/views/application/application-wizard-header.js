define([
], function (applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            application: null,
            credentialRequest: null
        };

        $.extend(defaultParams, params);    

        var vm = {
            application: defaultParams.application,
            credentialRequest: defaultParams.credentialRequest
        };

        return vm;
    }
});