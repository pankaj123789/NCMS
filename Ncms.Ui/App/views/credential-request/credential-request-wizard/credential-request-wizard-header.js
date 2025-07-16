define([
], function (applicationService) {
    return {
        getInstance: getInstance
    };

    function getInstance(params) {
        var defaultParams = {
            summary: null
        };

        $.extend(defaultParams, params);    

        var vm = {
            summary: defaultParams.summary,
        };

        return vm;
    }
});