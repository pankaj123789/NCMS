define([
    'services/system-data-service',
], function (systemService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            wiiseAuthentication: wiiseAuthentication
        };

        function wiiseAuthentication() {
            systemService.getFluid("WiiseAuthUrl").then(
                function (url) {
                    // todo: use ajax instead of this
                    window.location.href = url;
                });
        }

        return vm;
    }
});