define([], function () {
    return {
        getInstance: getInstance
    };

    function getInstance() {
        var event = Q.defer();

        var vm = {
            event: event.promise,
            message: ko.observable(),
            invoiceId: ko.observable(),
        };

        vm.show = function (invoiceId) {
            vm.message(ko.Localization('Naati.Resources.Finance.resources.InvoiceCreatedSuccessfully').format(invoiceId));
            vm.invoiceId(invoiceId);
            $('#invoiceCreatedSuccessfullyModal').modal('show');
        };

        vm.newInvoce = function () {
            event.notify({
                name: 'NewInvoice'
            });

            close();
        };

        vm.downloadInvoice = function () {
            return 'api/finance/downloadInvoice?number=' + vm.invoiceId() + '&type=Invoice&location=Wiise';
        };

        function close() {
            $('#invoiceCreatedSuccessfullyModal').modal('hide');
        }

        return vm;
    }
});