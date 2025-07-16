define([
    'plugins/router'
], function (router) {
    return {
        getInstance: getInstance
    };

    function getInstance() {

        var vm = {
            progressInvoice: progressInvoice
        };

        function progressInvoice() {
            router.navigate('system/utility/progress-invoice-form');
        }

        return vm;
    }
});