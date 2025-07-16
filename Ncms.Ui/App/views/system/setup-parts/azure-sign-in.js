define([
    'services/system-data-service',
], function (systemService) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            azureAuthentication: azureAuthentication
        };

        function azureAuthentication() {
            systemService.getFluid("AadAuthUrl").then(
                function (url) {
                    // todo: use ajax instead of this
                    window.location.href = url;
                });
        }

        return vm;
    }
});