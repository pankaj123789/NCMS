define(['durandal/app',
        'plugins/router',
        'services/requester'],
    function (app, router, requester) {

        var bills = new requester('bills');

        var vm = {
            loaded: ko.observable(false),
            source: ko.observableArray(),
            unHandledException: ko.observable(false),
            unHandledExceptionMessage: ko.observable(),
            getInvoices: ko.observableArray(),
            tableDefinition: {
                id: 'invoiceAndCreditNotesTable',
                headerTemplate: 'invoicecredit-header-template',
                rowTemplate: 'invoicecredit-row-template'
            }
        };


        vm.loaderOptions = {
            name: 'loader',
            params: {
                show: ko.observable(true),
                text: ko.observable('Loading bills...')
            }
        };

        vm.activate = function () {
            loadInvoices();
        };

        function loadInvoices() {

            var getUrl = window.location;
            var baseUrl = window.baseUrl + '/';
            vm.unHandledException(false);

            bills.getFluid('GetInvoices').then(function (data) {

                vm.loaded(true);
                vm.loaderOptions.params.show(false);

                vm.unHandledException(data.UnHandledException);
                vm.unHandledExceptionMessage(data.UnHandledExceptionMessage);

                ko.utils.arrayForEach(data.Data, function (d) {

                    var typeName = 'Invoice';

                    if (d.Type === CONST.invoiceTypes.creditNote) {
                        typeName = 'CreditNote';
                    }
                    else if (d.Type === CONST.invoiceTypes.bill) {
                        typeName = 'Bill';
                    }
                    else if (d.Type === CONST.invoiceTypes.payPal) {
                        typeName = 'PayPal';
                    }

                    var tempDate = moment(d.Date.slice(0, 10), 'YYYY-MM-DD');
                    d.Date = tempDate.format('DD-MM-YYYY'); 

                    d.TypeName = typeName;
                    
                    d.downloadUrl = null;
                    d.payOnline = null;
                    d.payPal = null;
                    d.hasBalance = null;

                    if (d.Balance) {
                        d.hasBalance = d.Balance > 0 ? true : false;
                    }

                    d.payOnline = baseUrl + 'Bills/PayOnline/?invoiceNumber=' + d.InvoiceNumber;
                    d.payPal = baseUrl + 'Bills/PayPalPayment/?invoiceNumber=' + d.InvoiceNumber;
                    d.downloadUrl = baseUrl + 'Bills/DownloadInvoicePdf?number=' + d.InvoiceNumber + '&invoiceId=' + d.InvoiceId + '&type=' + typeName + '&location=' + (d.TransactionId ? 'Sam' : 'Wiise');

                    vm.source.push(d);
                });
                
            });
        }


        vm.tableDefinition.dataTable = {
            source: vm.source,
            columnDefs: [
                {
                    targets: 0,
                }
            ],
            order: [[0, 'desc']]
        };

        vm.totalAmount = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Total;
            });

            return total;
        });

        vm.totalPayments = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Payment;
            });

            return total;
        });

        vm.totalBalance = ko.computed(function () {
            var total = 0;

            ko.utils.arrayForEach(vm.source(), function (s) {
                total += s.Balance;
            });

            return total;
        });

        vm.paymentsPercent = ko.computed(function () {
            if (!vm.totalAmount()) return 0;
            return parseInt(vm.totalPayments() / vm.totalAmount() * 100);
        });

        vm.balancePercent = ko.computed(function () {
            if (!vm.totalAmount()) return 0;
            return 100 - vm.paymentsPercent();
        });

        vm.sparklineValues = ko.computed(function () {
            return [vm.paymentsPercent(), vm.balancePercent()];
        });

        vm.sparklineOptions = {
            type: 'pie',
            height: 135,
            sliceColors: ['#27c24c', '#f05050'],
            tooltipFormat: '{{offset:offset}} {{value}}%',
            tooltipValueLookups: {
                offset: {
                    0: 'Payment Total',
                    1: 'Balance'
                }
            }
        };

        return vm;
    }
);