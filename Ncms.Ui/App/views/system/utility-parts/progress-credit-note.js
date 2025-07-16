define([
    'plugins/router'
], function (router) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            progressCreditNote: progressCreditNote
        };

        function progressCreditNote() {
            router.navigate('system/utility/progress-credit-note-form');
        }

        return vm;
    }
});