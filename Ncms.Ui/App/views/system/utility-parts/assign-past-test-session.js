define([
    'plugins/router'
], function (router) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            assignToPastTestSession: assignToPastTestSession
        };

        function assignToPastTestSession() {
            router.navigate('system/utility/assign-past-test-session-form');
        }

        return vm;
    }
});